/*******************************************************************************
  Copyright 2009-2014 Yaroslav Lyutvinskiy <Yaroslav.Lyutvinskiy@ki.se> and 
  Roman Zubarev <Roman.Zubarev@ki.se>
 
  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.using System;
 
 *******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quanty.Properties;

namespace Quanty
{
    public class PeptidePair{
        public double Ratio;
        public double ESIFactor;
        public double MaxConv; //максимальтное значение конволюции 
        public double Shift;   //смещение при котором достигается максимальное значяение конволюции 
        public QPeptide Peptide;
        public QMatch LeftMatch;
        public QMatch RightMatch;
        public bool Exists{
            get {
                return LeftMatch != null && RightMatch != null && LeftMatch.Score != 0.0 && RightMatch.Score != 0.0;
            }
        }
        public PeptidePair(QPeptide Pept, int Left, int Right){
            Ratio = 0.0;
            ESIFactor = 1.0;
            MaxConv = 0.0;
            Shift = 0.0;
            Peptide = Pept;
            if (Pept.Matches[Left] != null ){
                LeftMatch = new QMatch(Pept.Matches[Left]);
            }else{
                LeftMatch = null;
            }
            if (Pept.Matches[Right] != null ){
                //RightMatch - не меняется - поэтому может быть представлен ссылкой в основной экземпляр данных
                RightMatch = Pept.Matches[Right];
            }else{
                RightMatch = null;
            }
        }

        public class byApexRT : IComparer<PeptidePair> {
            public int Compare(PeptidePair x, PeptidePair y){
                if (!x.Exists && !y.Exists) return 0;
                if (!x.Exists) return -1;
                if (!y.Exists) return 1;
                if (x.LeftMatch.ApexRT>y.LeftMatch.ApexRT) { return 1;} 
                else if (x.LeftMatch.ApexRT == y.LeftMatch.ApexRT) { 
                    if (x.LeftMatch.Score > y.LeftMatch.Score) {return 1;}
                    else if (x.LeftMatch.Score == y.LeftMatch.Score) {return 0;}
                    return -1; 
                }
                else return -1;
            }
        }

        public class bySet: IComparer<PeptidePair> {
            public int Compare(PeptidePair x, PeptidePair y){
                return (new QPeptide.bySet()).Compare(x.Peptide, y.Peptide);
            }
        }

    }

    public class ProteinPair{
        public int NCount;
        public double LeftScore;
        public double RightScore;
        public double RSquare;
        public double Slope; 
        public double SlopeDelta;
        public double Median;
        public double PrAverage;
        public double MinInterval;
        public double MaxInterval;
        public double PValue;
        public List<PeptidePair> Peptides;
        public QProtein Protein;

        public ProteinPair(){
            Peptides = new List<PeptidePair>();
        }

        public void CleanUp(){
            for ( int i = Peptides.Count -1 ; i >= 0 ; i--){
                if (!Peptides[i].Exists){
                    Peptides.RemoveAt(i);
                }
            }
            NCount = Peptides.Count;
        }

        double RSquareCalc()
        {
            if (NCount < 3) {
                return 0.0;
            }
            //расчитываем средние 
            double Sum1=0.0 ,Sum2 = 0.0;
            for (int i = 0 ; i < Peptides.Count ; i++){
                Sum1 += Peptides[i].RightMatch.Score;
                Sum2 += Peptides[i].LeftMatch.Score;
            }
            double Ave1 = Sum1 / NCount;
            double Ave2 = Sum2 / NCount;
            //рассчитывем линию 
            Sum1 = 0.0; Sum2 = 0.0;
            for (int i = 0 ; i < Peptides.Count ; i++){
                Sum1 += (Peptides[i].RightMatch.Score - Ave1 )*(Peptides[i].LeftMatch.Score - Ave2);
                Sum2 += (Peptides[i].LeftMatch.Score - Ave2 )*(Peptides[i].LeftMatch.Score - Ave2);
            }
            if (Sum2 == 0.0) return 0.0;
            double a = Sum1/Sum2;
            double b = Ave1 - a*Ave2;
            //рассчитываем R2
            Sum1 = 0.0; Sum2 = 0.0;
            for (int i = 0 ; i < Peptides.Count ; i++){
                double Ost = (Peptides[i].RightMatch.Score - (a*Peptides[i].LeftMatch.Score+b));
                Sum1 += Ost*Ost;
                Sum2 += (Peptides[i].RightMatch.Score - Ave1 )*(Peptides[i].LeftMatch.Score - Ave1);
            }
            if (Sum2 == 0.0) return 0.0;
            return 1.0 - Sum1/Sum2;
        }

        public void ProteinsEst(){
            LeftScore = 0.0;
            RightScore = 0.0;
            for (int j = 0 ; j < Peptides.Count ; j++){
                if (Peptides[j].Exists) {
                    LeftScore += Peptides[j].LeftMatch.Score;
                    RightScore += Peptides[j].RightMatch.Score;
                }
            }
        }

        public double SlopeCalc(){
            double Sum1= 0.0, Sum2 = 0.0, SumX = 0.0;
            for( int i = 0 ; i < Peptides.Count; i++){
                Sum1 += Peptides[i].RightMatch.Score*Peptides[i].LeftMatch.Score;
                Sum2 += Peptides[i].LeftMatch.Score*Peptides[i].LeftMatch.Score;
                SumX += Peptides[i].RightMatch.Score;
            }
            if (Sum2 != 0){
                Slope = Sum2/Sum1;
            }else{
                Slope = 0.0;
            }

            if (NCount<2) {
                SlopeDelta = 0.0;
                return Slope;
            }

            //А теперь - доверительные интервалы для slope - как параметра a линейной регрессии 
            double SumXD = 0.0, SumYD = 0.0;
            double AveX = SumX / NCount;
            for( int i = 0 ; i < Peptides.Count; i++){
                SumXD += Math.Pow(Peptides[i].RightMatch.Score-AveX,2.0);
                SumYD += Math.Pow(Peptides[i].LeftMatch.Score - Peptides[i].RightMatch.Score*Slope,2.0);
            }
            SlopeDelta = Math.Sqrt((SumYD)/(SumXD*(NCount-1)))*alglib.studenttdistr.invstudenttdistribution(NCount-1,0.833);
            SlopeDelta = SlopeDelta / Slope;
            
            return Slope;

        }

        public double MedianCalc(){
            List<double> MList = new List<double>();
            for( int i = 0 ; i < Peptides.Count ; i++){
                MList.Add(Peptides[i].LeftMatch.Score/Peptides[i].RightMatch.Score);
            }
            MList.Sort();
            if (MList.Count > 0){ 
                if (MList.Count % 2 != 0){
                    return MList[MList.Count/2];
                }else{
                    return Math.Sqrt(MList[MList.Count/2-1]*MList[MList.Count/2]);
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
            for( int i = 0 ; i < Peptides.Count ; i++){
                Rats.Add(Math.Log(Peptides[i].LeftMatch.Score/Peptides[i].RightMatch.Score));
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

        public void MakeStats(){
            MinInterval = 0.0;
            MaxInterval = 0.0;
            RSquare = RSquareCalc();
            ProteinsEst();
            Slope = SlopeCalc(); 
            Median = MedianCalc();
            DoverIntervCalc();
            PValue = PValueCalc();
        }

        public class byIPI : IComparer<ProteinPair> {
            public int Compare(ProteinPair x, ProteinPair y){
                if (string.Compare(x.Protein.ipi,y.Protein.ipi)<0) { return -1;} 
                if (string.Compare(x.Protein.ipi,y.Protein.ipi)>0) { return 1;} 
                return 0;
            }
        }

    }
}