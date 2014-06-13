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
using System.Diagnostics;
using System.Threading;
using Raw2MGF.Properties;

namespace RawToMGF {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, EventArgs e) {
            if(openRAWDialog.ShowDialog() != DialogResult.OK)  {
                return;
            }
            for (int i = 0 ; i < openRAWDialog.FileNames.Length ; i++){
                bool Flag = false;
                for (int j = 0; j < RawList.Items.Count; j++ ){
                    if ( RawList.Items[j].Text == openRAWDialog.FileNames[i]) {
                        Flag = true;
                        break;
                    }
                }

                if (!Flag){
                    ListViewItem LItem = new ListViewItem();
                    LItem.Text = openRAWDialog.FileNames[i];
                    FileInfo fi = new FileInfo(openRAWDialog.FileNames[i]);
                    LItem.SubItems.Add((fi.Length/1000000).ToString());
                    LItem.SubItems.Add("--");
                    LItem.SubItems.Add("--");
                    LItem.SubItems.Add("Not processed yet.");
                    RawList.Items.Add(LItem);
                }
            }
            progressBar1.Value = 0;
            if (openRAWDialog.FileNames.Length>0){
                OutMgfBox.Text = Path.GetDirectoryName(openRAWDialog.FileNames[0]);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e) {
            ListView.SelectedListViewItemCollection ToDelete = RawList.SelectedItems;
            for ( int i = 0 ; i < ToDelete.Count ; i++){
                RawList.Items.Remove( ToDelete[i]);
            }
            RawList.Items.Remove(RawList.FocusedItem);
            progressBar1.Value = 0;
        }

        private void SaveMGFbutton_Click(object sender, EventArgs e) {
            if(folderBrowserDialog1.ShowDialog() != DialogResult.OK)  {
                return;
            }
            OutMgfBox.Text = folderBrowserDialog1.SelectedPath;
        }

        int SpCount = 0;

        ProcessDriver[] PDrivers;
        List<string> Files;

        private void DoWork() {

            try {
                //копируем список файлов
                Files = new List<string>();
                for (int i = 0 ; i < RawList.Items.Count ; i++){
                    //если файлы еще не обработаны
                    Files.Add(RawList.Items[i].Text);
                }
                //Создаем процессы вместе с прогресс-индикаторами
                CreateProcesses(Files.Count);
                Processed = 0;
                NextAvialable = 0;
                timer1.Enabled = true;
            }
            catch(Exception e){
                Log(e.Message,MessageBoxIcon.Error,e.StackTrace);
            }

        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            Settings.Default.Save();
        }

        private void Doubles_Validating(object sender, CancelEventArgs e) {
            try{
                Convert.ToDouble((sender as TextBox).Text);
            }
            catch{
                (sender as TextBox).Text = "0";
            }
        }

