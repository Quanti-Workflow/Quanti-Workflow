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

    public partial class QProtein : IPrinting{
        public string ipi;         //итоговый идентификатор белка               - pers - pr key
        public List<string> ipis;  //исходный набор идентификаторов белка       - pers
        public List<QPeptide> Peptides; //набор идентифицированных пептидов   - связь по ключу 
        public string Name; //имя белка (строка с идентификатором)              - pers
        public String Desc; //описание белка                                    - pers
        public double Est; //оценка белка                                       - no pers
        public double AveScore; //Reference Abundance

        public static QAligner Aligner;

        public class byIPI : IComparer<QProtein> {
            public int Compare(QProtein x, QProtein y){
                if (string.Compare(x.ipi,y.ipi)<0) { return -1;} 
                if (string.Compare(x.ipi,y.ipi)>0) { return 1;} 
                return 0;
            }
        }

        public void EstimateAveScore(){
            double SumScore;
            AveScore = 0.0;
            int Count = 0;
            bool flag;

            for ( int i = 0 ; i < FileList.Count ; i++){
                flag = false;
                SumScore = 0.0;
                for (int k = 0 ; k < Peptides.Count ; k++){
                    if (Peptides[k].Matches[i] != null && 
                        Peptides[k].Matches[i].Score > 0.0){
                        SumScore += Peptides[k].Matches[i].Score;
                        flag = true;
                    }
                }
                if (flag) { 
                    Count++;
                    AveScore += Math.Log(SumScore);
                }
            }
            AveScore = AveScore / Count;
            AveScore = Math.Exp(AveScore);
        }

        static double[] FileFactors;

        public static void FileFactorsCalc(List<QProtein> Proteins){
            int FileCount = FileList.Count;
            FileFactors = new double[FileCount];
            for ( int i = 0 ; i < Proteins.Count ; i++){
                if (Aligner is MatrixAligner){
                    Proteins[i].EstimateAveScore();
                    double[] Res = (Aligner as MatrixAligner).GetProtRatios(Proteins[i], false);
                    for ( int j = 0 ; j < FileCount ; j++){
                        FileFactors[j] += Proteins[i].AveScore * Res[j];
                    }
                }
                if (Aligner is RefAligner || Aligner == null){
                    for ( int j = 0 ; j < FileList.Count ; j++){
                        for (int k = 0 ; k < Proteins[i].Peptides.Count ; k++){
                            if (Proteins[i].Peptides[k].Matches[j] != null && 
                                Proteins[i].Peptides[k].Matches[j].Score > 0.0){
                                FileFactors[j] += Proteins[i].Peptides[k].Matches[j].Score;
                            }
                        }
                    }
                }
            }
        }
    }
}
