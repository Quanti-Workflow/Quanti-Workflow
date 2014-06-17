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
using System.Text;
using MSFileReaderLib;
//using Quanty.Properties;

namespace RawMSBox{

    public struct MZData {
        public double Mass;
        public double Intensity;
    }

    public class RawData{
        public MZData[] Data;
        public double RT;
        //для MSX спектров
        public string Filter="";

        public int FindMassBelow(double Mass){
            //запас на оптимизацию - двоичный поиск
            if (Data.GetLength(0) == 0){
                return -1;
            }
            if (Data[0].Mass > Mass){
                return -1;
            }
            for (int i = 1 ; i < Data.GetLength(0) ; i++){
                if (Data[i].Mass > Mass ) {
                    return i-1;
                }
            }
            return Data.GetLength(0)-1;
        }

        public int FindMassAbove(double Mass){
            //запас на оптимизацию - двоичный поиск
            if (Data.GetLength(0) == 0){
                return -1;
            }
            if (Data[Data.GetLength(0)-1].Mass < Mass){
                return -1;
            }
            for (int i = 0 ; i < Data.GetLength(0) ; i++){
                if (Data[i].Mass > Mass ) {
                    return i;
                }
            }
            return 0;
        }

        public double MZPlusPPM(double MZ,double ppm){
            return MZ * ((1000000.0 + ppm) / 1000000.0);
        }

        public MZData FindNearestPeak(double MZ, double Error){
            double LowerMass = MZPlusPPM(MZ, -Error);
            double UpperMass = MZPlusPPM(MZ, Error); 

            int LowerIndex = FindMassAbove(LowerMass);
            int UpperIndex = FindMassBelow(UpperMass);

            if (LowerIndex > UpperIndex || LowerIndex== -1 ){ //ничего не нашли - ничего и не делаем 
                return new MZData();
            }

            while (LowerIndex < UpperIndex) { //если больше одного - ищем самый близкий к целевой массе
                if (LowerIndex == Data.GetLength(0)-1 || 
                    Math.Abs(MZ - Data[LowerIndex].Mass) < Math.Abs(MZ - Data[LowerIndex + 1].Mass)) {
                    break;
                }
		        LowerIndex++;
			}
            return Data[LowerIndex];
        }

        public MZData FindBiggestPeak(double MZ, double Error){
            double LowerMass = MZPlusPPM(MZ, -Error);
            double UpperMass = MZPlusPPM(MZ, Error); 

            int LowerIndex = FindMassAbove(LowerMass);
            int UpperIndex = FindMassBelow(UpperMass);

            if (LowerIndex > UpperIndex || LowerIndex== -1 ){ //ничего не нашли - ничего и не делаем 
                return new MZData();
            }

            double MaxInt = Data[LowerIndex].Intensity;
            int MaxIndex = LowerIndex;
            LowerIndex++;
            while (LowerIndex < UpperIndex) { //если больше одного - ищем самый близкий к целевой массе
                if (Data[LowerIndex].Intensity > MaxInt ) {
                    MaxInt = Data[LowerIndex].Intensity;
                    MaxIndex = LowerIndex;
                }
		        LowerIndex++;
			}
            return Data[MaxIndex];
        }
    }
    
    public class RawFileBox {
        
        public RawData[] RawSpectra; 
        public MSFileReader_XRawfile RawFile; 
        public int Spectra;
        public string RawFileName;

        public int[] ms2index; //для каждого спектра дает номер скана последнего full-MS спектра
        //заполнены только сканов соответствующих full-MS спектрам
        public int[] IndexDir; //указывает на номер скана следующего full-MS спектра
        public int[] IndexRev; //указывает на номер скана предидущего full-MS спектра

        public double[] ESICurrents; //значения тока элекстроспрея 
        public double[] TimeStamps; //промежуток после предыдущего MS-only спектра - в минутах
        public double[] TimeCoefs;
        double AverageTimeStamp;

        public bool RTCorrection;

        MZData[] Buf;

        double LowRT;
        double HighRT;
        double TotalRT;
        bool StickMode;

        //поменять на делегата
        public delegate void Progress(int Perc);
        public static Progress RepProgress;


        public RawFileBox(){
            StickMode = false;
        }

