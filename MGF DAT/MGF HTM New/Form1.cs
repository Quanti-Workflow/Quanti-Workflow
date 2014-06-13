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
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Media;
using MGF_HTM_New.Properties;



namespace MGF_HTM_New {

    public partial class Form1 : Form {

        Mascot.MascotParser mp;

        MGFParser.MGFFile MGFin;
        MGFParser.MGFFile MGFout;

        string MGFFileName;

        public Form1() {
            InitializeComponent();
        }

        //Интерфейс 
        private void DATFileButton_Click(object sender, EventArgs e) {
            if(DatFileDialog.ShowDialog() != DialogResult.OK)  {
                return;
            }
            DatFileName.Text = DatFileDialog.FileName;
            mp = new Mascot.MascotParser();
            mp.ParseFile(DatFileDialog.FileName);

            //проверяем MGF файл 
            if (File.Exists(mp.MGFFileName)){
                MGFFileName = mp.MGFFileName;
                MGFRead(MGFFileName);
            }

            bool Reversed = false;
            for (int i = 0 ; i < mp.Proteins.Count ; i++){
                if (mp.Proteins[i].Name.ToUpper().Contains("REVERSED")){
                    Reversed = true;
                    break;
                }
            }
            if (Reversed){
                radioButton2.Enabled = true;
            }else{
                radioButton2.Enabled = false;
                radioButton1.Checked = true;
            }

        }

        private void MGFinFileButton_Click(object sender, EventArgs e) {
            if(MGFFileDialog.ShowDialog() != DialogResult.OK)  {
                return;
            }
            MGFRead(MGFFileDialog.FileName);
        }


        private void MGFRead(string FileName){
            MGFInFileName.Text = FileName;
            MGFin = new MGFParser.MGFFile();
            MGFin.MGFRead(FileName);
            MGFoutFileName.Text = Path.GetDirectoryName(FileName)+Path.DirectorySeparatorChar+
                Path.GetFileNameWithoutExtension(FileName)+"-filtered.mgf";
        }

        private void MGFoutFileButton_Click(object sender, EventArgs e) {
            if(saveFileDialog1.ShowDialog() != DialogResult.OK)  {
                return;
            }
            MGFoutFileName.Text = saveFileDialog1.FileName;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            MGF_HTM_New.Properties.Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e) {
            MGF_HTM_New.Properties.Settings.Default.Save();
            //button1.Enabled = false;
            //backgroundWorker1.RunWorkerAsync();
            backgroundWorker1_DoWork(null,null);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            button1.Enabled = true;
            progressBar1.Value = 0;
            if (e.Error != null){
                MessageBox.Show(e.Error.Message);
            }
            SystemSounds.Beep.Play();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            progressBar1.Value = e.ProgressPercentage;
            label4.Text = "Spectra number: "+MGFout.Spectra.Count.ToString();
        }



        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            //BackgroundWorker worker = sender as BackgroundWorker;
            int PercentCompleted = 0;
            
            if ( mp==null ){
                if (File.Exists(DatFileName.Text) ){
                    mp = new Mascot.MascotParser();
                    mp.ParseFile(DatFileName.Text);
                }else{
                    throw new ArgumentException(
                        "Mascot .DAT file must be specified", "n");
                }
            }
            if ( MGFin == null){
                if (File.Exists(DatFileName.Text) ){
                    MGFRead(MGFInFileName.Text);
                }else{
                    throw new ArgumentException(
                        "MGF file must be specified", "n");
                }
            }

            if (!Settings.Default.MScore){
                //сортируем по score
                MascotSpectrabyScore mss = new MascotSpectrabyScore();
                mp.Spectra.Sort(mss);
                int DirectCount = 0, ReversedCount = 0, i;
                for (i = 0 ; i < mp.Spectra.Count ; i++){
                    if (mp.Spectra[i].Peptides.Count == 0) 
                        break;
                    bool Reversed = true;
                    for (int j = 0 ; j < mp.Spectra[i].Peptides[0].ProteinNames.Length ; j++ ){
                        Reversed &= mp.Spectra[i].Peptides[0].ProteinNames[j].ToUpper().Contains("REVERSED");
                    }
                    if (Reversed){
                        ReversedCount++;
                    }else{
                        DirectCount++;
                    }
                    double FDRRatio = ((double)ReversedCount/(double)DirectCount)*100.0;
                    if (FDRRatio > Convert.ToDouble(Settings.Default.FDRThreshold)) 
                        break;
                }
                Settings.Default.Mascot_Threshold = Convert.ToString(mp.Spectra[i].Peptides[0].Score);
            }

            
            Mascot.MascotSpectra ms;
            MGFParser.MGFSpectrum mgfs;
            MGFout = new MGFParser.MGFFile();

