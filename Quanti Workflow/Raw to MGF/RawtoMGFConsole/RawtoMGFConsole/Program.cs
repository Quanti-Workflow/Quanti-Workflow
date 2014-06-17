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
using System.IO;
using System.Threading;
using MSFileReaderLib;
using MGFParser;
using RawMSBox;

namespace RawtoMGFConsole
{
    class Program
    {
        static RawFileBox MSFileBox = null; //for ms-only data
        static int MSCount = 0;

        public static void RepProgress(int Perc){
            /*int Count = 0;
            for (int i = 0 ; i < MSFileBox.RawSpectra.GetLength(0) ; i++){
                if (MSFileBox.RawSpectra[i].Data != null){
                    Count++;
                }
            }
            Console.WriteLine("{0}%... {1} 0",Perc,Count);*/
        }

        static double RTError = 1.0;
        static double MZError = 10; //ppm

        static int Main(string[] args){
            //Параметры соответствуют вводу в форму RawtoMGF
            //args[0] - имя Raw-файла
            //args[1] - имя MGF-файла
            //args[2] - minimal MZ
            //args[3] - maximal MZ
            //args[4] - minimal RT
            //args[5] - maximal RT
            //args[6] - minimal Charge
            //args[7] - maximal Charge
            //args[8] - number of top peaks 
            //args[9] - CleanETD:yes / CleanETD:no
            //args[10] - Instrument:yes / Instrument:no
            //дополнительно (к версии 2.0.6)
            //args[11] - CheckSpectra:yes / CheckSpectra:no
            try{
                Console.ReadLine();
                //Thread.Sleep(30000);
                string InFileName = args[0];
                string OutFileName = args[1];
                double MinMZ = Convert.ToDouble(args[2]);
                double MaxMZ = Convert.ToDouble(args[3]);
                double MinRT = Convert.ToDouble(args[4]);
                double MaxRT = Convert.ToDouble(args[5]);
                int MinCharge = Convert.ToInt32(args[6]);
                int MaxCharge = Convert.ToInt32(args[7]);
                int MaxPeakNumber = Convert.ToInt32(args[8]);
                bool CleanETD = args[9].Contains("CleanETD:yes");
                bool MarkInstrtument = args[10].Contains("Instrument:yes");
                bool CheckSpectra = args[11].Contains("CheckSpectra:yes");
                bool RTApex = args[12].Contains("RTApex:yes");

                string[] Refs = {"@cid","@hcd","@etd","@ecd","FTMS","ITMS"};
            
                MSFileReader_XRawfile RawFile;
                int Spectra,LocalCount = 0;
                string Filter;
                MGFFile MGF;
                MGFSpectrum ms;
                Childs ch;
                ChildbyIntensity ci = new ChildbyIntensity();
                ChildbyMass cm  = new ChildbyMass();


	            int ArraySize = 0;
                Object MassList, EmptyRef;
                Object Labels,Values;
                double temp=0.0;
                int Progress = 0;
                int MSCount = 0;
                int MSMSCount = 0;
                
                if (RTApex){
                    MSFileBox = new RawFileBox();
                    MSFileBox.LoadIndex(InFileName);
                    RawFileBox.RepProgress = RepProgress;
                    RawFile = MSFileBox.RawFile;
                }else{
                    RawFile = new MSFileReader_XRawfile();
                    RawFile.Open(InFileName);
                    RawFile.SetCurrentController(0, 1);
                }
                Spectra = 0;
                RawFile.GetNumSpectra(ref Spectra);
                if (Spectra == 0) {
                    throw new Exception("Cannot get spectra from the file "+InFileName+", File is invalid, broken or empty");
                }
                MGF = new MGFFile();
                for (int j=1 ; j<=Spectra ; j++){
                    Filter = null;
                    RawFile.GetFilterForScanNum(j, ref Filter);
                    if (Filter.IndexOf("ms2") == -1) {
                        MSCount++;
                        if (!CheckSpectra){
                            continue;
                        }else{
                            ArraySize = 0;
                            MassList = null;
                            EmptyRef=null;
                            temp=0.0;
                            try {
	                            RawFile.GetMassListFromScanNum(ref j, null, 0, 0, 0, 0, 
                                    ref temp, ref MassList, ref EmptyRef, ref ArraySize);
                                continue;
                            }catch{
                                Exception e = new Exception(string.Format("Scan #{0} cannot be loaded, probably RAW file is corrupted!",j));
                                throw e;
                            }
                        }
                    }
                    MSMSCount++;
                    ms = new MGFSpectrum();
                    //определяем родительскую массу, заряд и RT
                    Labels = null;
                    Values = null;
                    ArraySize = 0;
                    LocalCount++;
                    RawFile.GetTrailerExtraForScanNum(j, ref Labels, ref Values, ref ArraySize);
                    for (int k = 0 ; k < ArraySize ; k++ ){
                        if ((Labels as Array).GetValue(k).ToString().Contains("Mono")){
                            ms.mz = Convert.ToDouble((Values as Array).GetValue(k).ToString());
                        }
                        if ((Labels as Array).GetValue(k).ToString().Contains("Charge State")){
                            ms.Charge = Convert.ToInt32((Values as Array).GetValue(k).ToString());
                        }
                    }
                    //Если не нашли в labels - берем из фильтра
                    if (ms.mz == 0.0) {
                        string  part= Filter.Substring(0,Filter.IndexOf('@'));
                        ms.mz = Convert.ToDouble(part.Substring(part.LastIndexOf(' ')));
                    }
                    Labels = null;
                    Values = null;
                    ArraySize = 0;
                    double RT = 0;
                    RawFile.GetStatusLogForScanNum(j,ref RT, ref Labels, ref Values, ref ArraySize);

                    RawFile.RTFromScanNum(j,ref ms.RT);
                    ms.ScanNumber = j;
                    //Фильтры 
                    if (ms.Charge < MinCharge) continue;
                    if (ms.Charge > MaxCharge) continue;
                    if (ms.RT < MinRT) continue;
                    if (MaxRT != 0.0 && ms.RT > MaxRT) continue;
                    if (ms.mz < MinMZ) continue;
                    if (MaxMZ != 0.0 && ms.mz > MaxMZ) continue;
                    //забираем сам спектр 
                    MassList = null;
                    EmptyRef=null;
                    ArraySize = 0;
                    temp=0.0;
                    try{
                        if (Filter.IndexOf("FTMS") != -1){
                            //извлекаем FTMS данные 
                            (RawFile as IXRawfile2).GetLabelData(ref MassList, ref EmptyRef, ref  j);
                            ArraySize = (MassList as Array).GetLength(1); 
                            for (int k = 0 ; k<ArraySize ; k++ ){
                                ch = new Childs();
                                ch.Mass = (double)(MassList as Array).GetValue(0, k);
                                ch.Intensity = (double)(MassList as Array).GetValue(1, k);
                                ms.Data.Add(ch);
                            }
                        }else{
                            //извлекаем ITMS данные 
                            RawFile.GetMassListFromScanNum(ref j, null, 0, 0, 0, 1, ref temp, ref MassList, ref EmptyRef, ref ArraySize);
                            ArraySize = (MassList as Array).GetLength(1); 
                            for ( int k = 0 ; k<ArraySize ; k++){
                                ch = new Childs();
                                ch.Mass = (double)(MassList as Array).GetValue(0, k);
                                ch.Intensity = (double)(MassList as Array).GetValue(1, k);
                                ms.Data.Add(ch);
                            }
                        }
                    }catch{
                        Exception e = new Exception(string.Format("Scan #{0} cannot be loaded, probably RAW file is corrupted!",j));
                        throw e;
                    }
                    if (RTApex){
                        ms.RTApex = CheckRTApex(ms.RT, ms.mz);
                    }

                    if (MarkInstrtument){
                        if (Filter.Contains("ecd") || Filter.Contains("etd")){
                                ms.Instrument = "ETD-TRAP";
                            }else{
                                ms.Instrument= "ESI-FTICR";
                            }
                        }
                    //очистить ETD если был такой запрос
                    if ( CleanETD ){
                        if (Filter.Contains("ecd") || Filter.Contains("etd")){
                            FilterETD(ref ms,  Filter.Contains("FTMS"));
                        }
                    }
                    //сбросить лишние сигналы
                    if (MaxPeakNumber > 0 && ms.Data.Count > MaxPeakNumber){
                        ms.Data.Sort(ci);
                        ms.Data.RemoveRange(0,ms.Data.Count-MaxPeakNumber);
                        ms.Data.Sort(cm);
                    }
                    //сформировать TITLE
                    if (RTApex){
                        ms.Title = String.Format("Elution from: {0:f4} to {0:f4} RT Apex: {1:f2} FinneganScanNumber: {2}",
                            ms.RT, ms.RTApex, ms.ScanNumber);
                    }else{
                        ms.Title = String.Format("Elution from: {0:f4} to {0:f4} FinneganScanNumber: {1}",
                            ms.RT,ms.ScanNumber);
                    }
                    MGF.Spectra.Add(ms);
                    // рапортовать прогресс
                    GC.Collect(2);

                    if ((int)(((double)j/(double)Spectra)*100.0) > Progress){
                        Progress = (int)(((double)j/(double)Spectra)*100.0);
                        Console.WriteLine("{0}%... {1} {2}",Progress,MSCount,MSMSCount);
                    }

                    //backgroundWorker1.ReportProgress((int)(((double)LocalCount/(double)SpCount)*100.0));
                }
                MGF.MGFComments.Add(String.Format("Created by RawToMGF 2.1.3; Spectra obtained from {0}",InFileName));
                MGF.MGFComments.Add(String.Format("Filters: Parent m/z from {0} Th to {1} Th;",MinMZ,MaxMZ));
                MGF.MGFComments.Add(String.Format("         RT from {0} min. to {1} min.;",MinRT,MaxRT)); 
                MGF.MGFComments.Add(String.Format("         Charge state of parent ions minimum {0}, maximum {1};",MinCharge,MaxCharge)); 
                MGF.MGFComments.Add(String.Format("         Max number of peaks in MS/MS spectra - {0}",MaxPeakNumber)); 
                if ( CleanETD ) {
                    MGF.MGFComments.Add("         ETD spectra cleaned from precursors and neutral losses"); 
                }
                MGF.MGFWrite(OutFileName,true);
            }catch(Exception e){
                Console.Write("Error:");
                Console.Write(e.Message);
                Console.WriteLine("STACKINFO:"+e.StackTrace);
                //Console.ReadKey();
                return 1;
            }
            Console.WriteLine("Completed");
            Console.ReadLine();
            return 0;

        }

