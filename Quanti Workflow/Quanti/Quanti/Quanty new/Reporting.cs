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
using Quanty.Properties;
using System.IO;

namespace Quanty {

    public class Prints{

        static List<string> FileList;

        public void Init(List<string> Files){
            (new QPeptide()).SetReportString(Settings.Default.ReportVector.Substring(4,30));
            (new QProtein()).SetReportString(Settings.Default.ReportVector.Substring(34,14));
            (new QPeptide()).SetSheetReportString(Settings.Default.ReportVector.Substring(48,17));
            (new QProtein()).SetSheetReportString(Settings.Default.ReportVector.Substring(65,14));
            (new QProtein()).SetFileList(Files);
            (new QPeptide()).SetFileList(Files);
            FileList = Files;
        }

        public List<IPrinting> ForPrinting( List<QPeptide> RList){
            List<IPrinting> Res = new List<IPrinting>();
            Res.AddRange(RList);
            return Res;
        }

        public List<IPrinting> ForPrinting( List<QProtein> RList){
            List<IPrinting> Res = new List<IPrinting>();
            Res.AddRange(RList);
            return Res;
        }

        public void WriteHeader(StreamWriter sw, string RawFileName){
	        sw.WriteLine(Settings.Default.CaptionText);
            sw.WriteLine("RAW: {0}\r\nDAT: {1}", RawFileName, Settings.Default.DatFileName);
            sw.WriteLine("REF: {0}", Settings.Default.Reference);

	        sw.WriteLine("Filter Options: Mascot Score >=: {0}; Mass +/- ppm: {1}; RT: +/- ({2} %, {3} min); RT Order +/- {4}; Pept. per prot. >= {5} ", 
                Settings.Default.MascotScore,
                Settings.Default.MassError,
                Settings.Default.RTErrorProc,
                Settings.Default.RTErrorMinutes,
                Settings.Default.RTOrder,
                Settings.Default.PeptperProt);

            sw.WriteLine("Misc Options: {0} {1} Metabolomics options: Metabolomic Profiling:{2}; Force Stick Mode:{3}; RT peak width {4}; RT peak resolution {5};", 
                Settings.Default.Deconvolution?"Charge deconvolution;":"",
                Settings.Default.MascotScoreFiltering?"Using Best Mascot Peptides;":"Ignoring Best Mascot Peptides;",
                Settings.Default.MetaProfile?"Yes":"No",
                Settings.Default.StickMode?"Yes":"No",
                Utils.GetStringValue(Settings.Default.MinRTWidht)==0.0?"Not considered":String.Format("in {0:f2} to {1:f2} min.",Settings.Default.MinRTWidht,Settings.Default.MaxRTWidth),
                Utils.GetStringValue(Settings.Default.RTRes)==0.0?"Not considered":String.Format("{0}%.",Settings.Default.RTRes)
                );

            sw.WriteLine("Ref Options: ESI Interval Correction (min):{0}; Normalization:{1}; Peak shape filter:{2}; Cut Tails:{3}; Use All MSMS:{4}; Max Zeroes:{5}; Zero subst.:{6}", 
                Utils.GetStringValue(Settings.Default.ESIInterval)==0.0?"None":Settings.Default.ESIInterval,
                Settings.Default.Norm?"Yes":"None",
                Settings.Default.ShapeFilter?"Yes":"None",
                Settings.Default.CutTails?"Yes":"No",
                Settings.Default.UseAll?"Yes":"No",
                Settings.Default.MaxZeroes,
                Settings.Default.ZeroSubst);
        }

        public void PrintFile(List<IPrinting> RepList, int MatchNumber, int RefMatchNumber, string Extension){
            string FileName;
            if (FileList[MatchNumber].Contains("{Sample")){
                FileName = Settings.Default.OutFilePath+Path.DirectorySeparatorChar+
                    FileList[MatchNumber].Substring(1,FileList[MatchNumber].IndexOf("}")-1)
                    +Extension;
            }else{
                FileName = Settings.Default.OutFilePath+Path.DirectorySeparatorChar+
                    Path.GetFileNameWithoutExtension(FileList[MatchNumber])+Extension;
            }
            StreamWriter Outfile = new StreamWriter(FileName);
            WriteHeader(Outfile,FileList[MatchNumber]);
            Outfile.WriteLine(RepList[0].GetHeader()); ;
            for(int j = 0; j<RepList.Count ; j++){
                Outfile.WriteLine(RepList[j].GetString(MatchNumber,RefMatchNumber)); 
            }
            Outfile.Close();
        }

