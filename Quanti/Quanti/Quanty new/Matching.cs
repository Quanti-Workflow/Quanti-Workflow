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
                    if (i1 >= Match1.MSMatches.Count) break;
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

}
