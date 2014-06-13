using System;
using System.Collections.Generic;
using XRAWFILE2Lib;
using Quanty.Properties;

namespace Quanty {
    public class QMatch {
        //информация в текущем файле 
        public double ApexRT;   // RT в апексе хроматографического пика
        public double ApexScore;//Интенсивность в точке апекса
        public double ApexMZ;       // MZ в точке апекса
        public int ApexIndex; 
        public double Score;    //интеграл интенсивности 
        public double RTDisp;   //standart mean deviation of RT
        public double IsotopeRatio; //отношение первого и второго изотопов

        // справедливо только для парно сравниваемых LC-MS
        public double MaxConv; //максимальтное значение конволюции 
        public double Shift;   //смещение при котором достигается максимальное значяение конволюции 

        public double LongESICoef;

        public double ShitRatio; //сделано временно для нужд простынок и матриц

        public List<MSMatch> MSMatches;

        public QMatch(){
            Clean();
        }

        public QMatch(QMatch Match){
            ApexRT = Match.ApexRT;   
            ApexScore = Match.ApexScore;
            ApexMZ = Match.ApexMZ;      
            ApexIndex = Match.ApexIndex; 
            Score = Match.Score;    
            RTDisp = Match.RTDisp;
            IsotopeRatio = Match.IsotopeRatio; 
            MaxConv = Match.MaxConv; 
            Shift = Match.Shift;   
            LongESICoef = Match.LongESICoef;
            if (Match.MSMatches != null){
                MSMatches = new List<MSMatch>();
                MSMatch MS;
                for (int i = 0 ; i < Match.MSMatches.Count ; i++){
                    MS = new MSMatch(Match.MSMatches[i]);
                    MSMatches.Add(MS); 
                }
            }else{
                MSMatches = null;
            }
        }


        public void Clean(){
            ApexRT = 0.0;   
            Score = 0.0;    
            ApexScore = 0.0;
            ApexMZ = 0.0;       
            RTDisp = 0.0;   
            IsotopeRatio = 0.0; 
            ApexIndex = 0;
            LongESICoef = 1.0;
            ShitRatio = 0.0;
            MSMatches = null;
        }

        public void CleanUp(){
            //цикл вперед
            bool UpperFlag = true, LowerFlag = true;
            for (int i = ApexIndex ; i < MSMatches.Count ; i++){
                UpperFlag &= (MSMatches[i].UpperCharges != null);
                LowerFlag &= (MSMatches[i].LowerCharges != null);
                if (!UpperFlag) 
                    MSMatches[i].UpperCharges = null;  
                if (!LowerFlag) 
                    MSMatches[i].LowerCharges = null;  
            }
            UpperFlag = true;
            LowerFlag = true;
            for (int i = ApexIndex ; i >= 0 ; i--){
                UpperFlag &= (MSMatches[i].UpperCharges != null);
                LowerFlag &= (MSMatches[i].LowerCharges != null);
                if (!UpperFlag) 
                    MSMatches[i].UpperCharges = null;  
                if (!LowerFlag) 
                    MSMatches[i].LowerCharges = null;  
            }
            //Двойной апекс за пределами ворот
            int MinIndex = ApexIndex;
            for (int i = ApexIndex+1 ; i < MSMatches.Count ; i++){
                //запоминаем минимум 
                if (MSMatches[i].Score < MSMatches[MinIndex].Score){
                    MinIndex = i; 
                }
                //пора резать при условии разрешения на полувысоте 
                if (MSMatches[i].Score > MSMatches[ApexIndex].Score && 
                    MSMatches[MinIndex].Score*2 < MSMatches[ApexIndex].Score ){
                    MSMatches.RemoveRange(MinIndex+2, MSMatches.Count-MinIndex-2);
                    MSMatches[MinIndex+1].Score = 0.0;
                    break;
                }
            }
            MinIndex = ApexIndex;
            for (int i = ApexIndex-1 ; i >=0  ; i--){
                //запоминаем минимум 
                if (MSMatches[i].Score < MSMatches[MinIndex].Score){
                    MinIndex = i; 
                }
                if (MSMatches[i].Score > MSMatches[ApexIndex].Score && 
                    MSMatches[MinIndex].Score*2 < MSMatches[ApexIndex].Score ){
                    MSMatches[MinIndex-1].Score = 0.0;
                    MSMatches.RemoveRange(0,MinIndex-1);
                    ApexIndex = ApexIndex - MinIndex +1;
                    break;
                }
            }
            Estimate(IsotopeRatio);
        }


