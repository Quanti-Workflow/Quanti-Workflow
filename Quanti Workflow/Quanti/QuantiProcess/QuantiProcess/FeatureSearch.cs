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

namespace QuantiProcess
{
    class FeatureSearch
    {
        public static ILogAndProgress iLog = null;

        RawFileService MSFile;

        public FeatureSearch(RawFileService MSFile){
            this.MSFile = MSFile;
        }

        public void FirstSearch(List<QPeptide> Peptides){

            double RTPercDelta = Program.Processor.RTErrorProc/100.0;
            double RTMinutesDelta = Program.Processor.RTErrorMinutes;
            double rtMAX, rtMIN;
            int Progress = 0; 
            for (int i1 = 0 ; i1 < Peptides.Count ; i1++ ){
                QPeptide data = Peptides[i1];
                if ((int)(((double)i1/(double)Peptides.Count)*100.0) > Progress) {
                    Progress = (int)(((double)i1/(double)Peptides.Count)*100.0);
                    iLog.RepProgress(Progress);
                }
                if (data.Sequence != null){
                    data.TheorIsotopeRatio = AveragineIsotopicScore((data.MascotMZ-1)*data.Charge);
                    //data.TheorIsotopeRatio = IsotopicScore(data.Sequence);
                }else{
                    data.TheorIsotopeRatio = AveragineIsotopicScore((data.MascotMZ-1)*data.Charge);
                }
                rtMAX = data.MascotRT*(1+RTPercDelta)+RTMinutesDelta; 
                rtMIN = data.MascotRT*(1-RTPercDelta)-RTMinutesDelta; 
                data.Match =  MSFile.FindBestScore(data.MascotMZ,data.Charge,data.TheorIsotopeRatio,rtMIN,rtMAX);
            }
        }

        public void AlignOutLayers(List<QPeptide> Peptides, int Step, double Quota){
            Peptides.Sort(new QPeptide.byApexRT());
            //оценить - кто не на месте
            //имеет в виду - целиком загруженный файл
            double rtMAX, rtMIN;
            QPeptide data;
            int i1,Count;
            for (i1 = 0 ; i1 < Peptides.Count ; i1++){
                data = Peptides[i1];
                data.RTNumber = 0;
                for (int j = 0 ; j < Peptides.Count ; j++){
                    if (Peptides[j].Match == null) continue;
                    if ((Peptides[j].MascotScan > data.MascotScan && j < i1  /*Matches[MatchIndex].ApexRT != 0.0 && Math.Abs(j-i1)<15*/ )||
                        (Peptides[j].MascotScan < data.MascotScan && j > i1  /*Matches[MatchIndex].ApexRT != 0.0 && Math.Abs(j-i1)<15*/ )){
                        data.RTNumber += Math.Abs(Peptides[j].MascotScan-data.MascotScan);
                    }
                }
            }

            //исключеаем с данные превышающие локальную медиану в окрестности +/-10 спектров
            for (i1 = 0 ; i1 < Peptides.Count ; i1++){
                Count = 0; 
                data = Peptides[i1];
                if (data.AlreadySearched) continue;
                for (int j = i1-Step ; j < i1 + Step ; j++){
                    if (j>=0 && j<Peptides.Count){
                        if (data.RTNumber > Peptides[j].RTNumber){
                            Count++;
                        }
                    }
                }
                if ((double)(Count)/(double)(Step*2) > 1-Quota ){
                    data.Match= null;
                }
            }

            //if (Quota == 0.25) return;

            Peptides.Sort(new QPeptide.byMascotSN());

            double[] ApexRTs = new double[Peptides.Count];
            for (i1 = 0 ; i1 < Peptides.Count ; i1++ ){
                if (Peptides[i1].Match==null){
                    ApexRTs[i1] = 0.0;
                }else{
                    ApexRTs[i1] = Peptides[i1].Match.ApexRT;
                }
            }

            for (i1 = 0 ; i1 < Peptides.Count ; i1++ ){
                data = Peptides[i1];
                if ((data.Match != null) || data.AlreadySearched)  continue;
                iLog.RepProgress((int)(((double)i1/(double)Peptides.Count)*100.0));
                rtMIN = 1000000.0;
                rtMAX = 0.0;
                Count = 0;
                int MinCount;
                for (MinCount = i1 ; Count < Step ; MinCount--){
                    if (MinCount == 0) break;
                    if (ApexRTs[MinCount]!=0.0) Count++;
                }
                int MaxCount;
                Count = 0;
                for (MaxCount = i1 ; Count < Step ; MaxCount++){
                    if (MaxCount == Peptides.Count-1) break;
                    if (ApexRTs[MaxCount]!=0.0) Count++;
                }

                if (MinCount == 0) rtMIN = 0.0;
                if (MaxCount == Peptides.Count-1) rtMAX = 10000.0;

                for ( int j = MinCount ; j <= MaxCount ; j++){
                    if (ApexRTs[j] != 0.0){
                        if (ApexRTs[j] > rtMAX ){
                            rtMAX = ApexRTs[j];
                        }
                        if (ApexRTs[j] < rtMIN  ){
                            rtMIN = ApexRTs[j];
                        }
                    }
                }

                data.MinSearch = rtMIN;
                data.MaxSearch = rtMAX;

                data.Match = MSFile.FindBestScore(data.MascotMZ,data.Charge,data.TheorIsotopeRatio,rtMIN,rtMAX);

            }
        }

        public static double IsotopicScore(string Seq){
            double score = 0.0;
            for (int i = 0 ; i<Seq.Length ; i++){
                switch (Seq[i]){
                    case 'A': score+=0.037096769; break;
                    case 'C': score+=0.071153682; break;
                    case 'D': score+=0.048674349; break;
                    case 'E': score+=0.059720103; break;
                    case 'F': score+=0.102451192; break;
                    case 'G': score+=0.026051014; break;
                    case 'H': score+=0.077161165; break;
                    case 'I': score+=0.070234033; break;
                    case 'K': score+=0.074042639; break;
                    case 'L': score+=0.070234033; break;
                    case 'M': score+=0.067194178; break;
                    case 'N': score+=0.052102029; break;
                    case 'P': score+=0.058958252; break;
                    case 'Q': score+=0.063147783; break;
                    case 'R': score+=0.081429824; break;
                    case 'S': score+=0.037477695; break;
                    case 'T': score+=0.048523449; break;
                    case 'V': score+=0.059188279; break;
                    case 'W': score+=0.127891254; break;
                    case 'Y': score+=0.102832117; break;
                    default: break;
                }
            }
            return (float) score;
        }

        public static double AveragineIsotopicScore(double Mass){
            return Mass*0.00052945;
        }


    }
}
