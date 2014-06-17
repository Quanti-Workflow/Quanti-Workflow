using System;
using System.Collections.Generic;
using System.IO;
using Quanty.Properties;
using Mascot;

namespace Quanty {
    public class QProtMatch{
        public double RSquare;
        public double Slope; 
        public double SlopeDelta;
        public double Median;
        public double PrAverage;
        public double MinInterval;
        public double MaxInterval;
        public double Score;
    }

    public class QProtein{
        public string ipi;         //итоговый идентификатор белка               - pers - pr key
        public List<string> ipis;  //исходный набор идентификаторов белка       - pers
        public List<QPeptide> Peptides; //набор идентифицированных пептидов   - связь по ключу 
        public string Name; //имя белка (строка с идентификатором)              - pers
        public String Desc; //описание белка                                    - pers
        public double Est; //оценка белка                                       - no pers

        //группа статистичекских значений для попарной оценки белков 
        public int NCount;
        public double RSquare;
        public double RefScore;
        public double Slope; 
        public double SlopeDelta;
        public double Median;
        public double PrAverage;
        public double MinInterval;
        public double MaxInterval;
        public double PValue;
        //группа функций выполняющих статистические оценки белка 

        public int ZeroCount;
        public int AllPeptCount;
        

        //для простынки 
        public QProtMatch[] Matches;
        public double[,] MatrixMedian;
        public double[,] MatrixSlope;
        public double[,] SlopeError;
        public double[,] MatrixError;

        public class byIPI : IComparer<QProtein> {
            public int Compare(QProtein x, QProtein y){
                if (string.Compare(x.ipi,y.ipi)<0) { return -1;} 
                if (string.Compare(x.ipi,y.ipi)>0) { return 1;} 
                return 0;
            }
        }

        
        public void CreateMatches(int Number){
            Matches = new QProtMatch[Number];
            for (int i = 0 ; i < Number ; i++){
                Matches[i] = new QProtMatch();
            }
            MatrixMedian = new double[Number,Number];
            MatrixSlope = new double[Number,Number];
            SlopeError = new double[Number,Number];
            MatrixError = new double[Number,Number];
        }

        public void AddMatch(int Number){
            Matches[Number].RSquare = RSquare;
            Matches[Number].Median = Median;
            Matches[Number].Slope = Slope;
            Matches[Number].SlopeDelta = SlopeDelta;
            Matches[Number].PrAverage = PrAverage;
            Matches[Number].MinInterval = MinInterval;
            Matches[Number].MaxInterval = MaxInterval;
            Matches[Number].Score = Est;
        }

        double RSquareCalc(){
            if (Peptides.Count < 3) {
                return 0.0;
            }
            //расчитываем средние 
            NCount = 0;
            double Sum1=0.0 ,Sum2 = 0.0;
            for (int i = 0 ; i < Peptides.Count ; i++){
                if (Peptides[i].RefMatch == null || Peptides[i].RefNumberMatch == null) continue;
                if (Peptides[i].RefNumberMatch.Score==0.0 || Peptides[i].RefMatch.Score==0.0) continue;
                Sum1 += Peptides[i].RefNumberMatch.Score;
                Sum2 += Peptides[i].RefMatch.Score;
                NCount++;
            }
            if (NCount < 3) {
                return 0.0;
            }
            double Ave1 = Sum1 / NCount;
            double Ave2 = Sum2 / NCount;
            //рассчитывем линию 
            Sum1 = 0.0; Sum2 = 0.0;
            for (int i = 0 ; i < Peptides.Count ; i++){
                if (Peptides[i].RefMatch == null || Peptides[i].RefNumberMatch == null) continue;
                if (Peptides[i].RefNumberMatch.Score==0.0 || Peptides[i].RefMatch.Score==0.0) continue;
                Sum1 += (Peptides[i].RefNumberMatch.Score - Ave1 )*(Peptides[i].RefMatch.Score - Ave2);
                Sum2 += (Peptides[i].RefMatch.Score - Ave2 )*(Peptides[i].RefMatch.Score - Ave2);
            }
            if (Sum2 == 0.0) return 0.0;
            double a = Sum1/Sum2;
            double b = Ave1 - a*Ave2;
            //рассчитываем R2
            Sum1 = 0.0; Sum2 = 0.0;
            for (int i = 0 ; i < Peptides.Count ; i++){
                if (Peptides[i].RefMatch == null || Peptides[i].RefNumberMatch == null){
                    continue;
                }
                double Ost = (Peptides[i].RefNumberMatch.Score - (a*Peptides[i].RefMatch.Score+b));
                Sum1 += Ost*Ost;
                Sum2 += (Peptides[i].RefNumberMatch.Score - Ave1 )*(Peptides[i].RefNumberMatch.Score - Ave1);
            }
            if (Sum2 == 0.0) return 0.0;
            return 1.0 - Sum1/Sum2;
        }