        public int LoadIndex(string FileName){

            this.RawFileName = FileName;

            RawFile = new MSFileReader_XRawfile();

            RawFile.Open(FileName);
            RawFile.SetCurrentController(0, 1);

            Spectra = 0;
            RawFile.GetNumSpectra(ref Spectra);

            if( Spectra <= 0) 
                return 0;

	        int i, lastfull = 0, total = 0;
            double TotalEsi = 0.0;

	        ms2index = new int[Spectra+1];
            IndexDir = new int[Spectra+1];
            IndexRev = new int[Spectra+1]; 
            RawSpectra = new RawData[Spectra+1];
            for(int j = 0 ; j <= Spectra ; j++){
                RawSpectra[j] = new RawData();
            }
            Buf = new MZData[1000000];

            ESICurrents = new double[Spectra+1];
            TimeStamps = new double[Spectra+1];
            TimeCoefs = new double[Spectra+1];

            string Filter = null;

            LowRT = 0.0;
            HighRT = 0.0;

            int Progress = 0; 
            for(i = 1; i <= Spectra; i++){

                if ((int)(100.0*((double)i/(double)Spectra)) > Progress) {
                    Progress = (int)(100.0*((double)i/(double)Spectra));
                    if (RepProgress != null){
                        RepProgress(Progress);
                    }
                }

		        RawFile.GetFilterForScanNum(i, ref Filter);

		        //YL - для спектров ms-only
                //Заплатка для MSX спектров
		        if((Filter.IndexOf(" ms ") != -1) && (Filter.IndexOf("FTMS") != -1)) { //is a FULL MS

			        TimeStamps[i] = RawSpectra[lastfull].RT;
                    
                    IndexDir[lastfull] = i;
			        IndexRev[i] = lastfull;

			        lastfull = i;
			        ms2index[i] = lastfull;

			        ++total;

				    RawFile.RTFromScanNum(i, ref RawSpectra[i].RT);
                    RawSpectra[i].Filter = Filter;
                    TotalRT = RawSpectra[i].RT;

                    TimeStamps[i] = RawSpectra[i].RT - TimeStamps[i];

                    object Labels = null;
                    object Values = null;
                    int ArraySize = 0;
                    double RT = 0.0; 

                    RawFile.GetStatusLogForScanNum(i, ref RT, ref Labels, ref Values , ref ArraySize);

                    for (int k = 0 ; k < ArraySize ; k++ ){
                        if ((Labels as Array).GetValue(k).ToString().Contains("Source Current")){
                            ESICurrents[i] = Convert.ToDouble((Values as Array).GetValue(k).ToString());
                            TotalEsi+=ESICurrents[i];
                        }
                    }


		        }
		        else {
			        ms2index[i] = lastfull;
		        }
		        Filter = null ;
	        }
            IndexDir[lastfull] = -1;
            TotalRT = RawSpectra[lastfull].RT;
            AverageTimeStamp = TotalRT/total;

            //пересчитаем временные коэффициэнты 
            for (i = IndexDir[0] ; IndexDir[i] != -1 ; i = IndexDir[i]) {

                TimeCoefs[i] = (TimeStamps[i]+TimeStamps[IndexDir[i]])/(2.0*AverageTimeStamp);

                ESICurrents[i] = ESICurrents[i]/(TotalEsi/(double)total);
            }
            TimeCoefs[i] = 1.0;
            
            return Spectra;
        }

        public void LoadInterval(double MinRT, double MaxRT){
            int Index = 0;
            //по границам плюс один спектр 
            while (RawSpectra[IndexDir[Index]].RT<MinRT){
                RawSpectra[Index].Data = null;
                Index = IndexDir[Index];
            }
            int Progress = 0; 
            while (RawSpectra[IndexRev[Index]].RT<MaxRT){
                if (RawSpectra[Index].Data == null) {
                    ReadMS(Index);
                }
                if ((int)((RawSpectra[IndexRev[Index]].RT/MaxRT)*100) > Progress) {
                    Progress = (int)((RawSpectra[IndexRev[Index]].RT / MaxRT) * 100);
                    RepProgress(Progress);
                }
                if (IndexDir[Index] == -1) break;
                Index = IndexDir[Index];
            }
            while (RawSpectra[Index].RT<TotalRT){
                RawSpectra[Index].Data = null;
                if (IndexDir[Index] == -1) break;
                Index = IndexDir[Index];
            }
            LowRT = MinRT;
            HighRT = MaxRT;
        }


