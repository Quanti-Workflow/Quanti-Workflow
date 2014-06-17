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
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Media;
using System.Diagnostics;
using Quanty.Properties;
using Mascot;

namespace Quanty {
    public interface ILogAndProgress {
        void Log(string Message);
        void Log(string Message, System.Windows.Forms.MessageBoxIcon WarrningLevel, string StackInfo);
        void ProgressMessage(string Message);
        void RepProgress(int Perc);
        void RepProgress(int HowMatch, int From);
    }


    public partial class Form1 : Form, Quanty.ILogAndProgress {

        MascotParser mp;

        public Form1() {
            InitializeComponent();
        }

        StreamReader tabsr;

        //event handlers
        private void DATFileButton_Click(object sender, EventArgs e) {
            if(DatFileDialog.ShowDialog() != DialogResult.OK)  {
                return;
            }
            DatFileName.Text = DatFileDialog.FileName;
            if (File.Exists(Path.GetDirectoryName(DatFileDialog.FileName)+Path.DirectorySeparatorChar+Path.GetFileNameWithoutExtension(DatFileDialog.FileName)+".db3")){
                if (MessageBox.Show("There is a .db3 file for selected file. \n This file contains processing results "+
                                    "and will be overwriten.\n Are you sure you want to overwrite this file?","Result file already exsists",MessageBoxButtons.OKCancel,MessageBoxIcon.Exclamation) == DialogResult.Cancel){
                    DatFileName.Text="";
                    return; 
                };
            }

            if (DatFileDialog.FilterIndex == 1){
                //Mascots files
                mp = new Mascot.MascotParser();
                try{
                    mp.ParseFile(DatFileDialog.FileName);
                }
                catch (Exception ex){
                    MessageBox.Show(ex.Message,
                        "Incorrect .Dat file.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DatFileName.Text="";
                    return ;
                }
                //меняем маскот-scan на Order - если таковой имеется
                for (int i = 0; i < mp.Spectra.Count; i++) {
                    if (mp.Spectra[i].Title.Contains("Order")) {
                        string FSN = mp.Spectra[i].Title.Substring(mp.Spectra[i].Title.IndexOf("Order")+11);
                        FSN = FSN.Substring(0, FSN.IndexOf("Finnegan")-3);
                        mp.Spectra[i].ScanNumber = Convert.ToInt32(FSN);
                    }
                }
                //проверяем  - Finnigan ScanNumber должен быть уникальным после этой процедуры 
                mp.Spectra.Sort(new MascotsbyFSN());
                bool Flag = false;
                for (int i = 0 ; i < mp.Spectra.Count-1 ; i++){
                    Flag = (mp.Spectra[i].ScanNumber == mp.Spectra[i + 1].ScanNumber);
                    if (Flag) break;
                }
                if (Flag){
                    MessageBox.Show(".Dat file does not provide unique Order or ScanNumber. \n Please, Check .MGF used for Mascot Search.",
                        "Incorrect .Dat file.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DatFileName.Text="";
                    return ;
                }

                AllCheckBox.Enabled = true;

                UseFDRCheck.Enabled = mp.IsFDRAvialable();
                UseFDRCheck.Checked = false;
                UseFDRCheck.Text = "Use FDR mascot thres.";
                FDR = 0.0;
                tabsr = null;
            }else{
                //Text files
                tabsr = new StreamReader(DatFileName.Text);
                if (!MascotProc.FillMSMSLists(ref Mascots, ref MSMSList, ref Proteins, tabsr)){
                    tabsr.Close();
                    tabsr = null;
                    return;
                }
                mp = null;

                for (int i = 0; i < Proteins.Count; i++ ){
                    if (Proteins[i].ipi.ToUpper().Contains("REVERSED")){
                        UseFDRCheck.Enabled = true;
                    }else{
                        UseFDRCheck.Enabled = false;
                    }
                    UseFDRCheck.Checked = false;
                    UseFDRCheck.Text = "Use FDR mascot thres.";
                    FDR = 0.0;

                    AllCheckBox.Checked = false;
                    AllCheckBox.Enabled = false;

                }
                tabsr.Close();
            }
            OutPathName.Text = Path.GetDirectoryName(DatFileName.Text);
            folderBrowserDialog1.SelectedPath = Path.GetDirectoryName(DatFileName.Text);
            DBFileName.Text = OutPathName.Text+Path.DirectorySeparatorChar+Path.GetFileNameWithoutExtension(DatFileName.Text)+".db3";
        }

        private void dbOpenButton_Click(object sender, EventArgs e) {
            if(DBFileDialog.ShowDialog() != DialogResult.OK)  {
                return;
            }
            OutPathName.Text = Path.GetDirectoryName(DBFileDialog.FileName);
            DBFileName.Text = DBFileDialog.FileName;
            if ( db == null ) {
                db = new DBInterface();
            }
            db.ConnectTo(DBFileName.Text);
            LoadSettings();
            LoadFileList();
            LockForDB(true);
            LoadData();
        }


        private void Form1_Load(object sender, EventArgs e) {

            //if (File.Exists(Settings.Default.DatFileName)){
            //    DatFileName.Text = Settings.Default.DatFileName;
            //    mp = new Mascot.MascotParser();
            //    mp.ParseFile(Settings.Default.DatFileName);
            //    OutPathName.Text = Path.GetDirectoryName(DatFileName.Text);
            //    folderBrowserDialog1.SelectedPath = Path.GetDirectoryName(DatFileName.Text);
            //}else{
            //    DatFileName.Text = "";
            //}
            DatFileName.Text = "";
            DBFileName.Text = "";
            ReferenceFile = "None";
            LoadReportVector();
            TimeColumn.Width = 75;
            MessageHeader.Width = LogList.Width-100;
            RefCombo.SelectedIndex = 0;
            ZeroSubstBox.SelectedIndex = 0;
            LockRefFeatures(true);
            RefNumber = -1;
            db = null;
            DBInterface.iLog = this;
            RawList_ClientSizeChanged(null, null);
            LogList_ClientSizeChanged(null, null);
        }

        private void RawList_ClientSizeChanged(object sender, EventArgs e){
            FileName.Width = 500;
            Message.Width = RawList.Width-520;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            SaveReportVector();
            Settings.Default.Save();
        }

        private void OutPathButton_Click(object sender, EventArgs e) {
            if(folderBrowserDialog1.ShowDialog() != DialogResult.OK)  {
                return;
            }
            OutPathName.Text = folderBrowserDialog1.SelectedPath;
            DBFileName.Text = OutPathName.Text+Path.DirectorySeparatorChar+Path.GetFileNameWithoutExtension(DatFileName.Text)+".db3";
        }

        private void AllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!AllCheckBox.Checked){
                UseAllCheck.Checked = false;
                UseAllCheck.Enabled = false;
            }else{
                UseAllCheck.Enabled = true;
            }
        }