        public  void PrintMatrix(List<IPrinting> RepList , string Extension){
            string FileName;
            if (Settings.Default.DatFileName != "Merged"){
                FileName = Settings.Default.OutFilePath+Path.DirectorySeparatorChar+Path.GetFileNameWithoutExtension(Settings.Default.DatFileName)+Extension;
            }else{
                FileName = Settings.Default.OutFilePath+Path.DirectorySeparatorChar+Path.GetFileNameWithoutExtension(Settings.Default.DBFileName)+Extension;
            }
            StreamWriter pepshit = new StreamWriter(FileName);
            WriteHeader(pepshit,"Matrix file");
            pepshit.WriteLine(RepList[0].GetMatrixHeader()); ;
            for(int j = 0; j<RepList.Count ; j++){
                string RepString = RepList[j].GetMatrixString();
                if (RepString != ""){
                    pepshit.WriteLine(RepString); 
                }
            }
            pepshit.Close();
        }

        public void PrintSheet(List<IPrinting> RepList , int RefNumber, string Extension){
            string FileName; 
            if (Settings.Default.Reference.Contains("{Sample")){
                FileName = Settings.Default.OutFilePath+Path.DirectorySeparatorChar+
                    Path.GetFileNameWithoutExtension(Settings.Default.DatFileName)+"_on_"+
                    Settings.Default.Reference.Substring(1,Settings.Default.Reference.IndexOf("}")-1)
                    +Extension;
            }else{
                FileName = Settings.Default.OutFilePath+Path.DirectorySeparatorChar+
                    Path.GetFileNameWithoutExtension(Settings.Default.DatFileName)+"_on_"+
                    Path.GetFileNameWithoutExtension(Settings.Default.Reference)+Extension;
            }
            StreamWriter pepshit = new StreamWriter(FileName);
            WriteHeader(pepshit,"Sheet file");
            pepshit.WriteLine(RepList[0].GetSheetHeader()); ;
            for(int j = 0; j<RepList.Count ; j++){
                pepshit.WriteLine(RepList[j].GetSheetString(RefNumber)); 
            }
            pepshit.Close();
        }

    }

    public interface IPrinting{
        void SetReportString(string Patt);
        void SetSheetReportString(string Patt);
        void SetFileList(List<string> Files);

        string GetHeader();
        string GetSheetHeader();
        string GetMatrixHeader();
        string GetString(int MatchNumber, int RefMatchNumber /* if no refs =-1 */);
        string GetSheetString(int RefNumber);
        string GetMatrixString();
    }

    public partial class QPeptide : IPrinting{
        static string ReportString;
        static string SheetReportString;
        static List<string> FileList;

        public void SetReportString(string Patt){
            ReportString = Patt;
        }

        public void SetSheetReportString(string Patt){
            SheetReportString = Patt;
        }

        public void SetFileList(List<string> Files){
            FileList = Files;
        }



        public string GetHeader(){
            return 
                ((ReportString[0]!='1')?"":"SEQUENCE\t")+
                ((ReportString[1]!='1')?"":"RT MASCOT\t")+
                ((ReportString[2]!='1')?"":"RT APEX\t")+
                ((ReportString[3]!='1')?"":"RT ORDER\t")+
                ((ReportString[4]!='1')?"":"RT SMD\t")+
                ((ReportString[5]!='1')?"":"SHIFT\t")+
                ((ReportString[6]!='1')?"":"SVERT\t")+
                ((ReportString[7]!='1')?"":"SMD Ratio\t")+
                ((ReportString[8]!='1')?"":"ESI CORRECTION\t")+
                ((ReportString[9]!='1')?"":"INTEGRATED INTENSITY\t")+
                ((ReportString[10]!='1')?"":"REF.INT.\t")+
                ((ReportString[11]!='1')?"":"Log1\t")+
                ((ReportString[12]!='1')?"":"Log2\t")+
                ((ReportString[13]!='1')?"":"HIGHEST  INTENSITY\t")+
                ((ReportString[14]!='1')?"":"MZ MASCOT\t")+
                ((ReportString[15]!='1')?"":"MZ QUANTI\t")+
                ((ReportString[16]!='1')?"":"CHARGE\t")+
                ((ReportString[17]!='1')?"":"PI\t")+
                ((ReportString[18]!='1')?"":"ELEMENTS\t")+
                ((ReportString[19]!='1')?"":"MS SPECTRA\t")+
                ((ReportString[20]!='1')?"":"IPI\t")+
                ((ReportString[21]!='1')?"":"IPI DESC\t")+
                ((ReportString[22]!='1')?"":"IPIS\t")+
                ((ReportString[23]!='1')?"":"MASCOT SCORE\t")+
                ((ReportString[24]!='1')?"":"MODIFICATION MASS\t")+
                ((ReportString[25]!='1')?"":"ISOTOPE RATIO\t")+
                ((ReportString[26]!='1')?"":"ISOTOPE RATIO THEOR\t")+
                ((ReportString[27]!='1')?"":"MOD DESCR.\t")+
                ((ReportString[28]!='1')?"":"CASE\t")+
                ((ReportString[29]!='1')?"":"COMMENTS");
        }