        static private void RemoveInterval(ref MGFSpectrum Spectrum, double MinMZ, double MaxMZ){
            for (int i = Spectrum.Data.Count-1 ; i >= 0 ;i--){
                if (Spectrum.Data[i].Mass<MinMZ){
                    break;
                }
                if (Spectrum.Data[i].Mass<MaxMZ){
                    Spectrum.Data.RemoveAt(i);
                    continue;
                }
            }
        }

        public static double H = 1.00782503;
        public static double OH = 17.00273963;
        public static double NH2 = 16.01872406;
        public static double CO = 27.99491462;
        public static double Pr = 1.007276467;
        public static double C13Shift = 1.003354838;
        public static double C = 12.0;
        public static double N = 14.003074005;
        public static double O = 15.9949146196;


        private static void FilterETD(ref MGFSpectrum Spectrum , bool FTMS){
            //precursor area
            double Error;
            if (FTMS) {
                Error = 0.02;
            }else{
                Error = 0.8;
            }
            //precursor area
            //4 Isotops 
            double step =  C13Shift / Spectrum.Charge;
            for (int i = 0 ; i < 4 ; i++){
                RemoveInterval(ref Spectrum, Spectrum.mz-Error+step*i, Spectrum.mz+Error+step*i);
            }
            //Charge reduction precursors + neutral losses 
            for (int j = Spectrum.Charge - 1; j > 0 ; j-- ){
                step = C13Shift / j;
                double mz = (Spectrum.mz * (double)Spectrum.Charge - ((double)(Spectrum.Charge-j) * Pr))/(double)j;
                for (int i = 0 ; i < 4 ; i++){
                    RemoveInterval(ref Spectrum, mz-Error+step*i, mz+Error+step*i);
                    RemoveInterval(ref Spectrum, mz-((NH2+H)/(double)j)-Error+step*(double)i, 
                                                 mz-((NH2+H)/(double)j)+Error+step*(double)i);
                    RemoveInterval(ref Spectrum, mz-((OH+H)/(double)j)-Error+step*(double)i, 
                                                 mz-((OH+H)/(double)j)+Error+step*(double)i);
                    RemoveInterval(ref Spectrum, mz-((CO)/(double)j)-Error+step*(double)i, 
                                                 mz-((CO)/(double)j)+Error+step*(double)i);
                    RemoveInterval(ref Spectrum, mz-((C+H*3.0+OH)/(double)j)-Error+step*(double)i, 
                                                 mz-((C+H*3.0+OH)/(double)j)+Error+step*(double)i);
                    RemoveInterval(ref Spectrum, mz-((N*2.0+H*6.0)/(double)j)-Error+step*(double)i, 
                                                 mz-((N*2.0+H*6.0)/(double)j)+Error+step*(double)i);
                    RemoveInterval(ref Spectrum, mz-((N+H*4.0+O)/(double)j)-Error+step*(double)i, 
                                                 mz-((N+H*4.0+O)/(double)j)+Error+step*(double)i);
                    RemoveInterval(ref Spectrum, mz-((H*4.0+O*2.0)/(double)j)-Error+step*(double)i, 
                                                 mz-((H*4.0+O*2.0)/(double)j)+Error+step*(double)i);
                    RemoveInterval(ref Spectrum, mz-((C+H*4.0+N*2.0)/(double)j)-Error+step*(double)i, 
                                                 mz-((C+H*4.0+N*2.0)/(double)j)+Error+step*(double)i);
                    RemoveInterval(ref Spectrum, mz-((C+H*3.0+N+O)/(double)j)-Error+step*(double)i, 
                                                 mz-((C+H*3.0+N+O)/(double)j)+Error+step*(double)i);
                    RemoveInterval(ref Spectrum, mz-((C+H*2.0+O*2.0)/(double)j)-Error+step*(double)i, 
                                                 mz-((C+H*2.0+O*2.0)/(double)j)+Error+step*(double)i);
                    RemoveInterval(ref Spectrum, mz-((C*2.0+H*6.0+O)/(double)j)-Error+step*(double)i, 
                                                 mz-((C*2.0+H*6.0+O)/(double)j)+Error+step*(double)i);
                    RemoveInterval(ref Spectrum, mz-((C*2.0+H*5.0+N+O)/(double)j)-Error+step*(double)i, 
                                                 mz-((C*2.0+H*5.0+N+O)/(double)j)+Error+step*(double)i);
                    RemoveInterval(ref Spectrum, mz-((C+H*5.0+N*3.0)/(double)j)-Error+step*(double)i, 
                                                 mz-((C+H*5.0+N*3.0)/(double)j)+Error+step*(double)i);
                }

            }
        }