        public void ProteinsEst(){
            Est = 0.0;
            RefScore = 0.0;
            for (int j = 0 ; j < Peptides.Count ; j++){
                if (Peptides[j].CheckExist()) {
                    Est += Peptides[j].RefMatch.Score;
                    RefScore += Peptides[j].RefNumberMatch.Score;
                }
            }
        }

        public double SlopeCalc(){
            double Sum1= 0.0, Sum2 = 0.0, SumX = 0.0;
            int Count = 0;
            for( int i = 0 ; i < Peptides.Count; i++){
                if (Peptides[i].CheckExist()){
                    Sum1 += Peptides[i].RefNumberMatch.Score*Peptides[i].RefMatch.Score;
                    Sum2 += Peptides[i].RefMatch.Score*Peptides[i].RefMatch.Score;
                    SumX += Peptides[i].RefNumberMatch.Score;
                    Count++;
                }
            }
            double Slope;
            if (Sum2 != 0){
                Slope = Sum2/Sum1;
            }else{
                Slope = 0.0;
            }

            if (Count<2) {
                SlopeDelta = 0.0;
                return Slope;
            }

            //А теперь - доверительные интервалы для slope - как параметра a линейной регрессии 
            double SumXD = 0.0, SumYD = 0.0;
            double AveX = SumX / Count;
            for( int i = 0 ; i < Peptides.Count; i++){
                if (Peptides[i].CheckExist()){
                    SumXD += Math.Pow(Peptides[i].RefNumberMatch.Score-AveX,2.0);
                    SumYD += Math.Pow(Peptides[i].RefMatch.Score - Peptides[i].RefNumberMatch.Score*Slope,2.0);
                }
            }
            SlopeDelta = Math.Sqrt((SumYD)/(SumXD*(Count-1)))*alglib.studenttdistr.invstudenttdistribution(Count-1,0.833);
            SlopeDelta = SlopeDelta / Slope;
            
            return Slope;

        }

        public double MedianCalc(){
            List<double> MList = new List<double>();
            for( int i = 0 ; i < Peptides.Count ; i++){
                if (Peptides[i].CheckExist()){
                    MList.Add(Peptides[i].RefMatch.Score/Peptides[i].RefNumberMatch.Score);
                    AllPeptCount++;
                }else{
                    ZeroCount++;
                }
            }
            MList.Sort();
            if (MList.Count > 0){ 
                if (MList.Count % 2 != 0){
                    return MList[MList.Count/2];
                }else{
                    return (MList[MList.Count/2-1]+MList[MList.Count/2])/2;
                }
            }else{
                return 0.0;
            }

        }

        public double PValueCalc(){
            if (NCount<3) return 1.0;
            double stat = Math.Sqrt((RSquare*(NCount-2))/(1-RSquare));
            return 2*(1-alglib.studenttdistr.studenttdistribution(NCount-2,stat));
        }