        public string GetSheetHeader(){
            string Res = 
                ((SheetReportString[0]!='1')?"":"SEQUENCE\t")+
                ((SheetReportString[1]!='1')?"":"PROTEIN ID\t")+
                ((SheetReportString[2]!='1')?"":"PROTEIN DESC\t")+
                ((SheetReportString[3]!='1')?"":"PROTEIN IDs\t")+
                ((SheetReportString[4]!='1')?"":"REF ABUNDANCE\t")+
                ((SheetReportString[5]!='1')?"":"MODIF\t")+
                ((SheetReportString[6]!='1')?"":"MZ\t")+
                ((SheetReportString[7]!='1')?"":"CHARGE\t")+
                ((SheetReportString[8]!='1')?"":"MASCOT SCORE\t")+
                ((SheetReportString[9]!='1')?"":"PI\t")+
                ((SheetReportString[10]!='1')?"":"MODIFICATION MASS\t")+
                ((SheetReportString[11]!='1')?"":"ELEMENTS\t")+
                ((SheetReportString[12]!='1')?"":"CASE\t");
            if (SheetReportString[13] == '1'){
                for (int i = 0 ; i < FileList.Count ; i++){
                    Res = Res + FileList[i]+"\t";
                }
            }
            if (SheetReportString[14] == '1'){
                for (int i = 0 ; i < FileList.Count ; i++){
                    Res = Res + FileList[i]+"-RT Apex\t";
                }
            }
            if (SheetReportString[15] == '1'){
                for (int i = 0 ; i < FileList.Count ; i++){
                    Res = Res + FileList[i]+"-MZ Quanti\t";
                }
            }

            if (SheetReportString[16] == '1'){
                for (int i = 0 ; i < FileList.Count ; i++){
                    Res = Res + FileList[i]+"-RAW Values\t";
                }
            }

            return Res;
        }

        public string GetMatrixHeader(){
            string Res = 
                ((SheetReportString[0]!='1')?"":"SEQUENCE\t")+
                ((SheetReportString[1]!='1')?"":"PROTEIN ID\t")+
                ((SheetReportString[2]!='1')?"":"PROTEIN DESC\t")+
                ((SheetReportString[3]!='1')?"":"PROTEIN IDs\t")+
                ((SheetReportString[4]!='1')?"":"REF ABUNDANCE\t")+
                ((SheetReportString[5]!='1')?"":"MODIF\t")+
                ((SheetReportString[6]!='1')?"":"MZ\t")+
                ((SheetReportString[7]!='1')?"":"CHARGE\t")+
                ((SheetReportString[8]!='1')?"":"MASCOT SCORE\t")+
                ((SheetReportString[9]!='1')?"":"PI\t")+
                ((SheetReportString[10]!='1')?"":"MODIFICATION MASS\t")+
                ((SheetReportString[11]!='1')?"":"ELEMENTS\t")+
                ((SheetReportString[12]!='1')?"":"CASE\t");

            if (SheetReportString[13] == '1'){
                for (int i = 0 ; i < FileList.Count ; i++){
                    Res = Res + FileList[i]+"\t";
                }
            }
            if (SheetReportString[14] == '1'){
                for (int i = 0 ; i < FileList.Count ; i++){
                    Res = Res + FileList[i]+"-RT Apex\t";
                }
            }
            if (SheetReportString[15] == '1'){
                for (int i = 0 ; i < FileList.Count ; i++){
                    Res = Res + FileList[i]+"-MZ Quanti\t";
                }
            }

            if (SheetReportString[15] == '1'){
                for (int i = 0 ; i < FileList.Count ; i++){
                    Res = Res + FileList[i]+"-RAW Values\t";
                }
            }

            return Res;
        }