        //на основании подготовленной коллекции MSMatches 
        //рассчитываем общий вес, характеристики апекса 
        //дисперсию пика и отношение второго и первого изотопов 
        public void Estimate(double TheorIso){ 
            double CurrentScore = 0.0;
            double FirstIsotope = 0.0 , SecondIsotope = 0.0;
            Score = 0.0;
            ApexIndex = 0;
            MSMatches.Sort(new MSMatchbyRT());
            List<double> Intens = new List<double>();
            List<double> Moments = new List<double>();
            for (int i = 0 ; i < MSMatches.Count ; i++){
                CurrentScore = MSMatches[i].Score + 
                    (MSMatches[i].UpperCharges == null ? 0 : MSMatches[i].UpperCharges.Score)+
                    (MSMatches[i].LowerCharges == null ? 0 : MSMatches[i].LowerCharges.Score);
                //FirstIsotope = MSMatches[i].FirstIsotope + 
                //    (MSMatches[i].UpperCharges == null ? 0 : MSMatches[i].UpperCharges.FirstIsotope)+
                //    (MSMatches[i].LowerCharges == null ? 0 : MSMatches[i].LowerCharges.FirstIsotope);
                //SecondIsotope = MSMatches[i].SecondIsotope + 
                //    (MSMatches[i].UpperCharges == null ? 0 : MSMatches[i].UpperCharges.SecondIsotope)+
                //    (MSMatches[i].LowerCharges == null ? 0 : MSMatches[i].LowerCharges.SecondIsotope);
                //double IsoRatio = SecondIsotope/FirstIsotope;
//                if (IsoRatio < TheorIso*3.0 && IsoRatio > TheorIso*0.33) {
//                    Score += CurrentScore;
                Intens.Add(CurrentScore);
                Moments.Add(CurrentScore*MSMatches[i].RT);
//                }
                if (MSMatches[i].RT == ApexRT ){
                    ApexScore = CurrentScore;
                    ApexMZ = MSMatches[i].MZ;
                    ApexIndex = i;
                }
            }

            //Так это сделано в 2.2.0
            FirstIsotope = MSMatches[ApexIndex].FirstIsotope + 
                (MSMatches[ApexIndex].UpperCharges == null ? 0 : MSMatches[ApexIndex].UpperCharges.FirstIsotope)+
                (MSMatches[ApexIndex].LowerCharges == null ? 0 : MSMatches[ApexIndex].LowerCharges.FirstIsotope);
            SecondIsotope = MSMatches[ApexIndex].SecondIsotope + 
                (MSMatches[ApexIndex].UpperCharges == null ? 0 : MSMatches[ApexIndex].UpperCharges.SecondIsotope)+
                (MSMatches[ApexIndex].LowerCharges == null ? 0 : MSMatches[ApexIndex].LowerCharges.SecondIsotope);

            double Sum = 0.0, Moment1 = 0.0 , Moment2 = 0.0;
            for( int i = 0 ; i < Intens.Count ; i++){
                Score += MSMatches[i].Score + 
                (MSMatches[i].UpperCharges == null ? 0 : MSMatches[i].UpperCharges.Score)+
                (MSMatches[i].LowerCharges == null ? 0 : MSMatches[i].LowerCharges.Score);
                Sum+= Intens[i];
                Moment1 += Moments[i];
            }
            Moment1 = Moment1/Sum;
            for( int i = 0 ; i < Intens.Count ; i++){
                if (Moments[i]>0.0){
                    Moment2 += (Moments[i]/Intens[i]-Moment1)*(Moments[i]/Intens[i]-Moment1)*Intens[i];
                }
            }
            RTDisp = Math.Sqrt(Moment2/Sum);
            IsotopeRatio = SecondIsotope/FirstIsotope;
        }

        struct Intervs{
            public double RT;
            public double Value1;
            public double Value2;
        }