        public void RepProgress(int Perc){
            RepProgress(Perc, 100);
        }

        public void RepProgress(int HowMatch, int From){
            if (HowMatch == 0){
                progressBar1.Value=0;
                Application.DoEvents();
                return;
            }
            int Progr = (int)(((double)HowMatch / (double)From) * 100.0);
            if (Progr!=progressBar1.Value){
                progressBar1.Value=Progr;
            }
            Application.DoEvents();
        }

        public void ProgressMessage(string Message){
            ProgressLabel.Text = Message;
            Application.DoEvents();
        }

        int RefNumber;
        DBInterface db;

        ProcessDriver[] PDrivers;

        //Главная кнопка
        private void DoWork() {

            //а я не знаю почему оно само туда не лезет
            MascotProc.iLog = this;
            Settings.Default.DatFileName = DatFileName.Text;
            LockFDR();

            //Разбор Mascot
            RepProgress(0);
            if (mp!=null && db == null){
                Log("Parsing and grouping Mascot results:");
                MascotProc.FillMSMSLists(ref Mascots, ref MSMSList, ref Proteins, mp);
            }
            //или переразбор текстового файла
            if (tabsr!=null && db == null){
                tabsr = new StreamReader(DatFileName.Text);
                MascotProc.FillMSMSLists(ref Mascots, ref MSMSList, ref Proteins, tabsr);
                tabsr.Close();
            }


            //сохранение разбора маскот в базу данных
            String dbName = DBFileName.Text;
            if (db == null || db.GetFileName()!=DBFileName.Text){
                db = new DBInterface();
                db.CreateNewDB(dbName);
                SaveSettingsDB();
                db.SaveProteins(Proteins);
                db.SaveMSMS(Mascots,true);
                if (Settings.Default.QuantiAll){
                    db.SaveMSMS(MSMSList,false);
                }else{
                    MSMSList = Mascots;
                }
            }


            //MSMSList = Mascots;
            RepProgress(0);

            try {

                //копируем список файлов
                Files = new List<string>();
                for (int i = 0 ; i < RawList.Items.Count ; i++){
                    //если файлы еще не обработаны
                    if (!(RawList.Items[i].BackColor == Color.FromArgb(214,253,200))){
                        Files.Add(RawList.Items[i].Text);
                    }
                }
                //Создаем процессы вместе с прогресс-индикаторами
                CreateProcesses(Files.Count);
                if (Files.Count>0){
                    InitMatches();
                }
                NextAvialable = 0;
                Processed = 0;
                if (Files.Count > 0){ //запускаем обработку сырых файлов 
                    timer1.Enabled = true;
                }else{ //или сразу формируем отчет
                    ReportWork();
                    WorkCompleted(null);
                }
            }
            catch(Exception e){
                Log(e.Message,MessageBoxIcon.Error,e.StackTrace);
            }

        }

        private int CreateProcesses(int NumberOfTasks){

            int PCount = 0;
            try{
                foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
                {
                    PCount += int.Parse(item["NumberOfCores"].ToString());
                }
            }catch(Exception e){
                Log("Unable to figure out number of cores - run in 2 core mode",MessageBoxIcon.Warning,null);
                PCount = 2;
            }


            PCount = Math.Min(PCount, NumberOfTasks);
            //int PCount = 4; //Environment.ProcessorCount;
            //вывешиваем progress bars
			PDrivers = new ProcessDriver[PCount];

            int PosShift = 38;
            this.ResumeLayout(false);
            this.Height += PosShift*PCount;

            tabControl1.Height -= PosShift*PCount;
            label3.Top -= PosShift*PCount;
            OutPathName.Top -= PosShift*PCount;
            OutPathButton.Top -= PosShift*PCount;
            groupBox1.Top -= PosShift*PCount;
            groupBox2.Top -= PosShift*PCount;
            groupBox3.Top -= PosShift*PCount;
            groupBox4.Top -= PosShift*PCount;
            ProgressLabel.Top -= PosShift*PCount;
            progressBar1.Top -= PosShift*PCount;
            
            int i;
            for ( i = 0 ; i < PCount ; i++ ){
				PDrivers[i] = new ProcessDriver();
				PDrivers[i].CreateControls(this,12,progressBar1.Top+37+PosShift*i);
            }

            this.PerformLayout();
            return PCount;
        }