        public string GetString(int MatchNumber,int RefMatchNumber /* if no refs =-1 */){
            string comment = ""; //comment сейчас вообще никак не работает 
            QMatch Match,Refs;
            PeptidePair Pair;

            if (Aligner != null){
                Pair = Aligner.GetPeptPair(this, MatchNumber, RefMatchNumber);
                Match = Pair.LeftMatch;
                Refs = Pair.RightMatch;
            }else{
                Match = Matches[MatchNumber];
                Refs = null;
                Pair = null;
            }

            
            string Out = 
                ((ReportString[0]!='1')?"":String.Format("{0:20} \t",(Sequence ?? "NOSEQUENCE")))+
                ((ReportString[1]!='1')?"":String.Format("{0:f4}\t",MascotRT))+
                ((ReportString[2]!='1')?"":String.Format("{0:f4}\t",(Match != null)?Match.ApexRT:0.0))+
                ((ReportString[3]!='1')?"":String.Format("{0}\t",MascotScan))+
                ((ReportString[4]!='1')?"":String.Format("{0:F6}\t",(Match != null)?Match.RTDisp:0.0))+
                ((ReportString[5]!='1')?"":String.Format("{0}\t",(Pair != null)?Pair.Shift:0.0))+
                ((ReportString[6]!='1')?"":String.Format("{0}\t",(Pair != null)?Pair.MaxConv:0.0))+
                ((ReportString[7]!='1')?"":String.Format("{0}\t",(Match != null && Refs != null)?Match.RTDisp/Refs.RTDisp:0.0))+
                ((ReportString[8]!='1')?"":String.Format("{0:f4}\t",(Pair != null)?Pair.ESIFactor:0.0))+
                ((ReportString[9]!='1')?"":String.Format("{0:f2}\t",(Match != null)?Match.Score:0.0))+
                ((ReportString[10]!='1')?"":String.Format("{0}\t",(Refs != null)?Refs.Score:0.0))+
                ((ReportString[11]!='1')?"":String.Format("{0}\t",(Match != null)?Math.Log10(Match.Score):0.0))+
                ((ReportString[12]!='1')?"":String.Format("{0}\t",(Refs != null)?Math.Log10(Refs.Score):0.0))+
                ((ReportString[13]!='1')?"":String.Format("{0:f2}\t",(Match != null)?Match.ApexScore:0.0))+
                ((ReportString[14]!='1')?"":String.Format("{0:f5}\t",MascotMZ))+
                ((ReportString[15]!='1')?"":String.Format("{0:f5}\t",(Match != null)?Match.ApexMZ:0.0))+
                ((ReportString[16]!='1')?"":String.Format("{0}\t",Charge))+
                ((ReportString[17]!='1')?"":String.Format("{0}\t",PI))+
                ((ReportString[18]!='1')?"":String.Format("{0}\t",Sequence==null?"NOSEQUENCE":Fragmentation.Peptide.CalcElemComp(Sequence)))+
                ((ReportString[19]!='1')?"":String.Format("{0}\t",Match == null || Match.MSMatches==null?0:Match.MSMatches.Count-2))+
                ((ReportString[20]!='1')?"":String.Format("{0}\t",(IPI ?? "NOPROTEIN")))+
                ((ReportString[21]!='1')?"":String.Format("{0}\t",(PartOfProtein != null ? PartOfProtein.Desc : "NOPROTEIN")))+
                ((ReportString[22]!='1')?"":(Utils.IPIListtoString(IPIs))+"\t")+
                ((ReportString[23]!='1')?"":String.Format("{0:f2}\t",MascotScore))+
                ((ReportString[24]!='1')?"":String.Format("{0:f5}\t",ModMass))+
                ((ReportString[25]!='1')?"":String.Format("{0:f5}\t",(Match != null)?Match.IsotopeRatio:0.0))+
                ((ReportString[26]!='1')?"":String.Format("{0:f5}\t",TheorIsotopeRatio))+
                ((ReportString[27]!='1')?"":String.Format("{0}\t",ModDesk))+
                ((ReportString[28]!='1')?"":String.Format("{0}\t",Case))+
                ((ReportString[29]!='1')?"":String.Format("{0}",comment));
 
//              Out = String.Format("{0}\t{1}\t"+Out,RTNumber,MascotScan);
            return Out;

        }