        public static double Convolution(QMatch Match1, QMatch Match2, double Shift){
            QMatch MatchTemp = null;
            //ставим первой ту что начинается раньше
            double Shift1 = Shift;
            double Shift2 = 0.0;
            if (Match1.MSMatches[0].RT+Shift1 >= Match2.MSMatches[0].RT){
                MatchTemp = Match1;
                Match1 = Match2;
                Match2 = MatchTemp;
                Shift2 = Shift;
                Shift1 = 0.0;
            }
            if (Match1.MSMatches[Match1.MSMatches.Count-1].RT+Shift1 <= Match2.MSMatches[0].RT+Shift2){
                return 0.0;
            }

            List<Intervs> Ints = new List<Intervs>();
            Intervs Interv;
            int i1 = 0, i2 = 0; 
            double max1=0.0, max2=0.0;
            double sum1 = 0.0, sum2 = 0.0;
            //первый цикл - один QMatch активен
            //убираем влияние временного коэффициэнта 
            for (i1 = 0 ; i1<Match1.MSMatches.Count ; i1++){
                Match1.MSMatches[i1].Score = Match1.MSMatches[i1].Score/Match1.MSMatches[i1].TimeCoeff;
            }
            for (i2 = 0 ; i2<Match2.MSMatches.Count ; i2++){
                Match2.MSMatches[i2].Score = Match2.MSMatches[i2].Score/Match2.MSMatches[i2].TimeCoeff;
            }
            i1 = 0; i2 = 0; 

            do {
                Interv = new Intervs();
                Interv.RT = Match1.MSMatches[i1].RT+Shift1;
                Interv.Value1 = Match1.MSMatches[i1].Score;
                Interv.Value2 = 0.0;
                Ints.Add(Interv);
                max1 = Match1.MSMatches[i1].Score > max1 ? Match1.MSMatches[i1].Score : max1;
                sum1 += Match1.MSMatches[i1].Score * Match1.MSMatches[i1].TimeCoeff;
                i1++;
            }while (Match1.MSMatches[i1].RT+Shift1 < Match2.MSMatches[0].RT+Shift2);

            //средний цикл - оба активны
            do {
                //считаем значения в точках
                Interv = new Intervs();
                if (Match1.MSMatches[i1].RT+Shift1 < Match2.MSMatches[i2].RT+Shift2){
                    max1 = Match1.MSMatches[i1].Score > max1 ? Match1.MSMatches[i1].Score : max1;
                    sum1 += Match1.MSMatches[i1].Score * Match1.MSMatches[i1].TimeCoeff;
                    Interv.RT = Match1.MSMatches[i1].RT+Shift1;
                    Interv.Value1 = Match1.MSMatches[i1].Score;
                    //аппроксимированные значения
                    double RTRatio = ((Match1.MSMatches[i1].RT+Shift1) - (Match2.MSMatches[i2-1].RT+Shift2)) / 
                                     ((Match2.MSMatches[i2].RT+Shift2) - (Match2.MSMatches[i2-1].RT+Shift2)); 
                    Interv.Value2 = Match2.MSMatches[i2-1].Score + 
                        RTRatio*(Match2.MSMatches[i2].Score - Match2.MSMatches[i2-1].Score);
                    i1++;
                }else{
                    max2 = Match2.MSMatches[i2].Score > max2 ? Match2.MSMatches[i2].Score : max2;
                    sum2 += Match2.MSMatches[i2].Score * Match2.MSMatches[i2].TimeCoeff;
                    Interv.RT = Match2.MSMatches[i2].RT+Shift2;
                    Interv.Value2 = Match2.MSMatches[i2].Score;
                    //аппроксимированные значения
                    double RTRatio = ((Match2.MSMatches[i2].RT+Shift2) - (Match1.MSMatches[i1-1].RT+Shift1)) / 
                                     ((Match1.MSMatches[i1].RT+Shift1) - (Match1.MSMatches[i1-1].RT+Shift1)); 
                    Interv.Value1 = Match1.MSMatches[i1-1].Score + 
                        RTRatio*(Match1.MSMatches[i1].Score - Match1.MSMatches[i1-1].Score);
                    i2++;
                }
                Ints.Add(Interv);
            }while(((Match1.MSMatches[i1-1].Score > 0.0) || (max1 == 0.0)) && //мы только что обработали нулевую точку с дальнего конца
                   ((Match2.MSMatches[i2-1].Score > 0.0) || (max2 == 0.0))); //пройдена хотя по одной значащей точке в Match1 и Match2 

            //последний цикл - опять только один активен
            if (i1 == Match1.MSMatches.Count ){
                //остался только второй 
                for( ; i2<Match2.MSMatches.Count ; i2++){
                    Interv = new Intervs();
                    Interv.RT = Match2.MSMatches[i2].RT+Shift2;
                    Interv.Value1 = 0.0;
                    Interv.Value2 = Match2.MSMatches[i2].Score;
                    Ints.Add(Interv);
                    max2 = Match2.MSMatches[i2].Score > max2 ? Match2.MSMatches[i2].Score : max2;
                    sum2 += Match2.MSMatches[i2].Score * Match2.MSMatches[i2].TimeCoeff;
                }
            }else{
                //остался только первый 
                for( ; i1<Match1.MSMatches.Count ; i1++){
                    Interv = new Intervs();
                    Interv.RT = Match1.MSMatches[i1].RT+Shift1;
                    Interv.Value1 = Match1.MSMatches[i1].Score;
                    Interv.Value2 = 0.0;
                    Ints.Add(Interv);
                    max1 = Match1.MSMatches[i1].Score > max1 ? Match1.MSMatches[i1].Score : max1;
                    sum1 += Match1.MSMatches[i1].Score * Match1.MSMatches[i1].TimeCoeff;
                }
            }
            //возвращаем временной коэффициэнт
            for (i1 = 0 ; i1<Match1.MSMatches.Count ; i1++){
                Match1.MSMatches[i1].Score = Match1.MSMatches[i1].Score*Match1.MSMatches[i1].TimeCoeff;
            }
            for (i2 = 0 ; i2<Match2.MSMatches.Count ; i2++){
                Match2.MSMatches[i2].Score = Match2.MSMatches[i2].Score*Match2.MSMatches[i2].TimeCoeff;
            }

            //Применяем максимум 
            //double Ratio = max2 /max1;
            double Ratio = sum2 / sum1;
            for (int i = 0 ; i < Ints.Count ; i++){
                Interv = Ints[i];
                Interv.Value1 *= Ratio;
                Ints[i] = Interv;
            }

            //вычисление свертки 
            double Union = 0.0, Cross = 0.0;
            for (int i = 0 ; i < Ints.Count-1 ; i++){
                //полные площади
                double S10 = (Ints[i+1].RT - Ints[i].RT)*(Ints[i+1].Value2 + Ints[i].Value2)/2;
                double S20 = (Ints[i+1].RT - Ints[i].RT)*(Ints[i+1].Value1 + Ints[i].Value1)/2;
                double Sign = (Ints[i].Value1 - Ints[i].Value2)*(Ints[i+1].Value1 - Ints[i+1].Value2);
                if (Sign >= 0.0 ){ //не пересекаются
                    Cross += Math.Min(S10,S20);
                    Union += Math.Max(S10,S20);
                }else{ //пересекаются
                    //смысл коэффициэнтов - смотри в тетрадке
                    double Prop = (Ints[i].Value2 - Ints[i].Value1)/(Ints[i+1].Value1 - Ints[i+1].Value2);
                    double l2 = (Ints[i+1].RT - Ints[i].RT)/(1+Prop);
                    double l1 = l2*Prop;
                    //if (Ints[i].Value1 > Ints[i+1].Value1){ double buf = l1; l1=l2; l2 = buf; }
                    double S1 = Math.Abs(Ints[i].Value2 - Ints[i].Value1)*l1/2;
                    double S2 = Math.Abs(Ints[i+1].Value1 - Ints[i+1].Value2)*l2/2;
                    double S0;
                    if (Ints[i].Value1 < Ints[i].Value2 ){ //if (a1>b1)
                        S0 = S10 - S1;
                    }else{
                        S0 = S20 -S1; 
                    }
                    Cross += S0;
                    Union += S0 + S1 +S2;
                }
            }
            return Cross/Union; 
        }