        int NextAvialable;
        int Processed;
        List<string> Files;

        private void RunProcesses(){
            int PCount = PDrivers.GetLength(0);               

            //проверяем есть ли свободные процессы 
            if (NextAvialable < Files.Count){
                for(int i = 0 ; i < PCount ; i++){
                    if ( PDrivers[i].Proc == null ){
                        SetFileStatus(Files[NextAvialable], FileStatus.Processing);
                        PDrivers[i].StartProcess(DBFileName.Text,Files[NextAvialable],db.GetNextDbNumber());
                        NextAvialable++;
                        break;
                    }
                }
            }else{
                for(int i = 0 ; i < PCount ; i++){
                    if ( PDrivers[i].Proc == null && PDrivers[i].PLabel != null && PDrivers[i].Finished ){
                        DeleteProcess(i);
                    }
                }
            }
            for(int i = 0 ; i < PCount ; i++){
                if (PDrivers[i].Proc!=null && PDrivers[i].Proc.HasExited && PDrivers[i].Finished){
					if ( PDrivers[i].Proc.ExitCode==0){
                        SetFileStatus(PDrivers[i].FileName,FileStatus.Processed);
                        Log("File "+PDrivers[i].FileName+" was successfully processed.");
					}else{
                        if (PDrivers[i].ErrorMessage == ""){
                            continue;
                        }else{
                            SetFileStatus(PDrivers[i].FileName,FileStatus.Failed);
                        }
					}
                    PDrivers[i].Proc = null;
                    Processed++;
                }
            }
       		//total progress bar
            RepProgress(Processed,Files.Count);

        }

        private void DeleteProcess(int PNumber){
            int PosShift = 38;
            if (PDrivers[PNumber] == null || PDrivers[PNumber].Proc != null ) return;
            this.SuspendLayout();
            for (int i = 0 ; i<PDrivers.GetLength(0) ; i++){
                if (PDrivers[i].PLabel == null) continue;
                if (PDrivers[i].PBar.Top <= PDrivers[PNumber].PBar.Top) continue;
                PDrivers[i].PBar.Top -= PosShift; 
                PDrivers[i].PLabel.Top -= PosShift; 
            }
            this.Height -= PosShift;
           /*tabControl1.Height += PosShift;
            label3.Top += PosShift;
            OutPathName.Top += PosShift;
            OutPathButton.Top += PosShift;
            groupBox1.Top += PosShift;
            groupBox2.Top += PosShift;
            groupBox3.Top += PosShift;
            ProgressLabel.Top += PosShift;
            progressBar1.Top += PosShift;*/

            button1.Top -= PosShift;

            PDrivers[PNumber].RemoveControls();
            this.ResumeLayout(false);
        }

        enum FileStatus{
            Neutral,
            Processing,
            Processed,
            Failed
        }

        private void SetFileStatus(string FileName, FileStatus Status){
            for (int i = 0 ; i < RawList.Items.Count ; i++ ){
                if (   RawList.Items[i].Text == FileName ){
			        ListViewItem LItem = RawList.Items[i];
			        LItem.SubItems.Clear();
			        LItem.Text = FileName;
                    switch (Status) {
                        case FileStatus.Neutral: {
                			LItem.SubItems.Add("File is not processed yet.");
                            break;
                        }
                        case FileStatus.Processing: {
                			LItem.SubItems.Add("File is being processed...");
                			LItem.BackColor = Color.LightGoldenrodYellow;
                            break;
                        }
                        case FileStatus.Processed: {
                			LItem.SubItems.Add("Processed. Fine.");
                			LItem.BackColor = Color.FromArgb(214,253,200);
                            break;
                        }
                        case FileStatus.Failed: {
                            //здесь должно быть сообщение об ошибке 
                			LItem.SubItems.Add("Failed.");
			                LItem.BackColor = Color.LightCoral;
                            break;
                        }
                    }
			        RawList.Items.RemoveAt(i);
			        RawList.Items.Insert(i,LItem);
			        RawList.EnsureVisible(i);                
                }
            }
        }

        List<QPeptide> MSMSList;
        List<QPeptide> Mascots;
        List<QProtein> Proteins;

        public delegate void LogDelegate(string Message, MessageBoxIcon WarrningLevel, string StackInfo);
 
        public void Log(string Message){
            Log(Message,MessageBoxIcon.Information,null);
        }

        public void Log(string Message,MessageBoxIcon WarrningLevel,string StackInfo){
           if (InvokeRequired) {
                // We're not in the UI thread, so we need to call BeginInvoke
                Invoke(new LogDelegate(Log), new object[]{Message,WarrningLevel,StackInfo});
                return;
            }
            //from forms thread
            DateTime date = DateTime.Now;
            string TimeString = date.ToString("H:mm:ss.fff");

            //to file 
            string FileName = OutPathName.Text+Path.DirectorySeparatorChar+"Quanty.log";
            StreamWriter sw = new StreamWriter(FileName,true);
            string FileMessage = "Info:";
            if (WarrningLevel == MessageBoxIcon.Warning) FileMessage = "Warning!"; 
            if (WarrningLevel == MessageBoxIcon.Error)   FileMessage = "ERROR!"; 
            FileMessage += "\t"+TimeString;
            FileMessage += "\t"+Message;
            if (StackInfo != null){
                FileMessage += "\n StackInfo:"+StackInfo;
            }
            sw.WriteLine(FileMessage);
            sw.Close();


            //to form 
            ListViewItem LItem = new ListViewItem();
            ProgressLabel.Text = Message;
            LItem.Text = TimeString;
            LItem.SubItems.Add(Message);
            if (StackInfo != null){
                LItem.SubItems.Add(StackInfo);
            }
            LItem.ToolTipText = LItem.Text;
            if (WarrningLevel == MessageBoxIcon.Warning){
                LItem.BackColor = Color.Yellow;
            }
            if (WarrningLevel == MessageBoxIcon.Error){
                LItem.BackColor = Color.Red;
                ErrorCounter++;
            }
            LogList.Items.Add(LItem);
            LogList.EnsureVisible(LogList.Items.Count-1);
            ProgressLabel.Text=Message;
            Application.DoEvents();
        }