        public void DoverIntervCalc(){
            //собираем логарифмы отношений 
            List<double> Rats = new List<double>();
            NCount = 0;
            for( int i = 0 ; i < Peptides.Count ; i++){
                if (!Peptides[i].CheckExist()) continue;
                NCount++;
                Rats.Add(Math.Log(Peptides[i].RefMatch.Score/Peptides[i].RefNumberMatch.Score));
            }
            if (Rats.Count <2 ) {
                if (Rats.Count == 1 ){
                    PrAverage = Math.Exp(Rats[0]);
                }else{
                    PrAverage =0.0;
                }
                MinInterval = 0.0;
                MaxInterval = 0.0;
                return;
            }
            //среднее 
            double Sum = 0.0;
            for( int i = 0 ; i < Rats.Count ; i++){
                Sum += Rats[i];
            }
            double Ave = Sum/((double)Rats.Count);
            PrAverage = Math.Exp(Ave);
            //Дисперсия
            Sum = 0;
            for( int i = 0 ; i < Rats.Count ; i++){
                Sum += (Rats[i]-Ave)*(Rats[i]-Ave);
            }
            double Disp = Sum/((double)Rats.Count-1.0);
            //статистика
            double Stat = Math.Sqrt(Disp/(double)Rats.Count)*alglib.studenttdistr.invstudenttdistribution(Rats.Count-1,0.833);
            //возвращаемся к отношениям 
            MinInterval = Math.Exp(Ave-Stat);
            MaxInterval = Math.Exp(Ave+Stat);
        }

        public void MakeStats(bool Ref){
            MinInterval = 0.0;
            MaxInterval = 0.0;
            Est =0.0;
            if (!Ref){
                Est = 0.0;
                for (int j = 0 ; j < Peptides.Count ; j++){
                    if (Peptides[j].RefMatch != null ){
                        Est += Peptides[j].RefMatch.Score;
                        RSquare = 0.0;
                        Slope =0.0;
                        Median = 0.0;
                        MinInterval = 0.0;
                        MaxInterval = 0.0;
                        PValue = 0.0;
                    }
                }
                return;
            }
            RSquare = RSquareCalc();
            ProteinsEst();
            Slope = SlopeCalc(); 
            Median = MedianCalc();
            DoverIntervCalc();
            PValue = PValueCalc();
        }

        public override string ToString(){
            string Out = 
                ((ReportString[0]!='1')?"":String.Format("\"{0}\"\t",Name))+
                ((ReportString[1]!='1')?"":String.Format("{0:f4}\t",Est))+
                ((ReportString[2]!='1')?"":String.Format("{0:f4}\t",RefScore))+
                ((ReportString[3]!='1')?"":String.Format("\"{0}\"\t",Desc))+
                ((ReportString[4]!='1')?"":String.Format("{0}\t",Peptides.Count))+
                ((ReportString[5]!='1')?"":String.Format("{0}\t",NCount))+
                ((ReportString[6]!='1')?"":String.Format("{0:f4}\t",RSquare))+
                ((ReportString[7]!='1')?"":String.Format("{0:f4}\t",Slope))+
                ((ReportString[8]!='1')?"":String.Format("{0:f4}\t",Median))+
                ((ReportString[9]!='1')?"":String.Format("{0:f4}\t",PrAverage))+
                ((ReportString[10]!='1')?"":String.Format("{0:f4}\t",MinInterval))+
                ((ReportString[11]!='1')?"":String.Format("{0:f4}\t",MaxInterval))+
                ((ReportString[12]!='1')?"":String.Format("{0:e4}",PValue));
            return Out;
        }

        public static string TableCaption(){
            return (ReportString[0]=='1'?"PROTEIN ID\t":"")+
                (ReportString[1]=='1'?"SCORE\t":"")+
                (ReportString[2]=='1'?"REF. SCORE\t":"")+
                (ReportString[3]=='1'?"DESCRIPTION\t":"")+
                (ReportString[4]=='1'?"PEPTIDES IDENT\t":"")+
                (ReportString[5]=='1'?"PEPTIDES FOUND\t":"")+
                (ReportString[6]=='1'?"R-SQUARE\t":"")+
                (ReportString[7]=='1'?"SLOPE\t":"")+
                (ReportString[8]=='1'?"MEDIAN\t":"")+
                (ReportString[9]=='1'?"AVERAGE\t":"")+
                (ReportString[10]=='1'?"MIN. CONF.\t":"")+
                (ReportString[11]=='1'?"MAX. CONF.\t":"")+
                (ReportString[12]=='1'?"P-VALUE":"");
        }

        public static string ReportString;
    }

}