        public static double CalcMaxConv(QMatch Match1, QMatch Match2, ref double Shift){
            //с шагом в 1 секунду пробегаем весь диапазон 
            //первый нужно сместить так чтобы последняя точка первого приходилась на первую точку второго 
            double FirstShift = Match2.MSMatches[0].RT - Match1.MSMatches[Match1.MSMatches.Count-1].RT;
            double LastShift =  Match2.MSMatches[Match2.MSMatches.Count-1].RT - Match1.MSMatches[0].RT;
            double ShiftForMax = 0.0;
            double MaxConv = 0.0;
            for (double CurrShift = FirstShift ; CurrShift <= LastShift ; CurrShift+=(1.0/60.0)){
                double Conv = QMatch.Convolution(Match1,Match2,CurrShift);
                if (Conv > MaxConv){
                    MaxConv = Conv;
                    ShiftForMax = CurrShift;
                }
            }
            //с шагом в одну десятую секунды 
            FirstShift = ShiftForMax - 1.0/60.0;
            LastShift =  ShiftForMax + 1.0/60.0;
            MaxConv = 0.0;
            for (double CurrShift = FirstShift ; CurrShift <= LastShift ; CurrShift+=(1.0/600.0)){
                double Conv = QMatch.Convolution(Match1,Match2,CurrShift);
                if (Conv > MaxConv){
                    MaxConv = Conv;
                    ShiftForMax = CurrShift;
                }
            }
            //с шагом в одну сотую секунды 
            FirstShift = ShiftForMax - 1.0/600.0;
            LastShift =  ShiftForMax + 1.0/600.0;
            MaxConv = 0.0;
            for (double CurrShift = FirstShift ; CurrShift <= LastShift ; CurrShift+=(1.0/6000.0)){
                double Conv = QMatch.Convolution(Match1,Match2,CurrShift);
                if (Conv > MaxConv){
                    MaxConv = Conv;
                    ShiftForMax = CurrShift;
                }
            }
            Shift = ShiftForMax;
            return MaxConv;
        }

