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

namespace QuantiProcess{
    public class QPeptide {

        //базовый пептид
        public double MascotMZ; // масса полученная из файла Маскот                             - pers
        public double MascotRT; // RT полученное из файла Маскот                                - pers
        public double MascotScore;  // Mascot Score                                             - pers
        public double TheorIsotopeRatio; // теоретическое отношение первого и второго изотопов  - pers
        public int MascotScan;  //Finegan Scan Number полученный из Маскота -                   - pers
                                //После ClusterMGF соответствует порядку пептида в выборке
        public int RTNumber;    //номер в соотвествии с RT в данном эксперименте                - no pers
        public int Charge;      //зарядовое состояние пептида                                   - pers
        public string IPI;         //итоговый идентификатор белка                               - pers
        public List<string> IPIs;  //исходный набор идентификаторов белка                       - pers
        public string Sequence; //аминокислотная последовательность                             - pers
        public int peptides;    //количество пептидов в белке                                   - pers    
        public double ModMass;  //полная масса модификаций                                      - pers
        public string ModDesk;  //описане модификаций - текстом                                 - pers
        public string Case;     //описание - к какой группе относится данный файл               - pers

        public double MinSearch;
        public double MaxSearch;

        public double MatrixScore;

        public QMatch Match; //набор неизменных сборок из                                   


        public bool AlreadySearched;

        public QPeptide(){
            Match = new QMatch();
            AlreadySearched = false;
        }
        public class byMascotSN : IComparer<QPeptide> {
            public int Compare(QPeptide x, QPeptide y){
                if (x.MascotScan>y.MascotScan) { return 1;} 
                else if (x.MascotScan == y.MascotScan) { return 0; }
                else return -1;
            }
        }

        public class byApexRT : IComparer<QPeptide> {
            public int Compare(QPeptide x, QPeptide y){
                //if (string.Compare(x.Sequence,y.Sequence)>0) { return -1;} 
                //if (string.Compare(x.Sequence,y.Sequence)<0) { return 1;} 
                if (x.Match == null && y.Match == null ) return 0;
                if (x.Match == null ) return -1;
                if (y.Match == null ) return 1;
                if (x.Match.ApexRT>y.Match.ApexRT) { return 1;} 
                else if (x.Match.ApexRT == y.Match.ApexRT) { 
                    if (x.Match.Score > y.Match.Score) {return 1;}
                    else if (x.Match.Score == y.Match.Score) {return 0;}
                    return -1; 
                }
                else return -1;
            }
        }

    };
}