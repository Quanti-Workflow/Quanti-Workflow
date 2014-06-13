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
using System.Text;

namespace MGFParser {

    public struct Childs{
        public double Mass;
        public double Intensity;
    }
    
    public class ChildbyMass : IComparer<Childs> {
        public int Compare(Childs x, Childs y){
            if (x.Mass>y.Mass) { return 1;} 
            else if (x.Mass == y.Mass) { return 0; }
            else return -1;
        }
    }

    public class ChildbyIntensity : IComparer<Childs> {
        public int Compare(Childs x, Childs y){
            if (x.Intensity>y.Intensity) { return 1;} 
            else if (x.Intensity == y.Intensity) { return 0; }
            else return -1;
        }
    }

    public class MGFSpectrum {
        public double mz;
        public int Charge;
        public double RT;
        public double RTApex;
        public int ScanNumber;
        public int ExpNumber;
        public string Sequence;
        public string Title;
        public string Instrument;
        public List<Childs> Data;
        const int MergeNumber = 1;
        //только для смердживания 
        public double[] Parents;


        public MGFSpectrum(){
            mz = 0.0;
            Charge = 1;
            RT = 1;
            RTApex = 0.0;
            Sequence = "";
            Title = "";
            ScanNumber = 0;
            ExpNumber = 0;
            Data = null;
            Parents = new double[MergeNumber];
            Data = new List<Childs>();
        }

        public MGFSpectrum(List<string> MGFStrings):this(){
            ParseMGF(MGFStrings);
        }

        public void ParseMGF(List<string> MGFStrings){

            foreach(string s in MGFStrings){
                if (s.ToUpper().Contains("PEPMASS=")){
                    mz = Convert.ToDouble(s.Split(new char[] {' ','='})[1]);
                    continue;
                }
                if (s.ToUpper().Contains("CHARGE=")){
                    Charge = Convert.ToInt32(s.Substring(s.IndexOf('=')+1,1));
                    continue;
                }
                if (s.ToUpper().Contains("TITLE=")){
                    Title = s.Substring(s.IndexOf('=')+1);
                    try {
                        if (Title.IndexOf("FinneganScanNumber:")!=-1){
                            string Scan = Title.Substring(Title.IndexOf("FinneganScanNumber: ") + 20);
                            if (Scan.Contains(" ")) {
                                Scan = Scan.Substring(0, Scan.IndexOf(' '));
                            }
                            ScanNumber = Convert.ToInt32(Scan);
                        }
                        if (Title.IndexOf("Elution from: ")!=-1){
                            RT = Convert.ToDouble(Title.Substring(Title.IndexOf("Elution from: ")+14,7));
                        }
                        if (Title.IndexOf("experiment: ")!=-1){
                            ExpNumber = Convert.ToInt32(Title.Substring(Title.IndexOf("experiment: ")+12,1));
                        }
                        if (Title.IndexOf("RT Apex: ") != -1) {
                            string b = Title.Substring(Title.IndexOf("RT Apex: ") + 9, 7);
                            b = b.Substring(0, b.IndexOf(" "));
                            RTApex = Convert.ToDouble(b);
                        }

                    }
                    catch{};
                    continue;
                }
                if (s.ToUpper().Contains("SEQUENCE=")){
                     Sequence = s.Substring(s.IndexOf('=')+1);
                    continue;
                }

                if (s.ToUpper().Contains("RTINSECONDS=")){
                     RT = Convert.ToDouble(s.Substring(s.IndexOf('=')+1))/60;
                    continue;
                }

                if (s.ToUpper().Contains("SCANS=")){
                     ScanNumber = Convert.ToInt32(s.Substring(s.IndexOf('=')+1));
                    continue;
                }

                if (s.ToUpper().Contains("INSTRUMENT=")){
                     Instrument = s.Substring(s.IndexOf('=')+1);
                    continue;
                }


                string[] mch = s.Split(new char[] {' '});
                Childs ch = new Childs();
                ch.Mass = Convert.ToDouble(mch[0]);
                ch.Intensity = Convert.ToDouble(mch[1]);
                Data.Add(ch);
            }

            ChildbyMass cm = new ChildbyMass();
            Data.Sort(cm);

        }