        public string GetSheetString(int RefNumber){
            double Ref = 0.0;
            int Count = 0;
            if (RefNumber < 0){
                for (int i = 0 ; i < FileList.Count ; i++ ){
                    if (Matches[i]!=null){
                        Ref += Math.Log10(Matches[i].Score);
                        Count++;
                    }
                }
                Ref = Math.Pow(10,(Ref/(double)Count));
            }
            //!!добавить case 
            string Res = 
                ((SheetReportString[0]!='1')?"":String.Format("{0}\t",Sequence))+
                ((SheetReportString[1]!='1')?"":String.Format("{0}\t",IPI))+
                ((SheetReportString[2]!='1')?"":String.Format("{0}\t",PartOfProtein != null ? PartOfProtein.Desc : "No protein description"))+
                ((SheetReportString[3]!='1')?"":(Utils.IPIListtoString(IPIs))+"\t")+
                //reference abundance
                ((SheetReportString[4]!='1')?"":String.Format("{0:f2}\t",
                (RefNumber>=0?(Matches[RefNumber]==null?0.0:Matches[RefNumber].Score):Ref)))+
                ((SheetReportString[5]!='1')?"":String.Format("{0}\t",(ModDesk != null ? ModDesk : "")))+
                ((SheetReportString[6]!='1')?"":String.Format("{0:f5}\t",MascotMZ))+
                ((SheetReportString[7]!='1')?"":String.Format("{0}\t",Charge))+
                ((SheetReportString[8]!='1')?"":String.Format("{0:f2}\t",MascotScore))+
                ((SheetReportString[9]!='1')?"":String.Format("{0}\t",PI))+
                ((SheetReportString[10]!='1')?"":String.Format("{0:f5}\t",ModMass))+
                ((SheetReportString[11]!='1')?"":String.Format("{0}\t",Sequence==null?"NOSEQUENCE":Fragmentation.Peptide.CalcElemComp(Sequence)))+
                ((SheetReportString[12]!='1')?"":String.Format("{0}\t",Case));

            if (SheetReportString[13] == '1'){
                if (Aligner != null){
                    double[] Ratios = Aligner.GetPeptRatios(this);
                    for (int i = 0 ; i < FileList.Count ; i++){
                        Res = Res + String.Format("{0:f5}\t",Ratios[i]);
                    }
                }else{
                    for (int i = 0 ; i < FileList.Count ; i++){
                        if (Matches[i]!=null){
                            Res = Res + String.Format("{0:f5}\t",Matches[i].Score/Ref);
                        }else{
                            Res = Res + "0.0\t";
                        }
                    }
                }
            }

            if (SheetReportString[14] == '1'){
                for (int i = 0 ; i < FileList.Count ; i++){
                    if (Matches[i]!=null){
                        Res = Res + String.Format("{0:f5}\t",Matches[i].ApexRT);
                    }else{
                        Res = Res + "0.0\t";
                    }
                }
            }

            if (SheetReportString[15] == '1'){
                for (int i = 0 ; i < FileList.Count ; i++){
                    if (Matches[i]!=null){
                        Res = Res + String.Format("{0:f5}\t",Matches[i].ApexMZ);
                    }else{
                        Res = Res + "0.0\t";
                    }
                }
            }

           if (SheetReportString[16] == '1'){
                if (Aligner != null){
                    double[] Ratios = Aligner.GetPeptRatios(this);
                    for (int i = 0 ; i < FileList.Count ; i++){
                        Res = Res + String.Format("{0:f5}\t",Ratios[i]*(Matches[RefNumber]==null?0.0:Matches[RefNumber].Score));
                    }
                }else{
                    for (int i = 0 ; i < FileList.Count ; i++){
                        if (Matches[i]!=null){
                            Res = Res + String.Format("{0:f5}\t",Matches[i].Score);
                        }else{
                            Res = Res + "0.0\t";
                        }
                    }
                }
            }


            return Res;
        }