        int ErrorCounter;

        private void WorkCompleted(Exception e) {
            if (e != null && e.Message != null){
                Log(e.Message,MessageBoxIcon.Error,e.StackTrace);
                ProgressLabel.Text = "Processing aborted! See log for details...";
                ProgressLabel.BackColor = Color.Red;
            }else{
                if (ErrorCounter == 0){
                    ProgressLabel.Text = "Processing complete!";
                }else{
                    ProgressLabel.Text = string.Format("Processing complete with {0} errors! See log for details...",ErrorCounter);
                    ProgressLabel.BackColor = Color.Red;
                }
            }
            progressBar1.Value = 0;
            LockControls(false);
            LockForDB(true);
            SystemSounds.Beep.Play();
        }

        private void button1_Click(object sender, EventArgs e) {
            if (RefNumber != -1) {
                //string temp = RawList.Items[0].ToString();
                //RawList.Items[0] = RawList.Items[RefNumber];
                //RawList.Items[RefNumber] = temp;
                //RefNumber = 0;
            }
            LockControls(true);
            ErrorCounter = 0;
            ProgressLabel.BackColor = this.BackColor;
            DoWork();
        }

        private void LockControls(bool Lock){
            AddRawButton.Enabled = !Lock;
            DeleteRawButton.Enabled = !Lock;
            button1.Enabled = ! Lock;
            LockForDB(Lock);
            if (Lock){
                LockRefFeatures(Lock);
            }else{
                RefCombo_TextChanged(null,null);
            }
        }

        private void LockForDB(bool Lock){
            DATFileButton.Enabled = !Lock;
            OutPathButton.Enabled = !Lock;
            OutPathName.Enabled = ! Lock;
            textBox1.Enabled = !Lock;
            textBox2.Enabled = !Lock;
            textBox3.Enabled = !Lock;
            textBox4.Enabled = !Lock;
            textBox5.Enabled = !Lock;
            textBox8.Enabled = !Lock;
            checkBox1.Enabled = !Lock;
            checkBox5.Enabled = !Lock;
            AllCheckBox.Enabled = !Lock;
            MetaCheckBox.Enabled = !Lock;
            StickCheckBox.Enabled = !Lock;
            textBox9.Enabled = !Lock;
            textBox10.Enabled = !Lock;
            textBox11.Enabled = !Lock;
        }

        private void LockRefFeatures(bool Lock){
            textBox6.Enabled = !Lock;
            checkBox2.Enabled = !Lock;
            checkBox3.Enabled = !Lock;
            checkBox4.Enabled = !Lock;
            UseAllCheck.Enabled = !Lock && AllCheckBox.Checked;
            ZeroSubstBox.Enabled = !Lock;
            MaxZeroBox.Enabled = !Lock;
        }


        private void AddRawButton_Click(object sender, EventArgs e) {
            if (RAWFileDialog.ShowDialog() != DialogResult.OK)  {
                return;
            }
            for (int i = 0 ; i < RAWFileDialog.FileNames.Length ; i++){
                bool Flag = false;
                for (int j = 0; j < RawList.Items.Count; j++ ){
                    if ( RawList.Items[j].Text == RAWFileDialog.FileNames[i]) {
                        Flag = true;
                        break;
                    }
                }
                if (!Flag){
                    ListViewItem LItem = new ListViewItem();
                    LItem.Text = RAWFileDialog.FileNames[i];
                    LItem.SubItems.Add("Not processed yet.");
                    RawList.Items.Add(LItem);
                }
            }
            
            RefCombo.Items.Clear();
            RefCombo.Items.Add("Matrix");
            RefCombo.Items.Add("None");
            for ( int i = 0 ; i < RawList.Items.Count ; i++){
                RefCombo.Items.Add(RawList.Items[i].Text);
            }
            RefCombo.Text = "Matrix";
            ReferenceFile = "Matrix";
            MaxZeroBox.Text = Convert.ToString(RawList.Items.Count - 1);
            //RecalcButton.Enabled = false;
            progressBar1.Value = 0;
        }