        public void WriteMGF(StreamWriter sw){
            WriteMGF(sw,false);
        }

        public void WriteMGF(StreamWriter sw, bool AdvStrings){
            sw.WriteLine("BEGIN IONS");
            if (mz != 0.0 ){
                sw.WriteLine("PEPMASS={0}",mz);
            }
            if (Charge != 0 ){
                sw.WriteLine("CHARGE={0}+",Charge);
            }
            if (Title != null ){
                sw.WriteLine("TITLE={0}",Title);
            }
            if (Instrument != null){
                sw.WriteLine("INSTRUMENT={0}",Instrument);
            }
            if (AdvStrings){
                if (RT != 0.0 ){
                    sw.WriteLine("RTINSECONDS={0}",String.Format("{0:f2}",RT*60));
                }
                if (ScanNumber != 0 ){
                    sw.WriteLine("SCANS={0}",ScanNumber);
                }
                if (Parents.GetLength(0) > 0) {
                    for (int i = 0; i < Parents.GetLength(0); i++) {
                        if (Parents[i] != 0.0) {
                            sw.WriteLine("MASS{0}={1}", i + 1, Parents[i]);
                        }
                    }
                }
            }

            foreach(Childs ch in Data){
                sw.WriteLine("{0:f6} {1:f4}",ch.Mass,ch.Intensity);
            }
            sw.WriteLine("END IONS");
            sw.WriteLine();
        }

        public void WriteDTA(string FileName) {
            StreamWriter sw = new StreamWriter(FileName);
            sw.WriteLine("{0:f4} {1} ", (mz * (double)Charge) - (double)Charge+1.00785, Charge);
            foreach (Childs C in Data) {
                sw.WriteLine("{0:f4} {1:f4} ", C.Mass, C.Intensity);
            }
            sw.Close();
        }
        /*public void ChargeNorm(){
            mz = mz*Charge - (Charge-1)*H;
            Charge = 1;
        }*/

        public int FindPeak(double Mass, double Accuracy){
            int index = 0; 
            if (Data.Count == 0 ) return -1;
            for ( int k = 1 ; k < Data.Count ; k++){
                if (Math.Abs(Data[k].Mass - Mass) <  Math.Abs(Data[index].Mass - Mass) ){
                    index = k;
                }
            }
            if (Math.Abs(Data[index].Mass - Mass) > Accuracy ) {
                return -1;
            }else{
                return index;
            }
        }
    }

    public class MGFFile{

        public List<MGFSpectrum> Spectra; 

        string MGFFileName;
        public List<string> MGFComments;

        public MGFFile(){ 
            Spectra = new List<MGFSpectrum>();
            MGFComments = new List<string>();
        }

        public void MGFRead(string FileName){

            Spectra = new List<MGFSpectrum>();
            List<string> strings = new List<string>(); 
            StreamReader sr = new StreamReader(FileName);

            MGFFileName = FileName;

            MGFSpectrum Spectrum;

            while(! sr.EndOfStream){
                string str = sr.ReadLine();
                if (str.Contains("BEGIN IONS")){
                    str = sr.ReadLine();
                    while (!str.Contains("END IONS")){
                        strings.Add(str);
                        str = sr.ReadLine();
                    }
                    Spectrum = new MGFSpectrum(strings);
                    Spectra.Add(Spectrum);
                    strings.Clear();
                }
            }
        }

        public void MGFWrite(string FileName){
            MGFWrite(FileName,false);
        }

        public void MGFWrite(string FileName, bool AdvStrings){

            StreamWriter sw = new StreamWriter(FileName);

            sw.WriteLine("SEARCH=MIS \nREPTYPE=Peptide\n");

            for (int i = 0 ; i < MGFComments.Count ; i++){
                sw.WriteLine("COM=\"{0}\"",MGFComments[i]);
            }
            if (MGFComments.Count>0) sw.WriteLine();

            for (int i = 0 ; i<Spectra.Count ; i++){
                    Spectra[i].WriteMGF(sw,AdvStrings);
            }

            sw.Close();
        }

    }

}