        public void CutTails(QMatch Ext){
            //упрощенная версия - от апекса 
            double Shift = ApexRT - Ext.ApexRT;
            //double Shift = -Ext.Shift;

            int CutOff, i;
            if (MSMatches == null || Ext.MSMatches == null) {
                return;
            }
            //сортируем
            //передние хвосты
            if (MSMatches[0].RT - Shift > Ext.MSMatches[0].RT){
                //наш хвост короче
                for (i=1 ; i < Ext.MSMatches.Count ; i++){
                    if (MSMatches[0].RT - Shift < Ext.MSMatches[i].RT){
                        break;
                    }
                }
                if (i < Ext.MSMatches.Count) {
                    //Ext.MSMatches.RemoveRange(0,i-1);
                    ////ноль будет объявлен там же где и ноль у образца 
                    //Temp = ((MSMatches[0].RT - Shift) - Ext.MSMatches[i-1].RT)/2;
                    //Ext.MSMatches[i-1].RT = MSMatches[0].RT - Shift;
                    //Ext.MSMatches[i].RT = Ext.MSMatches[i].RT + Temp;

                    CutOff = (Math.Abs(MSMatches[0].RT - Shift - Ext.MSMatches[i].RT) <  Math.Abs(MSMatches[0].RT - Shift - Ext.MSMatches[i-1].RT))?i:i-1;

                    //Ext.MSMatches.RemoveRange(0,CutOff); - предидущее
                    if (CutOff > 0){
                        Ext.MSMatches[CutOff-1].Score =0.0;
                        Ext.MSMatches.RemoveRange(0,CutOff-1);
                    }
                }
            }else{
                //их хвост короче
                for (i=1 ; i < MSMatches.Count ; i++){
                    if (Ext.MSMatches[0].RT + Shift < MSMatches[i].RT){
                        break;
                    }
                }
                if (i < MSMatches.Count){
                    CutOff = (Math.Abs(MSMatches[i].RT - Shift - Ext.MSMatches[0].RT) <  Math.Abs(MSMatches[i-1].RT - Shift - Ext.MSMatches[0].RT))?i:i-1;
                    //MSMatches.RemoveRange(0,CutOff);- предидущее
                    if (CutOff > 0){
                        MSMatches[CutOff-1].Score =0.0;
                        MSMatches.RemoveRange(0,CutOff-1);
                    }
                }
            }

            //задние хвосты
            int IntCount = MSMatches.Count-1;
            int ExtCount = Ext.MSMatches.Count-1;

            if (MSMatches[IntCount].RT - Shift < Ext.MSMatches[ExtCount].RT){
                //наш хвост короче
                for (i=ExtCount-1 ; i >= 0 ; i--){
                    if (MSMatches[IntCount].RT - Shift > Ext.MSMatches[i].RT){
                        break;
                    }
                }
                if (i>=0){
                    CutOff = (Math.Abs(MSMatches[IntCount].RT - Shift - Ext.MSMatches[i].RT) <  Math.Abs(MSMatches[IntCount].RT - Shift - Ext.MSMatches[i+1].RT))?i:i+1;
                    //Ext.MSMatches.RemoveRange(CutOff+1, ExtCount-CutOff); - предыдущее 
                    if (CutOff <  Ext.MSMatches.Count-1 ){
                        Ext.MSMatches[CutOff+1].Score =0.0;
                        Ext.MSMatches.RemoveRange(CutOff+2, ExtCount-CutOff-1); 
                    }
                }
            }else{
                //их хвост короче
                for (i=IntCount-1 ; i >= 0 ; i--){
                    if (Ext.MSMatches[ExtCount].RT + Shift > MSMatches[i].RT){
                        break;
                    }
                }
                if (i>=0){
                    CutOff = (Math.Abs(MSMatches[i].RT - Shift - Ext.MSMatches[ExtCount].RT) <  Math.Abs(MSMatches[i+1].RT - Shift - Ext.MSMatches[ExtCount].RT))?i:i+1;
                    //MSMatches.RemoveRange(CutOff+1, IntCount-CutOff);- предыдущее 
                    if (CutOff <  MSMatches.Count-1 ){
                        MSMatches[CutOff+1].Score =0.0;
                        MSMatches.RemoveRange(CutOff+2, IntCount-CutOff-1); 
                    }
                }
            }
            this.Estimate(IsotopeRatio);
            Ext.Estimate(Ext.IsotopeRatio);

        }