        public string GetMatrixString(){
            string Res;
            int Count = 0;
            double MatrixScore = 0.0;
            double[] Ratios = Aligner.GetPeptRatios(this);
            for (int i = 0 ; i < FileList.Count ; i++){
                if (Matches[i] != null && Matches[i].Score > 0.0){
                    MatrixScore+=Math.Log10(Matches[i].Score);
                    Count++;
                }
            }
            MatrixScore = Math.Pow(10, MatrixScore/(double)Count);
            Res = 
                ((SheetReportString[0]!='1')?"":String.Format("{0}\t",Sequence))+
                ((SheetReportString[1]!='1')?"":String.Format("{0}\t",IPI))+
                ((SheetReportString[2]!='1')?"":String.Format("{0}\t", PartOfProtein != null ? PartOfProtein.Desc : "No protein description" ))+
                ((SheetReportString[3]!='1')?"":(Utils.IPIListtoString(IPIs))+"\t")+
                ((SheetReportString[4]!='1')?"":String.Format("{0:f2}\t",MatrixScore))+
                ((SheetReportString[5]!='1')?"":String.Format("{0}\t",(ModDesk != null ? ModDesk : "")))+
                ((SheetReportString[6]!='1')?"":String.Format("{0:f5}\t",MascotMZ))+
                ((SheetReportString[7]!='1')?"":String.Format("{0}\t",Charge))+
                ((SheetReportString[8]!='1')?"":String.Format("{0:f2}\t",MascotScore))+
                ((SheetReportString[9]!='1')?"":String.Format("{0}\t",PI))+
                ((SheetReportString[10]!='1')?"":String.Format("{0:f5}\t",ModMass))+
                ((SheetReportString[11]!='1')?"":String.Format("{0}\t",Sequence==null?"NOSEQUENCE":Fragmentation.Peptide.CalcElemComp(Sequence)))+
                ((SheetReportString[12]!='1')?"":String.Format("{0}\t",Case));


            if (SheetReportString[13] == '1'){
                for (int i = 0 ; i < FileList.Count ; i++){
                    Res = Res + String.Format("{0:f5}\t",Ratios[i]);
                }
            }

            if (SheetReportString[14] == '1'){
                for (int i = 0 ; i < FileList.Count ; i++){
                    if (Matches[i]!=null){
                        Res = Res + String.Format("{0:f5}\t",Matches[i].ApexRT);
                    }else{
                        Res = Res + "0.0\t";
                    }
                }
            }

            if (SheetReportString[15] == '1'){
                for (int i = 0 ; i < FileList.Count ; i++){
                    if (Matches[i]!=null){
                        Res = Res + String.Format("{0:f5}\t",Matches[i].ApexMZ);
                    }else{
                        Res = Res + "0.0\t";
                    }
                }
            }

            if (SheetReportString[16] == '1'){
                for (int i = 0 ; i < FileList.Count ; i++){
                    Res = Res + String.Format("{0:f5}\t",Ratios[i]*MatrixScore);
                }
            }


            return Res;
        }
    }

    public partial class QProtein : IPrinting{
        static string ReportString;
        static string SheetReportString;
        static List<string> FileList;

        public void SetReportString(string Patt){
            ReportString = Patt;
        }

        public void SetSheetReportString(string Patt){
            SheetReportString = Patt;
        }

        public void SetFileList(List<string> Files){
            FileList = Files;
        }

        public string GetHeader(){
            return (ReportString[0]=='1'?"PROTEIN ID\t":"")+
                (ReportString[1]=='1'?"SCORE\t":"")+
                (ReportString[2]=='1'?"REF. SCORE\t":"")+
                (ReportString[3]=='1'?"DESCRIPTION\t":"")+
                (ReportString[4]=='1'?"PROTEIN IDs\t":"")+
                (ReportString[5]=='1'?"PEPTIDES IDENT\t":"")+
                (ReportString[6]=='1'?"PEPTIDES FOUND\t":"")+
                (ReportString[7]=='1'?"R-SQUARE\t":"")+
                (ReportString[8]=='1'?"SLOPE\t":"")+
                (ReportString[9]=='1'?"MEDIAN\t":"")+
                (ReportString[10]=='1'?"AVERAGE\t":"")+
                (ReportString[11]=='1'?"MIN. CONF.\t":"")+
                (ReportString[12]=='1'?"MAX. CONF.\t":"")+
                (ReportString[13]=='1'?"P-VALUE":"");
        }

        public string GetSheetHeader(){
            string Res = 
                ((SheetReportString[0]!='1')?"":"PROTEIN ID\t")+
                ((SheetReportString[1]!='1')?"":"REF ABUNDANCE\t")+
                ((SheetReportString[2]!='1')?"":"DESCRIPTION\t")+
                ((SheetReportString[3]!='1')?"":"PROTEIN IDs\t")+
                ((SheetReportString[4]!='1')?"":"PEPTIDES\t");

            if (Aligner != null ){
                for (int i = 0 ; i < FileList.Count ; i++){
                    if (SheetReportString[5]=='1')
                        Res = Res + FileList[i]+" Median\t";
                    if (SheetReportString[6]=='1')
                        Res = Res + FileList[i]+" Slope\t";
                    if (SheetReportString[7]=='1')
                        Res = Res + FileList[i]+" Average\t";
                    if (SheetReportString[8]=='1')
                        Res = Res + FileList[i]+" Min.\t";
                    if (SheetReportString[9]=='1')
                        Res = Res + FileList[i]+" Max.\t";
                    if (SheetReportString[10]=='1')
                        Res = Res + FileList[i]+" R-Square.\t";
                }
            }

            if (SheetReportString[12]=='1'){
                Res = Res + "\t";
                for (int i = 0 ; i < FileList.Count ; i++){
                   Res = Res + FileList[i]+"-ppm \t";
                }
            }
            if (SheetReportString[13]=='1'){
                Res = Res + "\t";
                for (int i = 0 ; i < FileList.Count ; i++){
                   Res = Res + FileList[i]+"-RAW Values \t";
                }
            }

            return Res;
        }