        private void DeleteRawButton_Click(object sender, EventArgs e) {
            ListView.SelectedListViewItemCollection ToDelete = RawList.SelectedItems;

            //проверка на наличие уже обработанных
            bool Flag = false;
            for ( int i = 0 ; i < ToDelete.Count ; i++){
                if ( ToDelete[i].BackColor == Color.FromArgb(214,253,200)  && !Flag){
                    Flag = (MessageBox.Show("You are going to delete from the list already processed file(s).\n"+
                                    "All quantification results for this file(s) will be lost.\n"+
                                    "Are you sure you wish to delete this file(s)?","Warning",
                                    MessageBoxButtons.OKCancel,MessageBoxIcon.Exclamation) == DialogResult.OK);
                    if (!Flag) return;
                }
            }
            if (RawList.FocusedItem.BackColor == Color.FromArgb(214,253,200) && !Flag){
                Flag = (MessageBox.Show("You are going to delete from the list already processed file(s).\n"+
                                "All quantification results for this file(s) will be lost.\n"+
                                "Are you sure you wish to delete this file(s)?","Warning",
                                MessageBoxButtons.OKCancel,MessageBoxIcon.Exclamation) == DialogResult.OK);
                if (!Flag) return;
            }

            //цикл удаления из базы данных 
            for ( int i = 0 ; i < ToDelete.Count ; i++){
                if (ToDelete[i].BackColor == Color.FromArgb(214,253,200) ){
                    db.DeleteFile(RawList.Items.IndexOf(ToDelete[i]));
                }
            }
            if (RawList.FocusedItem.BackColor == Color.FromArgb(214,253,200)){
                db.DeleteFile(RawList.Items.IndexOf(RawList.FocusedItem));
            }

            //если данные были удалены из базы данных - то следует перезагрузка всех данных 
            if (Flag){
                LoadFileList();
                LockForDB(true);
                LoadData();
            }


            //цикл удаления из списка для обработки 
            for ( int i = 0 ; i < ToDelete.Count ; i++){
                RawList.Items.Remove( ToDelete[i]);
            }
            RawList.Items.Remove(RawList.FocusedItem);

            //цикл пересоставления возможных ссылочных файлов            
            progressBar1.Value = 0;
            RefCombo.Items.Clear();
            RefCombo.Items.Add("Matrix");
            RefCombo.Items.Add("None");
            for ( int i = 0 ; i < RawList.Items.Count ; i++){
                if (RawList.Items[i].Text[0] == '+'){
                    RefCombo.Items.Add(RawList.Items[i].Text.Substring(1));
                }else{
                    RefCombo.Items.Add(RawList.Items[i].Text);
                }
            }
            RefCombo.Text = "Matrix";
            ReferenceFile = "Matrix";
            MaxZeroBox.Text = Convert.ToString(RawList.Items.Count - 1);
            //RecalcButton.Enabled = false;
        }

        private void LogList_ClientSizeChanged(object sender, EventArgs e) {
            TimeColumn.Width = 75;
            MessageHeader.Width = LogList.Width-100;
        }

        private void LogSaveButton_Click(object sender, EventArgs e) {
            saveLogFileDialog.InitialDirectory = OutPathName.Text;
            if (saveLogFileDialog.ShowDialog() != DialogResult.OK)  {
                return;
            }
            string FileName = saveLogFileDialog.FileName;
            StreamWriter sw = new StreamWriter(FileName);
            for(int i = 0; i < LogList.Items.Count ; i++ ){
                string Label = "Info:";
                if (LogList.Items[i].BackColor == Color.Yellow) Label = "Warning!"; 
                if (LogList.Items[i].BackColor == Color.Red)    Label = "ERROR!"; 
                sw.WriteLine(Label+"\t"+LogList.Items[i].Text+"\t"+LogList.Items[i].SubItems[1].Text);
                if (LogList.Items[i].SubItems.Count > 2){
                    sw.WriteLine(LogList.Items[i].SubItems[2].Text);
                }
            }
            sw.Close();

        }

        QAligner Aligner;
        
        private void ReportWork(){
            
            Log("Report generation started...");
            Prints Print = new Prints();

            List<string> Files = new List<string>();

            for (int i = 0 ; i < RawList.Items.Count ; i++){
                if (RawList.Items[i].Text.Contains("{Sample")){
                    Files.Add(RawList.Items[i].Text);
                }else{
                    Files.Add(Path.GetFileName(RawList.Items[i].Text));
                }
            }
            Print.Init(Files);

            List<QPeptide> TempMasc = null;
            string PeptExt = "Pept";
            if (Settings.Default.UseAll){
                TempMasc = Mascots;
                Mascots = MSMSList;
                PeptExt = "All";
            }

            if (Settings.Default.Matrix){
                QAligner.Log = this;
                Aligner = new MatrixAligner(Mascots, Proteins, Files.Count);
                QPeptide.Aligner = Aligner;
                QProtein.Aligner = Aligner;
                RepProgress(0);
                Aligner.RunFast();
                Proteins.Sort(new QProtein.byIPI());
                Mascots.Sort(new QPeptide.bySet());
                QProtein.FileFactorsCalc(Proteins);
                Log("Report writing...");
                Print.PrintMatrix(Print.ForPrinting(Mascots),"_"+PeptExt+"Matrix.txt");
                Print.PrintMatrix(Print.ForPrinting(Proteins),"_ProtMatrix.txt");
            }else{
                if (RefNumber < 0 ){
                        //никаких сравнений - вывод как есть
                    QPeptide.Aligner = null;
                    QProtein.Aligner = null;
                }else{
                    QAligner.Log = this;
                    Aligner = new RefAligner(Mascots, Proteins, Files.Count,RefNumber);
                    QPeptide.Aligner = Aligner;
                    QProtein.Aligner = Aligner;
                    RepProgress(0);
                    Aligner.RunFast();
                }
                QProtein.FileFactorsCalc(Proteins);
                for (int i = 0 ; i < RawList.Items.Count ; i++){
                    Proteins.Sort(new QProtein.byIPI());
                    Mascots.Sort(new QPeptide.bySet());
                    Print.PrintFile(Print.ForPrinting(Mascots), i, RefNumber, "."+PeptExt);
                    Print.PrintFile(Print.ForPrinting(Proteins), i, RefNumber, ".prot");
                }
                //if (RefNumber>=0){
                    if (PrintAllPeptides)
                        Print.PrintSheet(Print.ForPrinting(Mascots), RefNumber, "."+PeptExt+"sht");
                    if (PrintAllProteins)
                        Print.PrintSheet(Print.ForPrinting(Proteins), RefNumber, ".prosht");
                //}
            }
            if (Settings.Default.UseAll){
                Mascots = TempMasc ;
            }

            Log("Reporting session completed.");
        }