        private void Ints_Validating(object sender, CancelEventArgs e) {
            try{
                Convert.ToInt32((sender as TextBox).Text);
            }
            catch{
                (sender as TextBox).Text = "0";
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
             //this.CanFocus = true;
            LockControls(false);
            label9.Text = label9.Text + "; Processing completed.";
            progressBar1.Value = 0;
            SystemSounds.Beep.Play();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            DoWork();
        }

        private void GoButton_Click(object sender, EventArgs e) {
            Settings.Default.Save();
            LockControls(true);
            //backgroundWorker1.RunWorkerAsync();
            DoWork();
            //backgroundWorker1_RunWorkerCompleted(null, null);
        }


        private void LockControls(bool Lock){
            GoButton.Enabled = ! Lock;
            AddButton.Enabled = ! Lock;
            DeleteButton.Enabled = ! Lock;
            OutMgfBox.Enabled = ! Lock;
            SaveMGFbutton.Enabled = ! Lock;
            textBox1.Enabled = ! Lock;
            textBox2.Enabled = ! Lock;
            textBox3.Enabled = ! Lock;
            textBox4.Enabled = ! Lock;
            textBox5.Enabled = ! Lock;
            textBox6.Enabled = ! Lock;
            textBox7.Enabled = ! Lock;
            checkBox9.Enabled = ! Lock;
            checkBox10.Enabled = !Lock;
            checkBox1.Enabled = !Lock;
            RTApexCheck.Enabled = !Lock;
        }

		class ProcessDriver {
			public Label PLabel;
			public ProgressBar PBar;
			public Process Proc;
			Form1 TForm;
			public int Progress;
            public string FileName;
			public string ErrorMessage;
            public int MSMSCount;
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

			public void StartProcess(string Params){
				Progress = 0;
				ErrorMessage = "";
                this.FileName = Params.Substring(1,Params.IndexOf("\" ")-1);
                Proc = new Process();
                ProcessStartInfo PSI = new ProcessStartInfo(Environment.CurrentDirectory + "\\RawtoMGFConsole.exe");
                PSI.UseShellExecute = false;
                PSI.RedirectStandardOutput = true;
                PSI.RedirectStandardInput = true;
                PSI.CreateNoWindow = true;
                PSI.Arguments = Params;
                PSI.WorkingDirectory = Environment.CurrentDirectory;
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
                        int MS = Convert.ToInt32(outLine.Data.Substring(outLine.Data.IndexOf(" "),outLine.Data.LastIndexOf(" ")-outLine.Data.IndexOf(" ")));
                        MSMSCount = Convert.ToInt32(outLine.Data.Substring(outLine.Data.LastIndexOf(" ")));
                        TForm.RawList.BeginUpdate();
                        TForm.SetFileSpectra(FileName, MS, MSMSCount);
                        TForm.SetTotalSpectra();
                        TForm.RawList.EndUpdate();
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

            RawList.Height -= PosShift*PCount;
            label8.Top -= PosShift*PCount;
            OutMgfBox.Top -= PosShift*PCount;
            SaveMGFbutton.Top -= PosShift*PCount;
            checkBox1.Top -= PosShift*PCount;
            groupBox2.Top -= PosShift*PCount;
            groupBox3.Top -= PosShift*PCount;
            DeleteButton.Top -= PosShift*PCount;
            AddButton.Top -= PosShift*PCount;
            label9.Top -= PosShift*PCount;
            RTApexCheck.Top -= PosShift*PCount;
            progressBar1.Top -= PosShift*PCount;
            
            int i;
            for ( i = 0 ; i < PCount ; i++ ){
				PDrivers[i] = new ProcessDriver();
				PDrivers[i].CreateControls(this,12,progressBar1.Top+37+PosShift*i);
            }

            this.PerformLayout();
            return PCount;
        }

        int Processed;
        int NextAvialable;

        private void RunProcesses(){
            int PCount = PDrivers.GetLength(0);               

            //проверяем есть ли свободные процессы 
            if (NextAvialable < Files.Count){
                for(int i = 0 ; i < PCount ; i++){
                    if ( PDrivers[i].Proc == null ){
                        SetFileStatus(Files[NextAvialable], FileStatus.Processing);
                        //Параметры RawtoMGFConsole
                        //args[0] - имя Raw-файла
                        //args[1] - имя MGF-файла
                        //args[2] - minimal MZ
                        //args[3] - maximal MZ
                        //args[4] - minimal RT
                        //args[5] - maximal RT
                        //args[6] - minimal Charge
                        //args[7] - maximal Charge
                        //args[8] - number of top peaks 
                        //args[9] - CleanETD:yes / CleanETD:no
                        //args[10] - Instrument:yes / Instrument:no
                        //дополнительно (к версии 2.0.6)
                        //args[11] - CheckSpectra:yes / CheckSpectra:no
                        string Params = "\""+Files[NextAvialable]+"\" ";
                        string OutFile = OutMgfBox.Text+Path.DirectorySeparatorChar+Path.ChangeExtension(Path.GetFileName(Files[NextAvialable]), "mgf");
                        Params += "\""+OutFile + "\" " + textBox1.Text + " " + textBox2.Text + " " + textBox4.Text + " " + textBox3.Text + " ";
                        Params += textBox5.Text + " " + textBox7.Text + " " + textBox6.Text + " ";
                        Params += (checkBox10.Checked ? "CleanETD:yes " : "CleanETD:no ") + 
                            (checkBox9.Checked ? "Instrument:yes " : "Instrument:no ") + 
                            (checkBox1.Checked ? "CheckSpectra:yes " : "CheckSpectra:no ") + 
                            (RTApexCheck.Checked ? "RTApex:yes " : "RTApex:no ");
                        PDrivers[i].StartProcess(Params);
                        NextAvialable++;
                        break;
                    }
                }
                //return;
            }else{
                for(int i = 0 ; i < PCount ; i++){
                    if ( PDrivers[i].Proc == null && PDrivers[i].PLabel != null && PDrivers[i].Finished){
                        DeleteProcess(i);
                    }
                }
            }
            //проверяем - есть ли завершенные процессы
            for(int i = 0 ; i < PCount ; i++){
                if (PDrivers[i].Proc!=null && PDrivers[i].Proc.HasExited && PDrivers[i].Finished){
					if ( PDrivers[i].Proc.ExitCode==0){
                        SetFileStatus(PDrivers[i].FileName,FileStatus.Processed);
                        Log("File "+PDrivers[i].FileName+" was successfully processed. Driver - "+i.ToString());
					}else{
                        if (PDrivers[i].ErrorMessage == ""){
                            continue;
                        }else{
                            SetFileStatus(PDrivers[i].FileName,FileStatus.Failed,PDrivers[i].ErrorMessage);
                        }
					}
                    PDrivers[i].Proc = null;
                    SpCount += PDrivers[i].MSMSCount;
                    PDrivers[i].MSMSCount = 0;
                    Processed++;
                    Log(String.Format("Processed - {0}",Processed));
                }
            }
       		//total progress bar
            progressBar1.Value = (int)(((double)Processed / (double)Files.Count) * 100.0);
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
            GoButton.Top -= PosShift;

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
            string Message = null;
            switch (Status) {
                case FileStatus.Neutral: {
                    Message = "File is not processed yet.";
                    break;
                }
                case FileStatus.Processing: {
                    Message = "File is being processed...";
                    break;
                }
                case FileStatus.Processed: {
                    Message = "Processed. Fine.";
                    break;
                }
                case FileStatus.Failed: {
                    Message = "Failed.";
                    break;
                }
            }
            SetFileStatus(FileName, Status, Message);
        }


        private void SetFileStatus(string FileName, FileStatus Status, string Message){
            for (int i = 0 ; i < RawList.Items.Count ; i++ ){
                if (   RawList.Items[i].Text == FileName ){
			        ListViewItem LItem = RawList.Items[i];
                    string Size = LItem.SubItems[1].Text;
                    string MS   = LItem.SubItems[2].Text;
                    string MSMS = LItem.SubItems[3].Text;
			        LItem.SubItems.Clear();
			        LItem.Text = FileName;
                	LItem.SubItems.Add(Size);
                	LItem.SubItems.Add(MS);
                	LItem.SubItems.Add(MSMS);
                	LItem.SubItems.Add(Message);
                    switch (Status) {
                        case FileStatus.Neutral: {
                            break;
                        }
                        case FileStatus.Processing: {
                			LItem.BackColor = Color.LightGoldenrodYellow;
                            break;
                        }
                        case FileStatus.Processed: {
                			LItem.BackColor = Color.FromArgb(214,253,200);
                            break;
                        }
                        case FileStatus.Failed: {
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

        private void SetFileSpectra(string FileName, int MSCount, int MSMSCount){
            for (int i = 0 ; i < RawList.Items.Count ; i++ ){
                if (   RawList.Items[i].Text == FileName ){
			        ListViewItem LItem = RawList.Items[i];
                    string Size = LItem.SubItems[1].Text;
                    string Message = LItem.SubItems[4].Text;
                    Color Col = LItem.BackColor;
			        LItem.SubItems.Clear();
			        LItem.Text = FileName;
                	LItem.SubItems.Add(Size);
                	LItem.SubItems.Add(MSCount.ToString());
                	LItem.SubItems.Add(MSMSCount.ToString());
                	LItem.SubItems.Add(Message);
                    LItem.BackColor = Col;
			        RawList.Items.RemoveAt(i);
			        RawList.Items.Insert(i,LItem);
			        RawList.EnsureVisible(i);                
                }
            }
        }

        private void SetTotalSpectra(){
            int TotSpectra = SpCount;
            foreach(ProcessDriver P in PDrivers){
                TotSpectra += P.MSMSCount;
            }
            label9.Text = "Total MS/MS spectra: " + TotSpectra.ToString();
        }

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
            string FileName = OutMgfBox.Text+Path.DirectorySeparatorChar+"RawtoMgf.log";
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
            Application.DoEvents();
        }

        private void timer1_Tick(object sender, EventArgs e){
            try{
                timer1.Enabled = false;
                if (Processed<Files.Count){
                    RunProcesses();
                }else{
                //удаляем последний процесс
                    int PCount = PDrivers.GetLength(0);
                    int FCount = PCount;
                    for(int i = 0 ; i < PCount ; i++){
                        if ( PDrivers[i].Proc == null && PDrivers[i].PLabel != null && PDrivers[i].Finished){
                            DeleteProcess(i);
                        }else{
                            FCount--;
                        }
                    }
                    if (FCount == 0){
                        backgroundWorker1_RunWorkerCompleted(null, null);
                        return;
                    }
                }
                timer1.Enabled = true;
            }
            catch(Exception ex){
                Log(ex.Message,MessageBoxIcon.Error,ex.StackTrace);
                timer1.Enabled = false;
            }
        }

    }
}

// TODO - перетащить обработку resize