        public void CutTails1(QMatch Ext){
            double Shift0 = ApexRT - Ext.ApexRT;
            double Shift = -Ext.Shift;
            double Temp;

            int i;
            if (MSMatches == null || Ext.MSMatches == null) {
                return;
            }
            //сортируем
            //передние хвосты
            if (MSMatches[0].RT - Shift > Ext.MSMatches[0].RT){
                //наш хвост короче
                for (i=1 ; i < Ext.MSMatches.Count ; i++){
                    if (MSMatches[0].RT - Shift < Ext.MSMatches[i].RT){
                        break;
                    }
                }
                if (i < Ext.MSMatches.Count) {
                    Ext.MSMatches.RemoveRange(0,i-1);
                    //ноль будет объявлен там же где и ноль у образца 
                    Temp = ((MSMatches[0].RT - Shift) - Ext.MSMatches[0].RT)/2;
                    Ext.MSMatches[0].RT = MSMatches[0].RT - Shift;
                    Ext.MSMatches[0].Score = 0.0; 
                    Ext.MSMatches[1].RT = Ext.MSMatches[1].RT + Temp;
//                    CutOff = (Math.Abs(MSMatches[0].RT - Shift - Ext.MSMatches[i].RT) <  Math.Abs(MSMatches[0].RT - Shift - Ext.MSMatches[i-1].RT))?i:i-1;
//                    Ext.MSMatches.RemoveRange(0,CutOff);
                }
            }else{
                //их хвост короче
                for (i=1 ; i < MSMatches.Count ; i++){
                    if (Ext.MSMatches[0].RT + Shift < MSMatches[i].RT){
                        break;
                    }
                }
                if (i < MSMatches.Count){

                    MSMatches.RemoveRange(0,i-1);
                    //ноль будет объявлен там же где и ноль у образца 
                    Temp = (Ext.MSMatches[0].RT - (MSMatches[0].RT - Shift) )/2;
                    MSMatches[0].RT = Ext.MSMatches[0].RT + Shift;
                    MSMatches[0].Score = 0.0; 
                    MSMatches[1].RT = MSMatches[1].RT + Temp;

                    //CutOff = (Math.Abs(MSMatches[i].RT - Shift - Ext.MSMatches[0].RT) <  Math.Abs(MSMatches[i-1].RT - Shift - Ext.MSMatches[0].RT))?i:i-1;
                    //MSMatches.RemoveRange(0,CutOff);
                }
            }

            //задние хвосты
            int IntCount = MSMatches.Count-1;
            int ExtCount = Ext.MSMatches.Count-1;

            if (MSMatches[IntCount].RT - Shift < Ext.MSMatches[ExtCount].RT){
                //наш хвост короче
                for (i=ExtCount-1 ; i >= 0 ; i--){
                    if (MSMatches[IntCount].RT - Shift > Ext.MSMatches[i].RT){
                        break;
                    }
                }
                if (i>=0){
                    Ext.MSMatches.RemoveRange(i+2,Ext.MSMatches.Count-i-2);
                    //ноль будет объявлен там же где и ноль у образца 
                    Temp = ((MSMatches[MSMatches.Count-1].RT - Shift) - Ext.MSMatches[Ext.MSMatches.Count-1].RT)/2;
                    Ext.MSMatches[Ext.MSMatches.Count-1].RT = MSMatches[MSMatches.Count-1].RT - Shift;
                    Ext.MSMatches[Ext.MSMatches.Count-1].Score = 0.0; 
                    Ext.MSMatches[Ext.MSMatches.Count-2].RT = Ext.MSMatches[Ext.MSMatches.Count-2].RT + Temp;

                    //CutOff = (Math.Abs(MSMatches[IntCount].RT - Shift - Ext.MSMatches[i].RT) <  Math.Abs(MSMatches[IntCount].RT - Shift - Ext.MSMatches[i+1].RT))?i:i+1;
                    //Ext.MSMatches.RemoveRange(CutOff+1, ExtCount-CutOff);
                }
            }else{
                //их хвост короче
                for (i=IntCount-1 ; i >= 0 ; i--){
                    if (Ext.MSMatches[ExtCount].RT + Shift > MSMatches[i].RT){
                        break;
                    }
                }
                if (i>=0){
                    MSMatches.RemoveRange(i+2,MSMatches.Count-i-2);
                    //ноль будет объявлен там же где и ноль у образца 
                    Temp = (Ext.MSMatches[Ext.MSMatches.Count-1].RT - (MSMatches[MSMatches.Count-1].RT - Shift))/2;
                    MSMatches[MSMatches.Count-1].RT = Ext.MSMatches[Ext.MSMatches.Count-1].RT + Shift;
                    MSMatches[MSMatches.Count-1].Score = 0.0; 
                    MSMatches[MSMatches.Count-2].RT = MSMatches[MSMatches.Count-2].RT + Temp;

                    //CutOff = (Math.Abs(MSMatches[i].RT - Shift - Ext.MSMatches[ExtCount].RT) <  Math.Abs(MSMatches[i+1].RT - Shift - Ext.MSMatches[ExtCount].RT))?i:i+1;
                    //MSMatches.RemoveRange(CutOff+1, IntCount-CutOff);
                }
            }
            this.Estimate(IsotopeRatio);
            Ext.Estimate(Ext.IsotopeRatio);

        }
    }



    public class MSMatch{
        public double Score;
        public double MZ;
        public double RT;
        public double FirstIsotope;
        public double SecondIsotope;
        public MSMatch LowerCharges;
        public MSMatch UpperCharges;
        public double TimeCoeff;

        public MSMatch(){
        }

        public MSMatch(MSMatch MS){
            Score = MS.Score;
            MZ = MS.MZ;
            RT = MS.RT;
            FirstIsotope = MS.FirstIsotope;
            SecondIsotope = MS.SecondIsotope;
            TimeCoeff = MS.TimeCoeff;
            if (LowerCharges != null){
                LowerCharges = new MSMatch(MS.LowerCharges);
            }else{
                LowerCharges = null;
            }
            if (UpperCharges != null){
                UpperCharges = new MSMatch(MS.UpperCharges);
            }else{
                UpperCharges = null;
            }
        }
    }

    public class MSMatchbyRT : IComparer<MSMatch> {
        public int Compare(MSMatch x, MSMatch y){
            if (x.RT>y.RT) { return 1;} 
            else if (x.RT == y.RT ) { return 0; }
            else return -1;
        }
    }


    public partial class RawFileBox {

        bool hasPreviousIsotope(RawData Peaks, int Index, int Charge){

	        double expectedMass = Peaks.Data[Index].Mass - (1.003/(double)Charge);
	        double mmi = expectedMass * ((1000000.0 - Convert.ToDouble(Settings.Default.MassError))/1000000.0);
	        double mma = expectedMass * ((1000000.0 + Convert.ToDouble(Settings.Default.MassError))/1000000.0);
        	
            foreach (MZData data in Peaks.Data){
		        if(data.Mass >= mmi && data.Mass <= mma && data.Intensity > 0.5*Peaks.Data[Index].Intensity ) {
			        return true;
                }
            }
	        return false;
        }

        int FindMassBelow(RawData Peaks, double Mass){
            //запас на оптимизацию - двоичный поиск
            if (Peaks.Data.GetLength(0) == 0){
                return -1;
            }
            if (Peaks.Data[0].Mass > Mass){
                return -1;
            }
            for (int i = 1 ; i < Peaks.Data.GetLength(0) ; i++){
                if (Peaks.Data[i].Mass > Mass ) {
                    return i-1;
                }
            }
            return Peaks.Data.GetLength(0)-1;
        }