        string ReferenceFile;

        private void RefCombo_TextChanged(object sender, EventArgs e) {
            if (String.Compare(RefCombo.Text,"Matrix") == 0){
                Settings.Default.Matrix = true;
                LockRefFeatures(false);
                return;
            }else{
                Settings.Default.Matrix = false;
            }
            for (int i = 0 ; i<RawList.Items.Count ; i++){
                //RecalcButton.Enabled = true;
                if (  String.Compare(RefCombo.Text,RawList.Items[i].Text) == 0
                    ||String.Compare(RefCombo.Text,RawList.Items[i].Text.Substring(1)) == 0 ){
                    ReferenceFile = RefCombo.Text;
                    RefNumber = i;
                    LockRefFeatures(false);
                    return;
                }
            }
            LockRefFeatures(true);
            RefNumber = -1;
        }

        private bool PrintPeptides;
        private bool PrintProteins;
        private bool PrintAllPeptides;
        private bool PrintAllProteins;
        private bool RepSettingsLoading;

        private void SaveReportVector(){
            Settings.Default.ReportVector = "";
            //что печатаем
            for (int i = 0 ; i < ReportTree.Nodes.Count ; i++){
                Settings.Default.ReportVector += (ReportTree.Nodes[i].Checked)?"1":"0";
            }
            //каков набор колонок
            for (int i = 0 ; i < ReportTree.Nodes.Count ; i++){
                for (int j = 0 ; j <ReportTree.Nodes[i].Nodes.Count ; j++){
                    Settings.Default.ReportVector += (ReportTree.Nodes[i].Nodes[j].Checked)?"1":"0";
                }
            }
            PrintPeptides = (Settings.Default.ReportVector[0] == '1');
            PrintProteins = (Settings.Default.ReportVector[1] == '1');
            PrintAllPeptides = (Settings.Default.ReportVector[2] == '1');
            PrintAllProteins = (Settings.Default.ReportVector[3] == '1');
        }

        private void LoadReportVector(){
            RepSettingsLoading = true;
            int Count = 0;
            for (int i = 0 ; i < ReportTree.Nodes.Count  ; i++){
                if ( Count < Settings.Default.ReportVector.Length ){
                    ReportTree.Nodes[i].Checked = (Settings.Default.ReportVector[Count] == '1');
                    Count++;
                }
            }
            for (int i = 0 ; i < ReportTree.Nodes.Count  ; i++){
                for (int j = 0 ; j < ReportTree.Nodes[i].Nodes.Count  ; j++){
                    if ( Count < Settings.Default.ReportVector.Length ){
                        ReportTree.Nodes[i].Nodes[j].Checked = (Settings.Default.ReportVector[Count] == '1');
                        Count++;
                    }
                }
            }
            RepSettingsLoading = false;
            SaveReportVector();
        }

        private void ReportTree_AfterCheck(object sender, TreeViewEventArgs e) {
            if (!RepSettingsLoading){
                SaveReportVector();
            }
        }

        private void SaveSettingsDB(){
            List<string> Names = new List<string>();
            List<string> Values = new List<string>();
            Names.Add("Quanty version:");
            Values.Add(Text);
            Names.Add("DatFileName");
            Values.Add(DatFileName.Text);
            Names.Add("MascotScoreThres");
            Values.Add(textBox1.Text);
            Names.Add("MassError");
            Values.Add(textBox2.Text);
            Names.Add("RTPercents");
            Values.Add(textBox3.Text);
            Names.Add("RTMinutes");
            Values.Add(textBox4.Text);
            Names.Add("MinimumPeptides");
            Values.Add(textBox5.Text);
            Names.Add("ChargeDeconvo");
            Values.Add(checkBox1.Checked?"Yes":"No");
            Names.Add("UseBestPept");
            Values.Add(checkBox5.Checked?"Yes":"No");
            Names.Add("QuantiAll");
            Values.Add(AllCheckBox.Checked?"Yes":"No");
            Names.Add("StickMode");
            Values.Add(StickCheckBox.Checked?"Yes":"No");

            Names.Add("MaxRTWidth");
            Values.Add(textBox9.Text);

            Names.Add("MinRTWidth");
            Values.Add(textBox10.Text);

            Names.Add("RTRes");
            Values.Add(textBox11.Text);
            Names.Add("MetaProfile");
            Values.Add(MetaCheckBox.Checked?"Yes":"No");
            Names.Add("RTOrder");
            Values.Add(textBox8.Text);
            Names.Add("FDR");
            Values.Add(String.Format("{0:f2}",FDR));
            db.SaveSettings(Names,Values);
        }

