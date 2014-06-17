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
using MSFileReaderLib;
using RawMSBox;

namespace QuantiProcess {
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


    public class RawFileService : RawFileBox {

        double MassError;

        public RawFileService(){
            MassError = Convert.ToDouble(Program.Processor.MassError);
        }

        bool hasPreviousIsotope(RawData Peaks, MZData Peak, int Charge){

	        double expectedMass = Peak.Mass - (1.003/(double)Charge);

            MZData Candidate = Peaks.FindBiggestPeak(expectedMass, MassError);

            if (Candidate.Mass != 0.0 && Candidate.Intensity > 0.5*Peak.Intensity){
                return true;
            }
	        return false;
        }

        public double CheckIsotope(RawData Peaks, double MZ, int Charge, int Isotope){
	        double TargetMass = MZ + (1.003/(double)Charge)*(double)Isotope;

            MZData Peak = Peaks.FindNearestPeak(TargetMass, MassError);

            return Peak.Intensity;
        }


        public MSMatch FindMatch(int Scan, double MZ, int Charge){     

	        MSMatch ret = new MSMatch();

            ret.TimeCoeff = RTCorrection?TimeCoefs[Scan]:1;

            RawData Peaks = RawSpectra[ms2index[Scan]];

            MZData FirstPeak = Peaks.FindBiggestPeak(MZ, MassError);

		    if ( hasPreviousIsotope( Peaks, FirstPeak, Charge)) { //skip this mass if Seq turns out Seq's a part of another peak
                return ret;
            }

		    ret.FirstIsotope = FirstPeak.Intensity;
            if (Program.Processor.MetaProfile){
                ret.Score = ret.FirstIsotope; 
                ret.SecondIsotope = 0.0;
            }else{
                ret.SecondIsotope = CheckIsotope(Peaks, FirstPeak.Mass, Charge, 1);
                ret.Score = ret.FirstIsotope + ret.SecondIsotope;
                if (ret.SecondIsotope != 0.0){
                    ret.Score += CheckIsotope(Peaks, FirstPeak.Mass, Charge, 2);
                }
            }

			ret.MZ = FirstPeak.Mass;
			ret.RT = Peaks.RT;

            if (RTCorrection){
                ret.Score *= TimeCoefs[Scan];
            }


            if (ret.SecondIsotope == 0.0 && !Program.Processor.MetaProfile){
                ret.Score = 0.0;
            }

	        return ret;
        }

        public MSMatch FindMatchwithCharges(int Scan, double MZ, int Charge, double LowMZ, double HighMZ){

            //double LowMZ =  (MZ * Charge + (float) 1.007277) / (Charge + 1);
            //double HighMZ = (MZ * Charge - 1.007277) / (Charge - 1); 

            MSMatch First = FindMatch(Scan,MZ,Charge);
            if (First.Score == 0.0 || (First.SecondIsotope == 0.0 && !Program.Processor.MetaProfile)){
                return First;
            }

            MSMatch Ret;
            if (Charge > 1) {
                Ret = FindMatch(Scan,HighMZ,Charge-1);
                if (Ret.Score != 0.0 && (Ret.SecondIsotope > 0.0 || Program.Processor.MetaProfile)){
                    First.LowerCharges = Ret;
                }
            }

            Ret = FindMatch(Scan,LowMZ,Charge+1);
            if (Ret.Score != 0.0 && (Ret.SecondIsotope > 0.0 || Program.Processor.MetaProfile)){
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

                if(Program.Processor.Deconvolution){ //прихватываем другие зарядовые состояния
                    First = FindMatchwithCharges(Scan,MZ,Charge,LowMZ,HighMZ);
                }else{
    		        First = FindMatch(Scan,MZ,Charge);
                }

                //если не найден сам пик или его первый изотоп - сбрасываем 
		        if(First.Score == 0.0 || (First.SecondIsotope == 0.0 && !Program.Processor.MetaProfile)) {
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
		        if(CurrentScore > BestScore && ((fs > IsoRatio*0.5 && fs < IsoRatio * 2) || Program.Processor.MetaProfile)  )
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

                if (Program.Processor.Deconvolution)
                { //прихватываем другие зарядовые состояния
                    First = FindMatchwithCharges(Scan,Best.MZ,Charge,LowMZ,HighMZ);
                }else{
    		        First = FindMatch(Scan,Best.MZ,Charge);
                }

                if (First.Score == 0.0 || (First.SecondIsotope == 0.0 && !Program.Processor.MetaProfile)){
                    Res.MSMatches.Add(First);
                    break;
                }

                //ограничение по уровню разрешения пика 
                if (Program.Processor.MetaProfile && First.Score <= Best.Score*(Program.Processor.RTRes/100.0)){
                    break;
                }

                //ограничение по ширине пика
                if (Program.Processor.MetaProfile && Res.MSMatches[Res.MSMatches.Count-1].RT-Res.MSMatches[0].RT > Program.Processor.MaxRTWidth){
                    return null;
                }

                fs = First.SecondIsotope/First.FirstIsotope;
                //включаем здесь контроль по соотношению изотопов 
                //if (fs < IsoRatio*0.5 || fs > IsoRatio * 2 ){
                //    break;
                //}
                //пик может распространяться за границы, но апекс должен быть внутри - по крайней мере для метаболитов
                if (Program.Processor.MetaProfile && First.RT > MaxRT && First.Score > Best.Score * 1.1 ) {
                    return null;
                }

                Res.MSMatches.Add(First);
            }

            //берем сплошную область вокруг Best - назад 
            for ( Scan = IndexRev[BestIndex] ; Scan > 0 ; Scan = IndexRev[Scan]){

                if (Program.Processor.Deconvolution){ //прихватываем другие зарядовые состояния
                    First = FindMatchwithCharges(Scan,Best.MZ,Charge,LowMZ,HighMZ);
                }else{
    		        First = FindMatch(Scan,Best.MZ,Charge);
                }

                if (First.Score == 0.0 || ( First.SecondIsotope == 0.0 && !Program.Processor.MetaProfile) ){
                    Res.MSMatches.Insert(0,First);
                    break;
                }
                //ограничение по уровню разрешения пика 
                if (Program.Processor.MetaProfile && First.Score <= Best.Score*(Program.Processor.RTRes/100.0)){
                    break;
                }

                //ограничение по ширине пика
                if (Program.Processor.MetaProfile && Res.MSMatches[Res.MSMatches.Count-1].RT-Res.MSMatches[0].RT > Program.Processor.MaxRTWidth){
                    return null;
                }

                fs = First.SecondIsotope/First.FirstIsotope;
                //включаем здесь контроль по соотношению изотопов 
                //if (fs < IsoRatio*0.5 || fs > IsoRatio * 2 ){
                //    break;
                //}
                Res.MSMatches.Insert(0,First);

                //пик может распространяться за границы, но апекс должен быть внутри - по крайней мере для метаболитов
                if (Program.Processor.MetaProfile && First.RT < MinRT && First.Score > Best.Score * 1.1 ) {
                    return null;
                }

            }
            if (Program.Processor.MetaProfile && Res.MSMatches[Res.MSMatches.Count-1].RT-Res.MSMatches[0].RT < Program.Processor.MinRTWidth){
                 return null;
            }

            Res.Estimate(IsoRatio);
            Res.CleanUp();

            return Res;
        	
        }


    }
}
