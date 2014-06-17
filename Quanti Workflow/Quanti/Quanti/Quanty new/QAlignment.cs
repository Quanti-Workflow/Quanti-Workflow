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
    public class QAlignment
    {
        public int Left;
        public int Right;
        public List<PeptidePair> Peptides;
        public List<ProteinPair> Proteins;
        public double NormFactor = 1.0;

        public QAlignment(List<QPeptide> Peptides, List<QProtein> Proteins,int LeftIndex,int RightIndex){
            Left = LeftIndex;
            Right = RightIndex;
            this.Peptides = new List<PeptidePair>();
            for ( int i = 0 ; i < Peptides.Count ; i++){
                this.Peptides.Add(new PeptidePair(Peptides[i],Left,Right));
            }
            this.Proteins = new List<ProteinPair>();
            int j = 0;
            for ( int i = 0 ; i < Proteins.Count ; i++){
                ProteinPair PPair = new ProteinPair();
                PPair.Protein = Proteins[i];
                while (Proteins[i].ipi == Peptides[j].IPI){
                    if (Proteins[i] == this.Peptides[j].Peptide.PartOfProtein && this.Peptides[j].Exists) {
                        PPair.Peptides.Add(this.Peptides[j]);
                    }
                    j++;
                    if (j >= Peptides.Count) break;
                }
                this.Proteins.Add(PPair);
            }
        }

        public void Run(){
            for (int i=0 ; i < Peptides.Count ; i++){

                if (Settings.Default.CutTails){
                    Peptides[i].LeftMatch.CutTails(Peptides[i].RightMatch);
                }

                //Раньше это было настройкой 
                double DispThres = 2;
                //double DispThres = GetStringValue(Settings.Default.DispThres);
                if ( Settings.Default.ShapeFilter){

                    if (Peptides[i].Exists && Peptides[i].LeftMatch.RTDisp != 0.0 && Peptides[i].RightMatch.RTDisp != 0.0){
                        double Ratio = Math.Max(Peptides[i].LeftMatch.RTDisp/Peptides[i].RightMatch.RTDisp,
                            Peptides[i].RightMatch.RTDisp/Peptides[i].LeftMatch.RTDisp);
                        if (Ratio > DispThres){
                            Peptides[i].LeftMatch = null;
                        }
                    }else{
                        Peptides[i].LeftMatch = null;
                    }
                }
            }

            //нормализация
            double Sum0 = 0.0, Sum1 = 0.0;
            for (int i=0 ; i < Peptides.Count ; i++){
                if (!Peptides[i].Exists) continue;
                Sum0+=Peptides[i].RightMatch.Score;
                Sum1+=Peptides[i].LeftMatch.Score;
            }
            NormFactor = Sum1/Sum0;

            //Log(String.Format("Intensity alignment - horisontal {0} versus {1}",Index,RefNumber));

            if (Settings.Default.Norm){
                for (int i=0 ; i < Peptides.Count ; i++){
                    if (!Peptides[i].Exists) continue;
                    Peptides[i].LeftMatch.Score = Peptides[i].LeftMatch.Score / NormFactor;
                }
            }

            double Interval = Utils.GetStringValue(Settings.Default.ESIInterval);

            if (Interval>0.0){
                LongESIAlignment(Interval);
            }

            //распихиваем Ratios
            for (int i = 0 ; i <  Peptides.Count ; i++){
                if (Peptides[i].Exists){
                    Peptides[i].Ratio = Peptides[i].LeftMatch.Score / Peptides[i].RightMatch.Score;
                }
            }

            //выработка статистики по белкам 
            for ( int i1 = 0 ; i1 < Proteins.Count ; i1++ ){
                Proteins[i1].CleanUp();
                Proteins[i1].MakeStats();
            }
        }

        class RatioScores{
            public double Ratio;
            public double Score;
            public double RT;
            public int Index;
            public RatioScores(double inRatio, double InScore, double inRT, int inIndex ){
                Ratio = inRatio;
                Score = InScore;
                RT = inRT;
                Index = inIndex;
            }
            public class byRat:IComparer<RatioScores>  {
                public int Compare(RatioScores x, RatioScores y){
                    if (x.Ratio>y.Ratio) { return 1;} 
                    else if (x.Ratio == y.Ratio) {
                        if (x.Index > y.Index) return 1;
                        if (x.Index < y.Index) return -1;
                        return 0; }
                    else return -1;
                }
            }
        }

        void LongESIAlignment(double Interval){

            Peptides.Sort(new PeptidePair.byApexRT());
            SortedList<RatioScores,RatioScores> Ratios = 
                new SortedList<RatioScores,RatioScores>(new RatioScores.byRat());

            int LowerIndex = 0;
            int OldLowerIndex = 0;
            int HigherIndex = 0;
            int OldHigherIndex = 0;
            double LowerRT = 0.0;
            double UpperRT = 0.0;
            for (int i=0 ; i < Peptides.Count ; i++){
                if (!Peptides[i].Exists) continue;
                //интервал для исключения из списка
                LowerRT = Peptides[i].LeftMatch.ApexRT - Interval;
                if (LowerRT < 0.0) LowerRT = 0.0;
                for (LowerIndex = OldLowerIndex; ; LowerIndex++){
                    if (LowerIndex >= Peptides.Count) break;
                    if (!Peptides[LowerIndex].Exists) continue;
                    if (Peptides[LowerIndex].LeftMatch.ApexRT > LowerRT) break;
                    Ratios.Remove(new RatioScores((Peptides[LowerIndex].RightMatch.Score / Peptides[LowerIndex].LeftMatch.Score),
                            Peptides[LowerIndex].RightMatch.Score,
                            Peptides[LowerIndex].RightMatch.ApexRT,
                            LowerIndex));
                }
                OldLowerIndex = LowerIndex; 
                //интервал для включения в список
                UpperRT = Peptides[i].LeftMatch.ApexRT + Interval;
                for (HigherIndex = OldHigherIndex; ; HigherIndex++){
                    if (HigherIndex >= Peptides.Count) break;
                    if (!Peptides[HigherIndex].Exists) continue;
                    if (Peptides[HigherIndex].LeftMatch.ApexRT > UpperRT) break;
                    RatioScores NewRatio = new RatioScores((Peptides[HigherIndex].RightMatch.Score / Peptides[HigherIndex].LeftMatch.Score),
                            Peptides[HigherIndex].RightMatch.Score,
                            Peptides[HigherIndex].RightMatch.ApexRT,
                            HigherIndex);
                    Ratios.Add(NewRatio,NewRatio);
                }
                OldHigherIndex = HigherIndex;
                if (Ratios.Count>3){
                    if (Ratios.Count % 2 != 0) {
                        Peptides[i].ESIFactor = Ratios.Values[Ratios.Count / 2].Ratio;
                    } else {
                        Peptides[i].ESIFactor = Math.Sqrt(Ratios.Values[Ratios.Count / 2 - 1].Ratio*Ratios.Values[Ratios.Count / 2].Ratio);
                    }
                } else {
                    Peptides[i].ESIFactor=0.0;
                }
            }
            //если не удалось найти подобающее - ищем ближайшие для которых удалось
            for (int i=0 ; i < Peptides.Count ; i++){
                if (!Peptides[i].Exists) continue;
                if (Peptides[i].ESIFactor == 0.0){
                    double Pred =0.0;
                    for (int j = i-1 ; j>=0  ;j-- ){
                        if (!Peptides[j].Exists) continue;
                        if (Peptides[j].ESIFactor  != 0.0){
                            Pred = Peptides[j].ESIFactor;
                            break;
                        }
                    }
                    double Post =0.0;
                    for (int j = i+1 ; j<Peptides.Count ; j++){
                        if (!Peptides[j].Exists) continue;
                        if (Peptides[j].ESIFactor  != 0.0){
                            Post = Peptides[j].ESIFactor;
                            break;
                        }
                    }
                    if (Post == 0.0) Post = Pred;
                    if (Pred == 0.0) Pred = Post;
                    Peptides[i].ESIFactor = (Post+Pred)/2.0;
                }
            }

            //накладываем изменения на Score

            for (int i=0 ; i < Peptides.Count ; i++){
                if (!Peptides[i].Exists) continue;
                Peptides[i].LeftMatch.Score *=  Peptides[i].ESIFactor;
            }

            //sw.Close();
        }


    }
}