        static double CheckRTApex(double RT, double MZ){
            int Start = 0;
            for (int i = 0 ; i < MSFileBox.Spectra ; i++){
                if ( MSFileBox.RawSpectra[i].RT >= RT ){
                    Start = MSFileBox.IndexRev[i];
                    break;
                }
            }
            double BestIntensity = 0.0;
            int BestIndex = 0;
            //на минуту назад 
            for ( int i = Start ; i>0 && RT - MSFileBox.RawSpectra[i].RT < 1.0 ; i = MSFileBox.IndexRev[i]){
                if (MSFileBox.RawSpectra[i].Data == null){
                    MSFileBox.ReadMS(i);
                }
                MZData CurPeak = MSFileBox.RawSpectra[i].FindBiggestPeak(MZ, 10.0);
                if (CurPeak.Intensity == 0.0) break;
                if (CurPeak.Intensity > BestIntensity) {
                    BestIntensity = CurPeak.Intensity;
                    BestIndex = i;
                }
            }
        
            //на две минуты вперед
            for ( int i = MSFileBox.IndexDir[Start] ; i>=0 && MSFileBox.RawSpectra[i].RT - RT < 2.0 ; i = MSFileBox.IndexDir[i]){
                if (MSFileBox.RawSpectra[i].Data == null){
                    MSFileBox.ReadMS(i);
                }
                MZData CurPeak = MSFileBox.RawSpectra[i].FindBiggestPeak(MZ, 10.0);
                if (CurPeak.Intensity == 0.0) break;
                if (CurPeak.Intensity > BestIntensity) {
                    BestIntensity = CurPeak.Intensity;
                    BestIndex = i;
                }
            }

            if (BestIntensity == 0.0) return 0.0;
            return MSFileBox.RawSpectra[BestIndex].RT;
        }

    }
}