        public string GetMatrixHeader(){
            string Res = 
                ((SheetReportString[0]!='1')?"":"PROTEIN ID\t")+
                ((SheetReportString[1]!='1')?"":"REF ABUNDANCE\t")+
                ((SheetReportString[2]!='1')?"":"DESCRIPTION\t")+
                ((SheetReportString[3]!='1')?"":"PROTEIN IDs\t")+
                ((SheetReportString[4]!='1')?"":"PEPTIDES\t");

            for (int i = 0 ; i < FileList.Count ; i++){
               Res = Res + FileList[i]+"\t";
            }

            if (SheetReportString[11]=='1'){
                Res = Res + "\t";
                for (int i = 0 ; i < FileList.Count ; i++){
                   Res = Res + FileList[i]+"-Maximum \t";
                }
                Res = Res + "\t";
                for (int i = 0 ; i < FileList.Count ; i++){
                   Res = Res + FileList[i] + "-Minimum \t";
                }
            }

            if (SheetReportString[12]=='1'){
                Res = Res + "\t";
                for (int i = 0 ; i < FileList.Count ; i++){
                   Res = Res + FileList[i]+"-ppm \t";
                }
            }

            if (SheetReportString[13]=='1'){
                Res = Res + "\t";
                for (int i = 0 ; i < FileList.Count ; i++){
                   Res = Res + FileList[i]+"-RAW Values \t";
                }
            }

            return Res;

        }

        public string GetString(int MatchNumber,int RefMatchNumber /* if no refs =-1 */){

            ProteinPair Pair;
            if (Aligner != null) {
                Pair = Aligner.GetProtPair(this, MatchNumber, RefMatchNumber);
            }else{
                Pair = new ProteinPair();
                //чего можем то заполним 
                for (int i = 0 ; i < Peptides.Count ; i++){
                    if (Peptides[i].Matches[MatchNumber] != null){
                        Pair.LeftScore += Peptides[i].Matches[MatchNumber].Score;
                        Pair.NCount++;
                    }
                }
                Pair.RightScore = Pair.LeftScore;
            }
            

            string Res = 
                ((ReportString[0]!='1')?"":String.Format("\"{0}\"\t",ipi))+
                ((ReportString[1]!='1')?"":String.Format("{0:f4}\t",Pair.LeftScore))+
                ((ReportString[2]!='1')?"":String.Format("{0:f4}\t",Pair.RightScore))+
                ((ReportString[3]!='1')?"":String.Format("\"{0}\"\t",Desc))+
                ((ReportString[4]!='1')?"":(Utils.IPIListtoString(ipis))+"\t")+
                ((ReportString[5]!='1')?"":String.Format("{0}\t",Peptides.Count))+
                ((ReportString[6]!='1')?"":String.Format("{0}\t",Pair.NCount))+
                ((ReportString[7]!='1')?"":String.Format("{0:f4}\t",Pair.RSquare))+
                ((ReportString[8]!='1')?"":String.Format("{0:f4}\t",Pair.Slope))+
                ((ReportString[9]!='1')?"":String.Format("{0:f4}\t",Pair.Median))+
                ((ReportString[10]!='1')?"":String.Format("{0:f4}\t",Pair.PrAverage))+
                ((ReportString[11]!='1')?"":String.Format("{0:f4}\t",Pair.MinInterval))+
                ((ReportString[12]!='1')?"":String.Format("{0:f4}\t",Pair.MaxInterval))+
                ((ReportString[13]!='1')?"":String.Format("{0:e4}",Pair.PValue));
            return Res;
        }


