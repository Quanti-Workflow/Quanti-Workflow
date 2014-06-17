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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace MatrixMerge
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() != DialogResult.OK)  {
                return;
            }
            for (int i = 0 ; i < openFileDialog1.FileNames.Length ; i++){
                if (FileList.Items.IndexOf(openFileDialog1.FileNames[i]) == -1){
                    FileList.Items.Add(openFileDialog1.FileNames[i]);
                }
            }
            MakeOutName();
			//loading
			PM = new ProtMatrix[FileList.Items.Count];
			for (int i = 0 ; i < FileList.Items.Count ; i++ ){
				PM[i] = new ProtMatrix(FileList.Items[i].ToString());
			}
            //count samples
            int CountSamples = 0;
			for (int i = 0 ; i < FileList.Items.Count ; i++ ){
                CountSamples += PM[i].Samples.Count;
			}
            textBox2.Text = (CountSamples - 1).ToString();
        }

        private void DeleteButton_Click(object sender, EventArgs e){
            ListBox.SelectedObjectCollection ToDelete = FileList.SelectedItems;
            for ( int i = 0 ; i < ToDelete.Count ; i++){
                FileList.Items.Remove( ToDelete[i]);
            }
            FileList.Items.Remove(FileList.SelectedItem);
            MakeOutName();
        }

        private void MakeOutName(){
            OutFileBox.Text = Path.GetDirectoryName(openFileDialog1.FileNames[0]) + Path.DirectorySeparatorChar;
            for (int i = 0 ; i < FileList.Items.Count ; i++){
                OutFileBox.Text += Path.GetFileNameWithoutExtension(FileList.Items[i].ToString());
            }
            OutFileBox.Text += "_Merged.txt";
        }

		public class ProtMatrix{
			public List<Prot> Proteins;
			public List<string> Samples;

			public ProtMatrix(){
				Proteins = new List<Prot>();
				Samples = new List<string>();
			}

			public ProtMatrix(string FileName){
				Proteins = new List<Prot>();
				Samples = new List<string>();
				Load(FileName);
			}

			public void Load(string ProtMatrFile){
				//отбрасываем заголовки 
				StreamReader sr = new StreamReader(ProtMatrFile);
				List<string> tokens = new List<string>();
				while (!sr.EndOfStream){
					string Caption = sr.ReadLine();
					tokens = Caption.Split(new char[] {'\t'}).ToList();
					if (tokens.Count > 2) break;
				}
				int[] Delims = new int[5]; 
				Delims[0] = tokens.IndexOf("PROTEIN ID");
				Delims[1] = tokens.IndexOf("DESCRIPTION");
				Delims[2] = tokens.IndexOf("PEPTIDES");
				Delims[3] = tokens.IndexOf("PROTEIN IDs");
				Delims[4] = tokens.IndexOf("REF ABUNDANCE");
				int Shift = 5;
				for (int i = 0 ; i < 5 ; i++){
					if (Delims[i] == -1){
						Shift--;
					}
				}
				int Count = tokens.Count - Shift;
				for ( int i = 0 ; i < Count ; i++ ){
					if (tokens[i+Shift] == ""){
						Count = i;
						break;
					}
					Samples.Add(tokens[i+Shift]);
				}
				while (!sr.EndOfStream){
					string str = sr.ReadLine();
					tokens = str.Split(new char[] {'\t'}).ToList();
					Prot P = new Prot();
					if (Delims[0]!=-1){
						P.ID = tokens[Delims[0]];
					}
					if (Delims[1]!=-1){
						P.Desc = tokens[Delims[1]];
					}
					if (Delims[2]!=-1){
						P.Peptides = Convert.ToInt32(tokens[Delims[2]]);
					}
					if (Delims[3]!=-1){
						P.IPIS = (tokens[Delims[3]].Split(new char[] {';'})).ToList();
					}
					if (Delims[4]!=-1){
						P.RefAbunds = Convert.ToDouble(tokens[Delims[4]]);
					}
					P.Abunds = new List<double>();
					for ( int i = 0 ; i < Count ; i++){
                        //if empty
                        if (tokens[i+Shift] == ""){
                            P.Abunds.Add(0.0);
                            continue;
                        }
                        //if 1
                        if (Convert.ToDouble(tokens[i+Shift]) == 1.0){
                            //if all alternativbe are 0.0 - then 1.0 otherwise 0.0
                            bool flag = true;
        					for ( int j = 0 ; j < Count ; j++){
                                if (i!=j){
                                    flag &= Convert.ToDouble(tokens[i + Shift]) == 0.0;
                                }
                            }
                            P.Abunds.Add(flag?1.0:0.0);
                        }else{
                            //if normal value 
						    P.Abunds.Add(Convert.ToDouble(tokens[i+Shift]));
                        }
					}
					Proteins.Add(P);
				}
			}

			public void Save(StreamWriter sw){
				sw.Write("PROTEIN ID\tREF ABUNDANCE\tDESCRIPTION\tPROTEIN IDs\tPEPTIDES\t");
				for (int i = 0 ; i < Samples.Count-1 ; i++ ){
					sw.Write("{0}\t",Samples[i]);
				}
				sw.WriteLine(Samples[Samples.Count-1]);
				for (int i = 0 ; i < Proteins.Count ; i++){
					string IPIS = "";
					for(int j = 0 ; j < Proteins[i].IPIS.Count-1; j++){
						IPIS += Proteins[i].IPIS[j]+";";
					}
					IPIS += Proteins[i].IPIS[Proteins[i].IPIS.Count-1];
					sw.Write("{0}\t{1}\t{2}\t{3}\t{4}\t",Proteins[i].ID,Proteins[i].RefAbunds,Proteins[i].Desc,IPIS,Proteins[i].Peptides);
					for (int j = 0 ; j < Samples.Count-1 ; j++ ){
						sw.Write("{0}\t",Proteins[i].Abunds[j]);
					}
					sw.WriteLine(Proteins[i].Abunds[Samples.Count-1]);
				}
			}
		}

        public class Prot {
            public string ID;
            public string Desc;
			public double RefAbunds;
			public List<String> IPIS;
			public int Peptides;
            public List<double> Abunds;

			public Prot Clone(){
				Prot P = new Prot();
				P.ID = ID;
				P.Desc = Desc;
				P.RefAbunds = RefAbunds;
				P.Peptides = Peptides;
				P.IPIS = new List<string>();
				P.IPIS.AddRange(IPIS);
				P.Abunds = new List<double>();
				P.Abunds.AddRange(Abunds);
				return P;
			}
        }

		static public bool Cross(List<string> A , List<string> B){
			foreach(string S1 in A){
				foreach(string S2 in B){
					if (string.Compare(S1,S2) == 0){
						return true;
					}
				}
			}
			return false;
		}

        ProtMatrix[] PM;

        private void GoButton_Click(object sender, EventArgs e)
        {
			//merge by IPIs
			ProtMatrix Merged = new ProtMatrix();
			//samples
			for (int i = 0 ; i < FileList.Items.Count ; i++ ){
				Merged.Samples.AddRange(PM[i].Samples);
			}
            //make a proterin list 
            List<List<string>> ProteinsIPIs = new List<List<string>>();
            if (radioButton1.Checked){
                //skip proteins
    			for (int i = 0 ; i<PM[0].Proteins.Count ; i++ ){
                    List<string> PIPIs = PM[0].Proteins[i].IPIS;
                    bool GlobalFlag = true;
    				for (int j = 1 ; j < PM.GetLength(0) ; j++ ){
                        bool flag = false;
    					foreach(Prot PS in PM[j].Proteins){
    						if (Cross(PIPIs,PS.IPIS)){
                                flag = true;
    							break;
                            }
                        }
                        GlobalFlag &= flag;
                    }
                    if (GlobalFlag) {
                        ProteinsIPIs.Add(PIPIs);
                    }
                }
            }else{
                //keep proteins
    			for (int k = 0 ; k < PM.GetLength(0) ; k++ ){
            		for (int i = 0 ; i<PM[k].Proteins.Count ; i++ ){
                        bool flag = false;
    					foreach(List<string> PS in ProteinsIPIs){
    						if (Cross(PM[k].Proteins[i].IPIS,PS)){
                                flag = true;
    							break;
                            }
                        }
                        if (!flag){
                            ProteinsIPIs.Add(PM[k].Proteins[i].IPIS);
                        }
                    }
                }
            }
			//proteins
			for (int i = 0 ; i<ProteinsIPIs.Count ; i++ ){
				List<string> PL = ProteinsIPIs[i];
                Prot P = null;
				for (int j = 0 ; j < PM.GetLength(0) ; j++ ){
                    bool flag = false;
					foreach(Prot PS in PM[j].Proteins){
						if (Cross(PL,PS.IPIS)){
                            flag = true;   
                            if (P == null){
                                P = PS.Clone();
                                //leading zeroes
                                for (int k = 0; k < j ; k++){
                                    foreach (string S in PM[k].Samples){
                                        P.Abunds.Insert(0,0.0);
                                    }
                                }
                            }else{
    							P.Abunds.AddRange(PS.Abunds);
                            }
							break;
						}
					}
                    if (P!=null && !flag){
                        foreach (string S in PM[j].Samples){
                            P.Abunds.Add(0.0);
                        }
                    }
				}
				if (P.Abunds.Count == Merged.Samples.Count){
                    //maximum zero filtering 
                    int ZeroCount = 0;
                    for (int j = 0; j < P.Abunds.Count; j++){
                        ZeroCount += P.Abunds[j] == 0.0 ? 1 : 0;
                    }
                    if (ZeroCount < Convert.ToInt32(textBox2.Text)){
                        Merged.Proteins.Add(P);
                    }
				}
			}
			//writing
    		StreamWriter sw = new StreamWriter(OutFileBox.Text);
            //Caption
            sw.WriteLine(Text);
            sw.WriteLine("Original Fies:");
            for (int i = 0; i < FileList.Items.Count; i++ ){
                sw.WriteLine(FileList.Items[i].ToString());
            }
            sw.WriteLine("Unique proteins has {0}; Maximum zeroes {1}.", radioButton1.Checked ? "skipped" : "kept", textBox2.Text);
            //Matrix
            Merged.Save(sw);
            sw.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(saveFileDialog1.ShowDialog() != DialogResult.OK)  {
                return;
            }
            OutFileBox.Text = saveFileDialog1.FileName;
        }

    }
}
//TODO - Zeroes can be 0,1.0,empty
