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
            for ( int i = 0 ; i < Proteins.Count ; i++){
                ProteinPair PPair = new ProteinPair();
                PPair.Protein = Proteins[i];
                for (int j = 0 ; j < Peptides.Count ; j++){
                    if (Proteins[i].Peptides.Contains(this.Peptides[j].Peptide) && this.Peptides[j].Exists) {
                        PPair.Peptides.Add(this.Peptides[j]);
                    }
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
                Sum0+=Peptides[i].LeftMatch.Score;
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
            public RatioScores(double inRatio, double InScore, double inRT){
                Ratio = inRatio;
                Score = InScore;
                RT = inRT;
            }
            public class byRat:IComparer<RatioScores>  {
                public int Compare(RatioScores x, RatioScores y){
                    if (x.Ratio>y.Ratio) { return 1;} 
                    else if (x.Ratio == y.Ratio) { return 0; }
                    else return -1;
                }
            }
        }

        void LongESIAlignment(double Interval){

            Peptides.Sort(new PeptidePair.byApexRT());

            for (int i=0 ; i < Peptides.Count ; i++){
                if (!Peptides[i].Exists) continue;
                //ищем медиану от Ratio
                List<RatioScores> Ratios = new List<RatioScores>();
                for (int j = 0 ; j<Peptides.Count ; j++){
                    if (!Peptides[j].Exists) continue;
                    if (Peptides[j].Peptide.BandID != Peptides[i].Peptide.BandID) continue;
                    if (Math.Abs(Peptides[j].LeftMatch.ApexRT - Peptides[i].LeftMatch.ApexRT)>Interval)
                        continue;

                    Ratios.Add(new RatioScores(Peptides[j].RightMatch.Score/Peptides[j].LeftMatch.Score,
                        Peptides[j].RightMatch.Score,
                        Peptides[j].RightMatch.ApexRT));
                }
                if (Ratios.Count>3){
                    Ratios.Sort(new RatioScores.byRat());
                    if (Ratios.Count % 2 != 0) {
                        Peptides[i].ESIFactor = Ratios[Ratios.Count / 2].Ratio;
                    } else {
                        Peptides[i].ESIFactor = (Ratios[Ratios.Count / 2 - 1].Ratio + Ratios[Ratios.Count / 2].Ratio) / 2;
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