        private void LoadSettings(){
            List<string> Names = new List<string>();
            List<string> Values = new List<string>();
            db.LoadSettings(ref Names, ref Values);
            DatFileName.Text = Values[Names.IndexOf("DatFileName")];
            textBox1.Text = Values[Names.IndexOf("MascotScoreThres")];
            textBox2.Text = Values[Names.IndexOf("MassError")];
            textBox3.Text = Values[Names.IndexOf("RTPercents")];
            textBox4.Text = Values[Names.IndexOf("RTMinutes")];
            textBox5.Text = Values[Names.IndexOf("MinimumPeptides")];
            checkBox1.Checked = Values[Names.IndexOf("ChargeDeconvo")] == "Yes";
            checkBox5.Checked = Values[Names.IndexOf("UseBestPept")] == "Yes";
            AllCheckBox.Checked = Values[Names.IndexOf("QuantiAll")] == "Yes";
            try {
                StickCheckBox.Checked = Values[Names.IndexOf("StickMode")] == "Yes";
                MetaCheckBox.Checked = Values[Names.IndexOf("MetaProfile")] == "Yes";
                textBox9.Text = Values[Names.IndexOf("MaxRTWidth")];
                textBox10.Text = Values[Names.IndexOf("MinRTWidth")];
                textBox11.Text = Values[Names.IndexOf("RTRes")];
            }catch(Exception e){
                StickCheckBox.Checked = false;
                MetaCheckBox.Checked = false;
                textBox9.Text = "0.0";
                textBox10.Text = "0.0";
                textBox11.Text = "0";
            }

            if (Names.IndexOf("RTOrder")!=-1) {
                textBox8.Text = Values[Names.IndexOf("RTOrder")];
            }else{
                textBox8.Text = "30";
            }
            if (Names.IndexOf("FDR")!=-1) {
                FDR = Convert.ToDouble(Values[Names.IndexOf("FDR")]);
                if (FDR > 0.0){
                    UseFDRCheck.Checked = true;
                    UseFDRCheck.Text = String.Format("Use FDR = {0:f2}%)",FDR);
                }else{
                    UseFDRCheck.Checked = false;
                    UseFDRCheck.Text = "Use FDR mascot thres.";
                }
            }
        }

        private void  LoadFileList(){
            List<string> FileNames = null;
            db.LoadFileList(ref FileNames);
            RawList.Items.Clear();
            for (int i = 0 ; i<FileNames.Count ; i++ ){
                ListViewItem LItem = new ListViewItem();
                LItem.Text = FileNames[i];
                LItem.SubItems.Add("Loaded.");
			    LItem.BackColor = Color.FromArgb(214,253,200);
                RawList.Items.Add(LItem);
            }
            RefCombo.Items.Clear();
            RefCombo.Items.Add("Matrix");
            RefCombo.Items.Add("None");
            for ( int i = 0 ; i < RawList.Items.Count ; i++){
                RefCombo.Items.Add(RawList.Items[i].Text);
            }
            RefCombo.Text = "Matrix";
            ReferenceFile = "Matrix";
            MaxZeroBox.Text = Convert.ToString(RawList.Items.Count - 1);
            //RecalcButton.Enabled = true;
            progressBar1.Value = 0;
        }

        private void LoadData(){
            Proteins = new List<QProtein>();
            db.LoadProteins(Proteins);
            Mascots = new List<QPeptide>();
            if (Settings.Default.QuantiAll ){
                db.LoadMascots(Proteins,Mascots);
                MSMSList = new List<QPeptide>();
                db.LoadAllMSMS(MSMSList);
                //а теперь сливаем Mascots и MSMSList
                Mascots.Sort(new QPeptide.byMascotSN());
                MSMSList.Sort(new QPeptide.byMascotSN());
                int Count=0;
                for ( int i = 0 ; i< Mascots.Count ; i++){
                    while( MSMSList[Count].MascotScan != Mascots[i].MascotScan) {
                        Count++;
                    }
                    MSMSList[Count] = Mascots[i];
                }
            }else{
                db.LoadMascots(Proteins,Mascots);
                MSMSList = Mascots;
            }
            InitMatches();
            SetProtReference();
            db.LoadQMatches(MSMSList);
            //db.LoadMSMatches(MSMSList);
            mp = null;
            Log("Loading completed");
            RepProgress(0);

        }

        private void InitMatches(){
            if (MSMSList != null){
                for (int i = 0 ; i < MSMSList.Count ; i++ ){
                    MSMSList[i].Matches = new QMatch[RawList.Items.Count];
                }
            }
        }

        private void SetProtReference(){
            for( int i = 0 ; i < Mascots.Count ; i++ ){
                for (int j = 0 ; j < Proteins.Count ; j++){
                    if (Mascots[i].IPI == Proteins[j].ipi){
                        Mascots[i].PartOfProtein = Proteins[j];
                        break;
                    }
                }
            }
        }

		class ProcessDriver {
			public Label PLabel;
			public ProgressBar PBar;
			public Process Proc;
			Form1 TForm;
			public int Progress;
            public string FileName;
			public string ErrorMessage;
            public StreamWriter Input;
            public bool Finished;

			public ProcessDriver(){
				PLabel = new Label();
				PBar = new ProgressBar();
				Proc = null;
			}

			public void CreateControls(Form1 TargetForm, int X, int Y){
				TForm = TargetForm;
				PLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
                PLabel.AutoSize = true;
                PLabel.Location = new System.Drawing.Point(X+7, Y);
                PLabel.Name = "Plabel"+Convert.ToString(X*Y);
                PLabel.Size = new System.Drawing.Size(83, 13);
                PLabel.Text = "Thread progress";

                TargetForm.Controls.Add(PLabel);

                PBar = new ProgressBar();
                PBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                            | System.Windows.Forms.AnchorStyles.Right)));
                PBar.Location = new System.Drawing.Point(X, Y+19);
                PBar.Name = "PBar"+Convert.ToString(X*Y);
                PBar.Size = new System.Drawing.Size(TargetForm.Width-40, 15);