            MGFout.MGFComments.AddRange(MGFin.MGFComments);
            MGFout.MGFComments.Add(String.Format("Filtered by {0};",Text));
            MGFout.MGFComments.Add(String.Format("Mascot Threshold - {0}",Settings.Default.Mascot_Threshold));
            if (!Settings.Default.MScore){
                MGFout.MGFComments.Add(String.Format("Based on FDR Threshold - {0}%",Settings.Default.FDRThreshold));
            }
            MGFout.MGFComments.Add(String.Format("Mascot Result File - {0}",DatFileName.Text));

            for (int i = 0 ; i < MGFin.Spectra.Count ; i++ ){
                mgfs = MGFin.Spectra[i];
                //Link to Mascot search results - не работает - мы только что пересортировалми спектры 
                //ms = mp.AccessByFSN(mgfs.ScanNumber);
                ms = null;
                for (int j = 0 ; j < mp.Spectra.Count ; j++){
                    if (mp.Spectra[j].ScanNumber == mgfs.ScanNumber){
                        ms = mp.Spectra[j];
                        break;
                    }
                }
                if (ms == null){
                    throw new ArgumentException(
                        "Probably .dat file does not correspond to .mgf file", "n");
                }

                if ( ms.Peptides.Count==0 || 
                     ms.Peptides[0].Score < Convert.ToDouble(Settings.Default.Mascot_Threshold)) {
                    continue;
                }
                bool Reversed = true;
                for (int j = 0 ; j < ms.Peptides[0].ProteinNames.Length ; j++ ){
                    Reversed &= ms.Peptides[0].ProteinNames[j].ToUpper().Contains("REVERSED");
                }
                if (Reversed) continue;

                if (mgfs.Data.Count == 0){
                    continue;
                }
                MGFout.Spectra.Add(mgfs);
                if (Convert.ToInt32(((i*100)/MGFin.Spectra.Count)) > PercentCompleted ) {
                    PercentCompleted = Convert.ToInt32(((i*100)/MGFin.Spectra.Count));
                    backgroundWorker1.ReportProgress(PercentCompleted);
                }
            }
            backgroundWorker1.ReportProgress(100);
            MGFout.MGFWrite(Settings.Default.MGF_Out);
            backgroundWorker1.ReportProgress(0);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e) {
            if (radioButton1.Checked){
                textBox1.Enabled = true;
                textBox2.Enabled = false;
            }else{
                textBox1.Enabled = false;
                textBox2.Enabled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
            if (radioButton1.Checked){
                textBox1.Enabled = true;
                textBox2.Enabled = false;
                radioButton2.Checked = false;
            }else{
                textBox1.Enabled = false;
                textBox2.Enabled = true;
                radioButton2.Checked = true;
            }
        }
    }
    public class MascotSpectrabyScore : IComparer<Mascot.MascotSpectra> {
        public int Compare(Mascot.MascotSpectra x, Mascot.MascotSpectra y){
            if (x.Peptides.Count == 0 && y.Peptides.Count == 0 ) {return 0;}
            if (x.Peptides.Count == 0 ) {return 1;}
            if (y.Peptides.Count == 0 ) {return -1;}
            if (x.Peptides[0].Score < y.Peptides[0].Score) {return 1;}
            if (x.Peptides[0].Score > y.Peptides[0].Score) {return -1;}
            if (x.Peptides[0].Score == y.Peptides[0].Score) {return 0;}
            return 0;
        }
    }

}

//TO DO 
//- индикация прогресса 
//- Проверка установок и загрузка файлов при взлете - DONE