        int FindMassAbove(RawData Peaks, double Mass){
            //запас на оптимизацию - двоичный поиск
            if (Peaks.Data.GetLength(0) == 0){
                return -1;
            }
            if (Peaks.Data[Peaks.Data.GetLength(0)-1].Mass < Mass){
                return -1;
            }
            for (int i = 0 ; i < Peaks.Data.GetLength(0) ; i++){
                if (Peaks.Data[i].Mass > Mass ) {
                    return i;
                }
            }
            return 0;
        }

        public MSMatch FindMatch(int Scan, double MZ, int Charge){     

	        MSMatch ret = new MSMatch();

	        ret.Score = 0.0;
            ret.FirstIsotope = 0.0;
            ret.SecondIsotope = 0.0;
            ret.TimeCoeff = RTCorrection?TimeCoefs[Scan]:1;

	        double maxs = 0.0, curs, bestd, tk10low, tk10high;

	        int i, t;

	        int cur = ms2index[Scan];


	        //ворота по массе 
	        double mmi = MZ * ((1000000.0 - Convert.ToDouble(Settings.Default.MassError))/1000000.0);
  		    double mma = MZ * ((1000000.0 + Convert.ToDouble(Settings.Default.MassError))/1000000.0);

	        RawData Peaks = RawSpectra[cur];
            ret.RT = Peaks.RT;

	        //Если сразу ясно что мы за пределами поиска

            int LowerIndex = FindMassAbove(Peaks,mmi);
            int UpperIndex = FindMassBelow(Peaks,mma);

            if (UpperIndex == -1 || LowerIndex == -1 || LowerIndex > UpperIndex ){
                return ret;
            }

	        while(LowerIndex <= UpperIndex){ //for all masses that match

		        if ( hasPreviousIsotope( Peaks, LowerIndex, Charge)) { //skip this mass if Seq turns out Seq's a part of another peak
                    LowerIndex++;
                    continue; 
                }

		        curs = Peaks.Data[LowerIndex].Intensity;

		        tk10low = 0.0; tk10high = 0.0;


		        for(i = 1; i < 3; ++i){
			        double maxs2 = Peaks.Data[LowerIndex].Mass + (1.003/(double)Charge)*(double)i;

			        double mmi2 = maxs2 * ((1000000.0 - Convert.ToDouble(Settings.Default.MassError))/1000000.0);
			        double mma2 = maxs2 * ((1000000.0 + Convert.ToDouble(Settings.Default.MassError))/1000000.0);

                    int LowerIndex2 = FindMassAbove(Peaks,mmi2);
                    int UpperIndex2 = FindMassBelow(Peaks,mma2);

                    if (LowerIndex2 > UpperIndex2 || LowerIndex2== -1 ){ //ничего не нашли - ничего и не делаем 
                        continue;
                    }

                    bestd = maxs2;
                    while (LowerIndex2 < UpperIndex2) { //если больше одного - ищем самый близкий к целевой массе
		                if(Math.Abs(maxs2 - Peaks.Data[LowerIndex2].Mass) < bestd) {
			                bestd = Math.Abs(maxs2 - Peaks.Data[LowerIndex2].Mass);
			                t = LowerIndex2;
		                }
		                LowerIndex2++;
			        }
			        if(i == 1) { 
                        tk10low = curs; 
                        tk10high = Peaks.Data[LowerIndex2].Intensity; 
                    }
			        curs += Peaks.Data[LowerIndex2].Intensity;

		        }

		        if(maxs < curs){
			        maxs = curs;
			        ret.Score = curs;
			        ret.MZ = Peaks.Data[LowerIndex].Mass;
			        ret.RT = Peaks.RT;
			        ret.FirstIsotope = tk10low; 
                    ret.SecondIsotope = tk10high;
		        }
		        LowerIndex++;
	        }

            if (RTCorrection){
                ret.Score *= TimeCoefs[Scan];
            }

            if (ESICorrection){ 
                ret.Score = ret.Score / ESICurrents[Scan];
            }

            if (ret.SecondIsotope == 0.0 ){
                ret.Score = 0.0;
            }
	        return ret;
        }

        public MSMatch FindMatchwithCharges(int Scan, double MZ, int Charge, double LowMZ, double HighMZ){

            //double LowMZ =  (MZ * Charge + (float) 1.007277) / (Charge + 1);
            //double HighMZ = (MZ * Charge - 1.007277) / (Charge - 1); 

            MSMatch First = FindMatch(Scan,MZ,Charge);
            if (First.Score == 0.0 || First.SecondIsotope == 0.0 ){
                return First;
            }

            MSMatch Ret;
            if (Charge > 1) {
                Ret = FindMatch(Scan,HighMZ,Charge-1);
                if (Ret.Score != 0.0 && Ret.SecondIsotope > 0.0){
                    First.LowerCharges = Ret;
                }
            }

            Ret = FindMatch(Scan,LowMZ,Charge+1);
            if (Ret.Score != 0.0 && Ret.SecondIsotope > 0.0){
                First.UpperCharges = Ret;
            }
            return First;
        }
        