                TargetForm.Controls.Add(PBar);
			}

            public void RemoveControls(){
                TForm.Controls.Remove(PLabel);
                TForm.Controls.Remove(PBar);
                PLabel = null;
                PBar = null;
            }

			public void StartProcess(string DBName, string FileName,int FileNumber){
				Progress = 0;
				ErrorMessage = "";
                this.FileName = FileName;
                Proc = new Process();
                ProcessStartInfo PSI = new ProcessStartInfo(Environment.CurrentDirectory + "\\QuantiProcess\\QuantiProcess.exe");
                PSI.UseShellExecute = false;
                PSI.RedirectStandardOutput = true;
                PSI.RedirectStandardInput = true;
                PSI.CreateNoWindow = true;
                PSI.Arguments = "\""+DBName+"\" \""+FileName+"\" "+FileNumber.ToString();
                PSI.WorkingDirectory = Environment.CurrentDirectory + "\\QuantiProcess";
				PLabel.Text = FileName;
                Proc.StartInfo = PSI;
				Proc.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                Finished = false;
                Proc.Start();
                Input = Proc.StandardInput;
				Proc.BeginOutputReadLine();
                Input.WriteLine(" ");
			}

			public void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)	{
				// Collect the sort command output.
			   if (TForm.InvokeRequired) {
					// We're not in the UI thread, so we need to call BeginInvoke
					TForm.Invoke( new DataReceivedEventHandler(OutputHandler), new object[]{sendingProcess,outLine});
					return;
				}
				if (!String.IsNullOrEmpty(outLine.Data)){
					int Perc;
					if (outLine.Data.Contains("%...")){
                        Perc = Convert.ToInt32(outLine.Data.Substring(0,outLine.Data.IndexOf("%...")));
                        if (PBar != null){
						    PBar.Value = Perc;
                        }
						Progress = Perc; 
					}else{
						//PLabel.Text = outLine.Data;
						//error propcessing
                        if (outLine.Data.IndexOf("Completed") != -1){
                            Input.WriteLine(" ");
                            Finished = true;
                            PLabel.Text = FileName + " " + outLine.Data;
                        }
                        if (outLine.Data.IndexOf("Information:") == 0){
                            if (PLabel != null){
                                PLabel.Text = FileName + " " + outLine.Data.Substring(12);
                            }
                            TForm.Log(FileName + " " + outLine.Data.Substring(12));
                        }
                        if (outLine.Data.IndexOf("Warning:") == 0){
                            if (PLabel != null){
                                PLabel.Text = FileName + " " + outLine.Data.Substring(8);
                            }
                            TForm.Log(FileName + " " + outLine.Data.Substring(8),MessageBoxIcon.Warning,null);
                        }
                        if (outLine.Data.IndexOf("Error:") == 0){
                            int StackInfoPos = outLine.Data.IndexOf("STACKINFO:");
                            if (StackInfoPos == -1 ){
                                if (PLabel != null){
                                    PLabel.Text = FileName + " " + outLine.Data.Substring(6);
                                }
                                TForm.Log(FileName + " " + outLine.Data.Substring(6),MessageBoxIcon.Error,null);
                            }else{
                                if (PLabel != null){
                                    PLabel.Text = FileName + " " + outLine.Data.Substring(6,StackInfoPos-6);
                                }
                                TForm.Log(FileName + " " + outLine.Data.Substring(6,StackInfoPos-6),MessageBoxIcon.Error,outLine.Data.Substring(StackInfoPos+10));
                            }
    						ErrorMessage = outLine.Data;
                        }
					}
				}
			}
		}

        private void UseFDRCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (UseFDRCheck.Checked){
                if (UseFDRCheck.Enabled){
                    label5.Text = "Mascot FDR thres %>=";
                    textBox1.Text = "0";
                }
            }else{
                label5.Text = "Mascot Score >=";
                FDR = 0.0;
            }
        }

        double FDR;
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (UseFDRCheck.Checked && UseFDRCheck.Enabled){
                FDR = Utils.GetStringValue(textBox1.Text);
                if (FDR > 0.0 && FDR < 100.0){
                    double MScore = mp.MascotThresForFDR(FDR);
                    UseFDRCheck.Text = String.Format("Use FDR mascot thres. = {0:f2}",MScore);
                }
            }
        }

        private void LockFDR(){
            UseFDRCheck.Enabled = false;
            if (UseFDRCheck.Checked && mp!=null){
                textBox1.Text = String.Format("{0:f2}", mp.MascotThresForFDR(FDR));
                label5.Text = "Mascot Score >=";
                UseFDRCheck.Text = String.Format("Use FDR = {0:f2}%",FDR);
            }
        }

        private void timer1_Tick(object sender, EventArgs e){
            try{
                timer1.Enabled = false;
                if (Processed<Files.Count){
                    RunProcesses();
                }else{
                //удаляем последний процесс
                    int PCount = PDrivers.GetLength(0);               
                    for(int i = 0 ; i < PCount ; i++){
                        if ( PDrivers[i].Proc == null && PDrivers[i].PLabel != null && PDrivers[i].Finished){
                            DeleteProcess(i);
                        }
                    }
                    //загружаемся и запускаем оставшуюся обработку
                    if (Files.Count > 0 ){
                        db.LoadQMatches(MSMSList);
                        if (Settings.Default.CutTails){
                            db.LoadMSMatches(MSMSList);
                        }
                    }
                    ReportWork();
                    WorkCompleted(null);
                    return;
                }
                timer1.Enabled = true;
            }
            catch(Exception ex){
                WorkCompleted(ex);
                Log(ex.Message,MessageBoxIcon.Error,ex.StackTrace);
            }
        }
    }
}


//TODO:
//Матрица несовместности в cluster MGF - если значения в ней достаточно велики - выравниваения не произошло 
//организровать объект Alignment - а вернее матрицу таких объектов - в этот объект перенести процедуру LongESIalignment. а также хранение и вычисление 
//всевозможных матриц построение этого объекта можно пустить в паралель.