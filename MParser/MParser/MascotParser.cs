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
using System.IO;

namespace Mascot{

    public class Utils{
        /// <summary>
        /// Compare sequence string with respect to I/L equality for mass specs
        /// </summary>
        /// <param name="Sequence1">Aminoacid string 1</param>
        /// <param name="Sequence2">Aminoacid string 2</param>
        /// <returns></returns>
        public static bool MSEqual(string Sequence1, string Sequence2) {
            if (Sequence1.Length != Sequence2.Length) return false;
            for (int i=0; i<Sequence1.Length ; i++ ){
                if (Sequence1[i]!=Sequence2[i]){
                    if (Sequence1[i] == 'I' && Sequence2[i] == 'L') continue;
                    if (Sequence1[i] == 'L' && Sequence2[i] == 'I') continue;
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Calculates the abundance of second isotope peak
        /// </summary>
        /// <param name="Seq">Aminoacid sequence</param>
        /// <returns>Second Isotope peak abundance</returns>
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
        /// <summary>
        /// Calculates second isotope peak relative abundance for peptide average C13 contribution
        /// </summary>
        /// <param name="Mass">Mass</param>
        /// <returns>Second isotope peak relative abundance</returns>
        public static double AveragineIsotopicScore(double Mass){
            return Mass*0.00052945;
        }
    }

    public class MascotSpectrabyScore : IComparer<Mascot.MascotSpectra> {
        public int Compare(Mascot.MascotSpectra x, Mascot.MascotSpectra y) {
            if (x.Peptides.Count == 0 && y.Peptides.Count == 0) { return 0; }
            if (x.Peptides.Count == 0) { return 1; }
            if (y.Peptides.Count == 0) { return -1; }
            if (x.Peptides[0].Score < y.Peptides[0].Score) { return 1; }
            if (x.Peptides[0].Score > y.Peptides[0].Score) { return -1; }
            if (x.Peptides[0].Score == y.Peptides[0].Score) { return 0; }
            return 0;
        }
    }
    /// <summary>
    /// PSM 
    /// </summary>
    public class Peptide{
        /// <summary>
        /// Aminoacid sequence
        /// </summary>
        public string Sequence;
        /// <summary>
        /// Mascot score
        /// </summary>
        public double Score;
        /// <summary>
        /// Calculated mass
        /// </summary>
        public double Mass;
        /// <summary>
        /// Delta of experimental to calculated mass
        /// </summary>
        public double Delta;
        /// <summary>
        /// List of porotein names
        /// </summary>
        public string[] ProteinNames;
        /// <summary>
        /// Start position for each protein
        /// </summary>
        public int[] StartPositons;
        /// <summary>
        /// End position for each protein
        /// </summary>
        public int[] EndPositons;
        /// <summary>
        /// Modification index for each aminoacid
        /// </summary>
        public int[] ModIndex;
        /// <summary>
        /// Modification index for N-termini
        /// </summary>
        public int NModIndex;
        /// <summary>
        /// Modification index for C-termini
        /// </summary>
        public int CModIndex;
    }
    
    public class MascotSpectra{
        /// <summary>
        /// Mass-to-charge ratio
        /// </summary>
        public double mz;
        /// <summary>
        /// Ion charge
        /// </summary>
        public int Charge;
        /// <summary>
        /// Pure mass calculated by mascot
        /// </summary>
        public double Mass;
        /// <summary>
        /// Intensity as provided by mascot
        /// </summary>
        public double Intensity;
        /// <summary>
        /// Finnegan scan number (if present)
        /// </summary>
        public int ScanNumber;
        /// <summary>
        /// Retention time of MS2 event
        /// </summary>
        public double RT;
        /// <summary>
        /// Retention time for maximum of corresponded MS-only signal
        /// </summary>
        public double RTApex;
        /// <summary>
        /// Title string (if exsists)
        /// </summary>
        public string Title;
        /// <summary>
        /// List of PSMs
        /// </summary>
        public List<Peptide> Peptides;

        //Spectra signals can be introduced here later if necessary 
        public MascotSpectra(){
            Peptides = new List<Peptide>();
        }
    }

    public class Protein{
        /// <summary>
        /// Name of protein, like "IPI0003232"
        /// </summary>
        public string Name;
        /// <summary>
        /// Long name of protein like "human serum albumin"
        /// </summary>
        public string Descr;
        /// <summary>
        /// Mass of protein
        /// </summary>
        public double Mass;
    }

    public class Mods{
        /// <summary>
        /// to which aminoacid applicable
        /// </summary>
        public string AminoAcids;
        /// <summary>
        /// Name like "Oxidation"
        /// </summary>
        public string Name;
        /// <summary>
        /// Mass shift due to modification like 15.99 for oxidation
        /// </summary>
        public double Mass;
    }

    /// <summary>
    /// Class for extraction data of mascot .dat files
    /// </summary>
    public class MascotParser {
        /// <summary>
        /// List of processed spectra
        /// </summary>
        public List<MascotSpectra> Spectra;
        /// <summary>
        /// List of all proteins for all PSMs
        /// </summary>
        public List<Protein> Proteins;
        //one-based
        /// <summary>
        /// List of fixed modifications applied to dataset
        /// </summary>
        public Mods[] FixedMods;
        /// <summary>
        /// List of variable modifications applied to dataset
        /// </summary>
        public Mods[] VaryMods;
        /// <summary>
        /// Initial name of MGF file
        /// </summary>
        public string MGFFileName;

        public MascotParser(){
            Spectra = new List<MascotSpectra>();
            Proteins = new List<Protein>();
        }

        StreamReader MFile;

        private int SearchFor(string str){
            string buf;
            do {
                buf = MFile.ReadLine();
            } while (buf.IndexOf(str) == -1 || MFile.EndOfStream);
            if (MFile.EndOfStream) { return -1; }
            return 1;
        }

        /// <summary>
        /// Fills data structures from specified file
        /// </summary>
        /// <param name="FileName">Mascot .dat file</param>
        /// <returns>1 if success</returns>
        public int ParseFile (string FileName ){
            MFile = new StreamReader(FileName);
            string str = MFile.ReadLine();
            if (str != "MIME-Version: 1.0 (Generated by Mascot version 1.0)"){
                return -1;
            }

            int k = 0;

            //parametrs
            if (SearchFor("parameters") == -1) return -1;

            string[] Tokens, SubTokens;
            while (str.Length==0 || str[0] != '-' ){
                str = MFile.ReadLine();
                if (str.IndexOf("FILE=") != -1) {
                    MGFFileName = str.Substring(5);
                }
                if (str.IndexOf("MODS=") == 0) {
                    Tokens = str.Split(new char[] {'=',','});
                    FixedMods = new Mods[Tokens.Length];
                }
                if (str.IndexOf("IT_MODS=") != -1) {
                    Tokens = str.Split(new char[] {'=',','});
                    VaryMods = new Mods[Tokens.Length];
                }
            }
            //masses
            if (SearchFor("masses") == -1) return -1;
            str = MFile.ReadLine();
              
            int Ind = 0;
            while (str.Length==0 || str[0] != '-' ){
                str = MFile.ReadLine();
                try{
                    if (str.IndexOf("delta") == 0) {
                        Tokens = str.Split(new char[] {'=',','});
                        Ind = Convert.ToInt32(str.Substring(5,1));
                        VaryMods[Ind] = new Mods();
                        VaryMods[Ind].Mass = Convert.ToDouble(Tokens[1]);
                        VaryMods[Ind].Name = Tokens[2];
                        Tokens = Tokens[2].Split(new char[] {'(',')'});
                        VaryMods[Ind].AminoAcids = Tokens[1];
                    }
                    if (str.IndexOf("FixedMod") == 0) {
                        if ( ! char.IsDigit(str[8])) continue;
                        Tokens = str.Split(new char[] {'=',','});
                        Ind = Convert.ToInt32(str.Substring(8,1));
                        FixedMods[Ind] = new Mods();
                        FixedMods[Ind].Mass = Convert.ToDouble(Tokens[1]);
                        FixedMods[Ind].Name = Tokens[2];
                        Tokens = Tokens[2].Split(new char[] {'(',')'});
                        FixedMods[Ind].AminoAcids = Tokens[1];
                    }
                }
                catch (Exception e){
                    Exception ne = new Exception("Inconsistent descriprion of modifications in .dat file.", e);
                    throw ne;
                }
            }

            //Summary
            if (SearchFor("summary") == -1) return -1;
            //spacing 
            MFile.ReadLine();
            //spectra headers reading loop 
            str = MFile.ReadLine();

            MascotSpectra ms; 
            string str1;
            while (str.IndexOf("num_hits") == -1){
                ms = new MascotSpectra();
                //first string like "qmass1=402.204024"
                ms.Mass = Convert.ToDouble(str.Substring(str.IndexOf('=')+1));
                //second string like "qexp1=403.211300,1+"
                str = MFile.ReadLine();
                str1 = str.Substring(str.IndexOf('=')+1);
                ms.mz = Convert.ToDouble(str1.Substring(0,str1.IndexOf(',')-1));
                ms.Charge = Convert.ToInt32(str1.Substring(str1.IndexOf(',')+1,1));
                //third string like "qintensity1=1.0000" if exsist
                str = MFile.ReadLine();
                if (str.Length >=10 && str.Substring(0,10) == "qintensity") {
                    ms.Intensity = Convert.ToDouble(str.Substring(str.IndexOf('=')+1));
                    //fourth string qmatch1=21075
                    str = MFile.ReadLine();
                }
                Spectra.Add(ms);
                //fifth qplughole1=29.158475
                str = MFile.ReadLine();
                //other first 
                str = MFile.ReadLine();
            }
            //Peptides 
            if (SearchFor("peptides") == -1) return -1;
            //spacing
            MFile.ReadLine();
            //spectra interpretation reading loop 
            int SpectaNumber;
            str = MFile.ReadLine();
            Peptide pep;
            for(int i = 0 ; i<Spectra.Count ; i++){
                if (str.Substring(str.IndexOf('=')+1) != "-1") {
                    SpectaNumber = Convert.ToInt32(str.Substring(1,str.IndexOf('_')-1));
                    //PSMs reading loop
                    while (SpectaNumber-1 == i){
                        Tokens = str.Split(new char[] {'=',',',';'});
                        pep = new Peptide();
                        pep.Mass = Convert.ToDouble(Tokens[2]);
                        pep.Delta = Convert.ToDouble(Tokens[3]);
                        pep.Sequence = Tokens[5];
                        pep.Score = Convert.ToDouble(Tokens[8]);
                        int Prots = Tokens.GetLength(0) - 12;
                        pep.ProteinNames = new string[Prots];
                        pep.StartPositons = new int[Prots];
                        pep.EndPositons = new int[Prots];
                        for ( int j = 12 ; j < Tokens.GetLength(0); j++){
                            SubTokens = Tokens[j].Split(new char[] {':'});
                            pep.ProteinNames[j-12] = SubTokens[0];
                            pep.StartPositons[j-12] = Convert.ToInt32(SubTokens[2]);
                            pep.StartPositons[j-12] = Convert.ToInt32(SubTokens[3]);
                        }
                        pep.ModIndex = new int[pep.Sequence.Length];
                        for ( k = 0 ; k < pep.Sequence.Length ; k++ ){
                            pep.ModIndex[k] = Convert.ToInt32(Tokens[7].Substring(k+1,1));
                        }
                        pep.CModIndex = Convert.ToInt32(Tokens[7].Substring(pep.Sequence.Length+1,1));
                        pep.NModIndex = Convert.ToInt32(Tokens[7].Substring(0,1));
                        while ( Tokens[0] == str.Substring(0,Tokens[0].Length) ){
                            str = MFile.ReadLine();//all other strings
                            if (str.IndexOf("subst")!= -1){
                                SubTokens  = str.Split(new char[] {'=',','});
                                pep.Sequence = pep.Sequence.Replace(SubTokens[2],SubTokens[3]);
                            }
                        Spectra[i].Peptides.Add(pep);
                        }
                        if (str[0] == '-') break;
                        SpectaNumber = Convert.ToInt32(str.Substring(1,str.IndexOf('_')-1));
                    }
                }else{
                    str = MFile.ReadLine();
                    if (str[0] == '-') break;
                    SpectaNumber = Convert.ToInt32(str.Substring(1,str.IndexOf('_')-1));
                }
            }
            //Proteins
            if (SearchFor("proteins") == -1) return -1;
            //spacing  
            MFile.ReadLine();
            //proteins reading loop 
            str = MFile.ReadLine();
            Protein prot;
            while (str[0] != '-'){
                prot = new Protein();
                int eqpos,commapos;
                eqpos = str.IndexOf('=');
                commapos = str.IndexOf(',');
                prot.Name = str.Substring(0,eqpos);
                prot.Descr = str.Substring(commapos+1);
                prot.Mass = Convert.ToDouble(str.Substring(eqpos+1,commapos-eqpos-1));
                Proteins.Add(prot);
                str = MFile.ReadLine();
                //Mascot 2.4 может содержать строчки с информацией о таксономии вида 
                //"ZN726_HUMAN"_tax=9606 - мы их пока будем пропускать 
                while (str.IndexOf("\"_tax=") != -1){
                    str = MFile.ReadLine();
                }
            }
            //QueryN 
            //now we interesed only by titles from advanced description
            //there was some broken description in sample file 
            //so for broken descs we leave blank fields Title,RT and SN 
            for(int i = 0 ; i<Spectra.Count ; i++){
                try {
                    if (SearchFor("query"+Convert.ToString(i+1)) == -1) return -1;
                    do {
                        str = MFile.ReadLine();
                        if (str.Length>5 && str.Substring(0,5).ToLower() == "title"){
                            ms = Spectra[i];
                            ms.Title = str.Substring(6);
                            if (str.IndexOf("FinneganScanNumber%3a%20") != -1){
                                string buf = str.Substring(str.IndexOf("FinneganScanNumber%3a%20")+24);
                                for ( k = 0 ; k<buf.Length && char.IsNumber(buf[k]) ; k++); 
                                ms.ScanNumber = Convert.ToInt32(buf.Substring(0,k));
                            }else{
                                ms.ScanNumber = 0 ;
                            }
                            if (str.IndexOf("Elution%20from%3a%20") != -1){
                                string buf = str.Substring(str.IndexOf("Elution%20from%3a%20") + 20);
                                buf = buf.Replace("%2e", ".");
                                ms.RT = Convert.ToDouble(buf.Substring(0, buf.IndexOf("%20")));
                            }else{
                                ms.RT = 0;
                            }
                            if (str.IndexOf("RT%20Apex%3a%20") != -1) {
                                string buf = str.Substring(str.IndexOf("RT%20Apex%3a%20") + 15);
                                buf = buf.Replace("%2e", ".");
                                ms.RTApex = Convert.ToDouble(buf.Substring(0, buf.IndexOf("%20")));
                            } else {
                                ms.RTApex = 0;
                            }
                        }
                        if (str.Length>6 && str.Substring(0,6).ToLower() == "charge"){
                            Spectra[i].Charge = Convert.ToInt32(str.Substring(7,1));
                        }
                        if (str.Length>5 && str.Substring(0,5).ToLower() == "scans"){
                            Spectra[i].ScanNumber = Convert.ToInt32(str.Substring(6));
                        }
                        if (str.Length>11 && str.Substring(0,11).ToLower() == "rtinseconds"){
                            Spectra[i].RT = Convert.ToDouble(str.Substring(12))/60.0;
                        }
                    }while(str.Length==0 || str[0]!='-');
                }catch {
                    MFile.Close();
                    MFile = new StreamReader(FileName);
                };
            }

            //access by ScanNumbers;
            int MaxFSN = 0;
            for ( int i = 0 ; i < Spectra.Count ; i++){
                if (Spectra[i].ScanNumber>MaxFSN) MaxFSN = Spectra[i].ScanNumber;
            }
            FSNAccessions = new int[MaxFSN+1];
            for ( int i = 0 ; i <= MaxFSN ; i++){
                FSNAccessions[i] = -1;
            }
            for ( int i = 0 ; i < Spectra.Count ; i++){
                FSNAccessions[Spectra[i].ScanNumber] = i;
            }
            return 1;
        }

        int[] FSNAccessions;

        /// <summary>
        /// Access to spectra collection by Finnegan scan number
        /// </summary>
        /// <param name="SN">Desired scan number</param>
        /// <returns></returns>
        public MascotSpectra AccessByFSN(int SN){
            if (FSNAccessions[SN] != -1){
                return Spectra[FSNAccessions[SN]];
            }else{
                return null;
            }
        }

        /// <summary>
        /// Access to spectra collection by Mascot query number (displayd in Mascot reports)
        /// </summary>
        /// <param name="QN"></param>
        /// <returns></returns>
        public MascotSpectra AccessByQueryNumber(int QN){
            if ( Spectra.Count > QN && QN>0){
                return Spectra[QN-1];
            }else{
                return null;
            }
        }


        /// <summary>
        /// Check if false discovery rate calculation is availabvle 
        /// </summary>
        /// <returns>True if FDR available</returns>
        public bool IsFDRAvialable() {
            bool Res = false;
            for (int i = 0; i < Proteins.Count; i++) {
                Res |= Proteins[i].Name.ToUpper().Contains("REVERSED");
            }
            return Res;
        }
        /// <summary>
        /// Determine Mascot threshold for desired FDR
        /// </summary>
        /// <param name="FDR">FDR - </param>
        /// <returns>Mascot score threshold in percents</returns>
        public double MascotThresForFDR(double FDR) {
            Spectra.Sort(new MascotSpectrabyScore());
            int DirectCount = 0, ReversedCount = 0, i;
            for (i = 0 ; i < Spectra.Count ; i++){
                if (Spectra[i].Peptides.Count == 0) 
                    break;
                bool Reversed = true;
                for (int j = 0 ; j < Spectra[i].Peptides[0].ProteinNames.Length ; j++ ){
                    Reversed &= Spectra[i].Peptides[0].ProteinNames[j].ToUpper().Contains("REVERSED");
                }
                if (Reversed){
                    ReversedCount++;
                }else{
                    DirectCount++;
                }
                double FDRRatio = ((double)ReversedCount/(double)DirectCount)*100.0;
                if (FDRRatio > FDR) 
                    break;
            }
            if (i < Spectra.Count ){
               if (Spectra[i].Peptides.Count >0){
                    return Spectra[i].Peptides[0].Score;
               }else{
                   return 0.0;
               }
            }else{
                return Spectra[i-1].Peptides[0].Score;
            }
        }
    }
}