        public void ReadMS(int Scan){
	        int ArraySize = 0;
            Object MassList = null, EmptyRef=null;
            double temp=0.0;

            try {
                if(StickMode && Scan > 0 ){
                    (RawFile as IXRawfile2).GetLabelData(ref MassList, ref EmptyRef, ref  Scan);
                    ArraySize = (MassList as Array).GetLength(1); 
                    RawSpectra[Scan].Data = new MZData[ArraySize];
                    for (int k = 0 ; k<ArraySize ; k++ ){
                        RawSpectra[Scan].Data[k].Mass = (double)(MassList as Array).GetValue(0, k);
                        RawSpectra[Scan].Data[k].Intensity = (double)(MassList as Array).GetValue(1, k);
                    }
                    return;
                }else{
	                RawFile.GetMassListFromScanNum(ref Scan, null, 
		                0, //type
                        0, //value
                        0, //peaks
                        0, //centeroid
                        ref temp,
		                ref MassList, 
                        ref EmptyRef, 
                        ref ArraySize);
                }
            }
            catch{
                Exception e = new Exception(string.Format("Scan #{0} cannot be loaded, probably RAW file is corrupted!",Scan));
                throw e;
            }
			
            //RawSpectra[Scan].Data = new MZData[ArraySize];

            for ( int j = 0 ; j<ArraySize ; j++){
                Buf[j].Mass = (double)(MassList as Array).GetValue(0,j);
                Buf[j].Intensity =  (double)(MassList as Array).GetValue(1,j);
            }

            MassList = null;
            GC.Collect(2);

            //if (Settings.Default.Centroids){
            //    RawSpectra[Scan].Data = PeakDetect(Buf);
            //}else{
            //    RawSpectra[Scan].Data = Centroid(Buf, ArraySize);
            //}
            int isCentroided = 0;

            RawFile.IsCentroidScanForScanNum(Scan,ref isCentroided);

            RawSpectra[Scan].Data = Centroid(Buf, ArraySize, isCentroided != 0);
        }

/*        public MZData[] PeakDetect(MZData[] Data ){

            PeakDetecting.PeakDetector pd = new PeakDetecting.PeakDetector();
            PeakDetecting.peakinside[] Peaks = new PeakDetecting.peakinside[1];
            pd.PeaksDetecting(ref Data, ref Peaks);
	        MZData[] OutData = new MZData[Peaks.GetLength(0)];
            for (int i = 0 ; i < Peaks.GetLength(0) ; i++){
                OutData[i].Intensity = Peaks[i].Value;
                OutData[i].Mass = Peaks[i].Center;
            }
            return OutData;
        }*/

        public MZData[] Centroid(MZData[] Data,int Len, bool StickMode /* former "in" */)
        {
	        int total = 0, u;
	        int o = 0, i = 0, count = Len;
	        double sumIi, sumI, last = 0.0;
            double du = 0.0;
	        bool goingdown = false;
            MZData[] OutData;

            if (StickMode) {
                //считаем пока не начнутся нули или пока следующий не станет меньше помассе 
                for ( i = 1 ; i<count ; i++){
                    if (Data[i].Mass < Data[i-1].Mass || Data[i].Mass == 0){
                        break;
                    }
                }
                OutData = new MZData[i];
                count = i;
                for (i=0; i<count ; i++){
                    OutData[i].Intensity = Data[i].Intensity;
                    OutData[i].Mass = Data[i].Mass;
                }
                return OutData;
            }


            //пропуск начальных нулей
	        while(i < count && Data[i].Intensity == 0.0) ++i;

	        //считает области больше нуля 
	        while(i < count)
	        {
		        while(i < count && Data[i].Intensity != 0.0)
		        {
			        if(last > Data[i].Intensity) {
                        goingdown = true;
                    }else{
                        if(goingdown) {
				            ++total;
				            goingdown = false;
    			        }
                    }

			        last = Data[i].Intensity;
			        ++i;
		        }

		        last = 0.0;
		        goingdown = false;

		        while(i < count && Data[i].Intensity == 0.0) 
                    i++;

		        total++;
	        }

	        //запасает память на подсчитанные области 
	        OutData = new MZData[total];
	        i = 0; o = 0; total = 0; last = 0.0; goingdown = false;

	        while(i < count && Data[i].Intensity == 0.0) i++;

	        while(i < count)
	        {
		        sumIi = sumI = 0.0;
		        o = i -1;
		        while(i < count && Data[i].Intensity != 0.0){

			        //если пошло на спад
			        if(last > Data[i].Intensity) {
                        goingdown = true;
                    }else{
                        if(goingdown) {
				            u = Convert.ToInt32((sumIi / sumI)/* + 0.5*/);
				            OutData[total].Intensity = sumI;
				            OutData[total].Mass = Data[o+u].Mass;
				            ++total;

				            sumIi = sumI = 0.0;
				            o = i -1;
				            goingdown = false;
    			        }
                    }

			        sumIi += Data[i].Intensity*(i-o);
			        sumI += Data[i].Intensity;

			        last = Data[i].Intensity;
			        i++;
		        }

		        u = Convert.ToInt32((sumIi / sumI) /*+0.5*/ );
                du = sumIi / sumI - (double)u;
		        //интенсивность по интегралу 
		        OutData[total].Intensity = sumI;
		        //сентроид - по апексу 
		        //OutData[total].Mass = Data[o+u].Mass;
                //центроид по центру
                OutData[total].Mass = Data[o+u].Mass*(1-du) + Data[o+u+1].Mass*du;

		        last = 0.0;
		        goingdown = false;

		        while(i < count && Data[i].Intensity == 0.0) 
                    i++;
		        total++;
	        }
            return OutData;
        }



    }
}
