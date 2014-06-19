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

namespace Fragmentation {
    /// <summary>
    /// Class for peptide features and fragmnets
    /// </summary>
    public class Peptide{
        //Mass constants
        const double H = 1.00782503;
        const double OH = 17.00273963;
        const double NH2 = 16.01872406;
        const double CO = 27.99491462;
        const double Pr = 1.007276467;
        const double C13Shift = 1.003354838;
        const double C = 12.0;
        const double N = 14.003074005;
        const double O = 15.9949146196;

        const int AANumber = 20;


        static double[] AminoAcidExactMass = {
            57.02146371, 71.03711377, 87.03202839, 97.05276383, 99.06841389, 101.04767845, 103.00918446,
            113.08406395, 113.08406395, 114.04292742, 115.02694301, 128.05857748, 128.09496298, 129.04259307, 131.04048458,
            137.05891183, 147.06841389, 156.10111098, 163.06332851, 186.07931292 };

        static char[] AminoAcidsNames = {
            'G','A','S','P','V','T','C',
            'L','I','N','D','Q','K','E','M',
            'H','F','R','Y','W'};
        /// <summary>
        /// Elemental composition of aminoacid residues
        /// </summary>
        static string[] ElemComps = { "C02H03N1O1S0","C03H05N1O1S0","C03H05N1O2S0","C05H07N1O1S0","C05H09N1O1S0","C04H07N1O2S0","C03H05N1O1S1",
            "C06H11N1O1S0","C06H11N1O1S0","C04H06N2O2S0","C04H05N1O3S0","C05H08N2O2S0","C06H12N2O1S0","C05H07N1O3S0","C05H09N1O1S1",
            "C06H07N3O1S0","C09H09N1O1S0","C06H12N4O1S0","C09H09N1O2S0","C11H10N2O1S0"
        };

        //конструктор 
        public string Sequence;

        /// <summary>
        /// Peptide constructor
        /// </summary>
        /// <param name="Seq">Aminoacid sequence of peptide (1-lettere code)</param>
        public Peptide(string Seq){
            Parsed = new ParsedSequence[Seq.Length];
            Sequence = Seq;
            for ( int i = 0 ; i < Seq.Length ; i++){
                Parsed[i].AminoAcid = Convert.ToString(Seq[i]);
                Parsed[i].ModifMass = 0.0;
            }
            ParseSeq();
        }

        /// <summary>
        /// Calculates summed mass of aminoacid residues
        /// </summary>
        /// <param name="Seq">Aminoacid sequence</param>
        /// <returns>summed mass of aminoacid residues</returns>
        public static double CalcMass(string Seq){
            int Count=0;
            double Mass = 0.0;
            char Residue;
            while (Count < Seq.Length){
                Residue = Seq[Count];
                for (int j = 0 ; j < AANumber ; j++){
                    if (Residue == AminoAcidsNames[j]){
                        Mass += AminoAcidExactMass[j];
                        break;
                    }
                }
                Count++;
            }
            return Mass;
        }

        /// <summary>
        /// Calculates elementary composition of peptide
        /// </summary>
        /// <param name="Sequence">Aminoacid sequence</param>
        /// <returns>String, described elements in format like: C#1#H#1#N#1#O#1#S#1 </returns>
        public static string CalcElemComp(string Sequence) {
            int C = 0, H = 2, N = 0, O = 1, S = 0; //water
            char Residue;
            for (int i = 0; i < Sequence.Length; i++) {
                Residue = Sequence[i];
                for (int j = 0; j < AANumber; j++) {
                    if (Residue == AminoAcidsNames[j]) {
                        C += Convert.ToInt32(ElemComps[j].Substring(1, 2));
                        H += Convert.ToInt32(ElemComps[j].Substring(4, 2));
                        N += Convert.ToInt32(ElemComps[j].Substring(7, 1));
                        O += Convert.ToInt32(ElemComps[j].Substring(9, 1));
                        S += Convert.ToInt32(ElemComps[j].Substring(11, 1));
                        break;
                    }
                }
            }
            return String.Format("C#{0}#H#{1}#N#{2}#O#{3}#S#{4}",C,H,N,O,S);
        }


        struct ParsedSequence {
            public string AminoAcid;
            public double ModifMass;
            public double BMass;
            public double YMass;
        }

        ParsedSequence[] Parsed; 

        /// <summary>
        /// Calculates masses for b- and y- ions 
        /// </summary>
        public void ParseSeq(){
            string BEnd,YEnd;
            for (int i = 0 ; i<Sequence.Length ; i++ )	{
                BEnd = Sequence.Substring(0,i+1);
                YEnd = Sequence.Substring(i+1);
                Parsed[i].BMass = CalcMass(BEnd)+Pr;
                Parsed[i].YMass = CalcMass(YEnd)+OH+H+Pr;
                //собираем массы модификаций 
                for (int j = 0 ; j < Sequence.Length ; j++ ){
                    if (j<=i){
                        Parsed[i].BMass += Parsed[j].ModifMass;
                    }else{
                        Parsed[i].YMass += Parsed[j].ModifMass;
                    }
                }
            }
        }

        /// <summary>
        /// Set modofications on peptide sequence 
        /// </summary>
        /// <param name="Position">Modified aminoacid</param>
        /// <param name="Mass">Mass of modification</param>
        public void SetMod(int Position, double Mass){
            Parsed[Position].ModifMass = Mass;
            ParseSeq();
        }
        /// <summary>
        /// Gets mass of main fragments
        /// </summary>
        /// <param name="Kind">a|b|c|x|y|z</param>
        /// <param name="Number">Index of fragment</param>
        /// <returns></returns>
        public double GetFragment(string Kind, int Number){
            switch  (Kind) {
                case "a":
                    return Parsed[Number-1].BMass-CO;
                case "b":
                    return Parsed[Number-1].BMass;
                case "c":
                    return Parsed[Number-1].BMass+NH2;
                case "z":
                    return Parsed[Sequence.Length-Number-1].YMass-NH2;
                case "y":
                    return Parsed[Sequence.Length-Number-1].YMass;
                case "x":
                    return Parsed[Sequence.Length-Number-1].YMass+CO;
                default:
                    return 0.0;
            }
        }
    }
}