        public string GetSheetString(int RefNumber){

            ProteinPair Pair;
            if (Aligner != null) {
                Pair = Aligner.GetProtPair(this, RefNumber, RefNumber);
            }else{
                Pair = new ProteinPair();
                Pair.RightScore = 0.0;
            }

            string Res = 
                ((SheetReportString[0]!='1')?"":String.Format("{0}\t",ipi))+
                ((SheetReportString[1]!='1')?"":String.Format("{0:f2}\t",Pair.RightScore))+
                ((SheetReportString[2]!='1')?"":String.Format("{0}\t",Desc))+
                ((SheetReportString[3]!='1')?"":(Utils.IPIListtoString(ipis))+"\t")+
                ((SheetReportString[4]!='1')?"":String.Format("{0}\t",Peptides.Count));

            for (int i = 0 ; i < FileList.Count ; i++){
                if (Aligner != null){
                    Pair = Aligner.GetProtPair(this, i, RefNumber);
                }else{
                    break;
                }
                Res = Res +
                    ((SheetReportString[5]!='1')?"":String.Format("{0:f5}\t",Pair.Median))+
                    ((SheetReportString[6]!='1')?"":String.Format("{0:f5}\t",Pair.Slope))+
                    ((SheetReportString[7]!='1')?"":String.Format("{0:f5}\t",Pair.PrAverage))+
                    ((SheetReportString[8]!='1')?"":String.Format("{0:f5}\t",Pair.MinInterval))+
                    ((SheetReportString[9]!='1')?"":String.Format("{0:f5}\t",Pair.MaxInterval))+
                    ((SheetReportString[10]!='1')?"":String.Format("{0:f5}\t",Pair.RSquare));
            }

            double[] RawScores = new double[FileList.Count];
            if (SheetReportString[12]=='1' || SheetReportString[1]=='1'){
                for (int i = 0 ; i < FileList.Count ; i++){
                    for (int j = 0 ; j<Peptides.Count ; j++){
                        if (Peptides[j].Matches[i] != null && 
                            Peptides[j].Matches[i].Score > 0.0){
                            if (RefNumber <0 || Peptides[j].Matches[RefNumber] != null){
                                RawScores[i] += Peptides[j].Matches[i].Score;
                            }
                        }
                    }
                }
            }

            //ppm's
            if (SheetReportString[12]=='1'){
                Res += "\t";
                for (int i = 0 ; i < FileList.Count ; i++){
                    double PPM = (RawScores[i] / FileFactors[i]) * 1000000.0;
                    Res += String.Format("{0:f5}\t",PPM);
                }
            }

            //Raw values
            if (SheetReportString[13]=='1'){
                Res += "\t";
                for (int i = 0 ; i < FileList.Count ; i++){
                    Res += String.Format("{0:f5}\t",RawScores[i]);
                }
            }

            return Res;
        }

        public string GetMatrixString(){

            if (AveScore == 0.0){
                EstimateAveScore();
            }

            string ResStr = 
                ((SheetReportString[0]!='1')?"":String.Format("{0}\t",ipi))+
                ((SheetReportString[1]!='1')?"":String.Format("{0:f2}\t",AveScore))+
                ((SheetReportString[2]!='1')?"":String.Format("{0}\t",Desc))+
                ((SheetReportString[3]!='1')?"":(Utils.IPIListtoString(ipis))+"\t")+
                ((SheetReportString[4]!='1')?"":String.Format("{0}\t",Peptides.Count));

            double[] Res;

            if (SheetReportString[11]=='1' || SheetReportString[12]=='1'){
                Res = (Aligner as MatrixAligner).GetProtRatios(this, true);
            }else{
                Res = (Aligner as MatrixAligner).GetProtRatios(this, false);
            }


            int ResZeroCount = 0;
            for (int i = 0 ; i < FileList.Count ; i++){
                if (Res[i]!=0.0){
                    ResStr += String.Format("{0:f5}\t",Res[i]);
                }else{
                    if (Settings.Default.ZeroSubst == "0.0"){
                        ResStr += "0.0\t";
                    }
                    if (Settings.Default.ZeroSubst == "1.0"){
                        ResStr += "1.0\t";
                    }
                    if (Settings.Default.ZeroSubst == "Empty"){
                        ResStr += "\t";
                    }
                    ResZeroCount++;
                }
            }
            if (ResZeroCount > Utils.GetStringValue(Settings.Default.MaxZeroes)){
                return "";
            }

            if (SheetReportString[11]=='1'){
                ResStr += "\t";
                for (int i = FileList.Count ; i < FileList.Count*2 ; i++){
                    ResStr += String.Format("{0:f5}\t",Res[i]);
                }
                ResStr += "\t";
                for (int i = FileList.Count*2 ; i < FileList.Count*3 ; i++){
                    ResStr += String.Format("{0:f5}\t",Res[i]);
                }
            }


            if (SheetReportString[12]=='1'){
                ResStr += "\t";
                for (int i = 0 ; i < FileList.Count ; i++){
                    double PPM = ((Res[i] * AveScore) / FileFactors[i]) * 1000000.0;
                    ResStr += String.Format("{0:f5}\t",PPM);
                }
            }

            if (SheetReportString[13]=='1'){
                ResStr += "\t";
                for (int i = 0 ; i < FileList.Count ; i++){
                    double Raw = Res[i] * AveScore ;
                    ResStr += String.Format("{0:f5}\t",Raw);
                }
            }
            return ResStr;
        }
    }


}