        public QMatch FindBestScore(  double MZ, int Charge, double IsoRatio, double MinRT, double MaxRT) 
		{

            MSMatch First = null , Best = null;

            QMatch Res = new QMatch();
            Res.Clean();
            Res.MSMatches = new List<MSMatch>();

            double FirstIso = 0.0, SecondIso = 0.0;
	        double BestScore = 0.0, fs = 0.0;
            int BestIndex = 0 ;

            int Scan = 0;
            double AveRT = (MinRT+MaxRT)/2;

            RawFile.ScanNumFromRT(AveRT,ref Scan);
	        Scan = ms2index[Scan];

            //отступаем до минимального времени RT в массиве спектров 
            double LowMZ =  (MZ * Charge + (float) 1.007277) / (Charge + 1);
            double HighMZ; 
            if (Charge > 1){
                HighMZ = (MZ * Charge - 1.007277) / (Charge - 1); 
            }else{
                HighMZ = 0.0;
            }

	        while(Scan > 0 && RawSpectra[Scan].RT >= MinRT) {
		        Scan = IndexRev[Scan];
            }

            //цикл поиска лучшего 
            while( Scan != -1 && RawSpectra[Scan].RT <= MaxRT  ) {

                if(Settings.Default.Deconvolution){ //прихватываем другие зарядовые состояния
                    First = FindMatchwithCharges(Scan,MZ,Charge,LowMZ,HighMZ);
                }else{
    		        First = FindMatch(Scan,MZ,Charge);
                }

                //если не найден сам пик или его первый изотоп - сбрасываем 
		        if(First.Score == 0.0 || First.SecondIsotope == 0.0 ) {
    		        Scan = IndexDir[Scan];
                    continue;
                }

                FirstIso = First.FirstIsotope + 
                    (First.UpperCharges == null ? 0 : First.UpperCharges.FirstIsotope)+
                    (First.LowerCharges == null ? 0 : First.LowerCharges.FirstIsotope);
                SecondIso = First.SecondIsotope + 
                    (First.UpperCharges == null ? 0 : First.UpperCharges.SecondIsotope)+
                    (First.LowerCharges == null ? 0 : First.LowerCharges.SecondIsotope);
        		
		        fs = SecondIso/FirstIso;

                double CurrentScore = First.Score + 
                    (First.UpperCharges == null ? 0 : First.UpperCharges.Score)+
                    (First.LowerCharges == null ? 0 : First.LowerCharges.Score);

                //пик может распространяться за границы, но апекс должен быть внутри
		        if(CurrentScore > BestScore && fs > IsoRatio*0.5 && fs < IsoRatio * 2  )
		        {
			        BestScore = CurrentScore;
                    BestIndex = Scan; 
                    Best = First;
		        }
                //переходим к следующему full-ms
		        Scan = IndexDir[Scan];
	        }

	        if( BestScore == 0.0 ) { 
                return null;
            }

            //если хоть что-то нашли 
            //добавляем наибольшее 
            LowMZ =  (Best.MZ * Charge + (float) 1.007277) / (Charge + 1);
            if (Charge > 1){
                HighMZ = (MZ * Charge - 1.007277) / (Charge - 1); 
            }else{
                HighMZ = 0.0;
            }


            Res.ApexRT = Best.RT;
            Res.MSMatches.Add(Best);

            //берем сплошную область вокруг Best - вперед 
            for ( Scan = IndexDir[BestIndex] ; Scan > 0 ; Scan = IndexDir[Scan]){

                if(Settings.Default.Deconvolution){ //прихватываем другие зарядовые состояния
                    First = FindMatchwithCharges(Scan,Best.MZ,Charge,LowMZ,HighMZ);
                }else{
    		        First = FindMatch(Scan,Best.MZ,Charge);
                }

                if (First.Score == 0.0 || First.SecondIsotope == 0.0 ){
                    Res.MSMatches.Add(First);
                    break;
                }
                fs = First.SecondIsotope/First.FirstIsotope;
                //включаем здесь контроль по соотношению изотопов 
                //if (fs < IsoRatio*0.5 || fs > IsoRatio * 2 ){
                //    break;
                //}
                Res.MSMatches.Add(First);
            }

            //берем сплошную область вокруг Best - назад 
            for ( Scan = IndexRev[BestIndex] ; Scan > 0 ; Scan = IndexRev[Scan]){

                if(Settings.Default.Deconvolution){ //прихватываем другие зарядовые состояния
                    First = FindMatchwithCharges(Scan,Best.MZ,Charge,LowMZ,HighMZ);
                }else{
    		        First = FindMatch(Scan,Best.MZ,Charge);
                }

                if (First.Score == 0.0 || First.SecondIsotope == 0.0 ){
                    Res.MSMatches.Add(First);
                    break;
                }
                fs = First.SecondIsotope/First.FirstIsotope;
                //включаем здесь контроль по соотношению изотопов 
                //if (fs < IsoRatio*0.5 || fs > IsoRatio * 2 ){
                //    break;
                //}
                Res.MSMatches.Insert(0,First);
            }

            Res.Estimate(IsoRatio);
            Res.CleanUp();

            return Res;
        	
        }


    }
}
