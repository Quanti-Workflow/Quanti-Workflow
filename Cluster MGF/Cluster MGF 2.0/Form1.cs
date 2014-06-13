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
using System.Threading;
using MGFParser;
using Cluster_MGF.Properties;

namespace Cluster_MGF {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, EventArgs e) {
            if(openMGFDialog.ShowDialog() != DialogResult.OK)  {
                return;
            }
            for (int i = 0 ; i < openMGFDialog.FileNames.Length ; i++){
                if (MGFlist.Items.IndexOf(openMGFDialog.FileNames[i]) == -1){
                    MGFlist.Items.Add(openMGFDialog.FileNames[i]);
                }
            }
            progressBar1.Value = 0;
            //default name and place for output file
            OutMgfBox.Text = Path.GetDirectoryName(openMGFDialog.FileNames[0]) + Path.DirectorySeparatorChar +  "Clustered.mgf";
            
        }

        private void DeleteButton_Click(object sender, EventArgs e) {
            ListBox.SelectedObjectCollection ToDelete = MGFlist.SelectedItems;
            for ( int i = 0 ; i < ToDelete.Count ; i++){
                MGFlist.Items.Remove( ToDelete[i]);
            }
            MGFlist.Items.Remove(MGFlist.SelectedItem);
            progressBar1.Value = 0;
        }

        private void SaveMGFbutton_Click(object sender, EventArgs e) {
            if(saveMGFDialog.ShowDialog() != DialogResult.OK)  {
                return;
            }
            OutMgfBox.Text = saveMGFDialog.FileName;
        }

        struct Aligner{
            public MGFSpectrum Best;
            public int[] Indexes;
            public Int64 Equalizer;
            public Int32 Origin;
            public bool SignChanged;
        }

        public class SpectrabyRT : IComparer<MGFSpectrum> {
            public int Compare(MGFSpectrum x, MGFSpectrum y){
                double XRT = x.RTApex == 0.0 ? x.RT : x.RTApex;
                double YRT = y.RTApex == 0.0 ? y.RT : y.RTApex;
                if (XRT>YRT) { return 1;} 
                else if (XRT== YRT) { return 0; }
                else return -1;
            }
        }
        List<Aligner> Aligners;
        MGFFile[] Files;
        int FileCount;
        double PPMs,RTPerc,RTMin,MS2Acc;
        double MaxRT;
        int SpectraPerCluster;
        int FrMatch;
        List<MGFMatched> Merged = new List<MGFMatched>();
        int[] Slices;
        List<MGFMatched>[] MatchedFiles;

        public class IntArg{
            public int Arg;
            public IntArg(int Arg){
                this.Arg = Arg;
            }
        }

        public class DoubleArg{
            public double Arg;
            public DoubleArg(double Arg){
                this.Arg = Arg;
            }
        }

        int ExecCount;

        private void Progress(int Curr, int Total){
            int percent = (int)(((double)Curr/(double)Total)*100.0);
            backgroundWorker1.ReportProgress(percent);
        }

        private bool InInterval(double Value,double Pattern,double Error){
            return Math.Abs(Value-Pattern)<Error;
        }

        private void DoWork() {
            //проверка параметров 
            try{
                PPMs = Convert.ToDouble(Settings.Default.MassDev);
                RTPerc = Convert.ToDouble(Settings.Default.RTDevProc);
                RTMin = Convert.ToDouble(Settings.Default.RTDevMin);
                MS2Acc = Convert.ToDouble(Settings.Default.MS2Da);
                FrMatch = Convert.ToInt32(Settings.Default.MS2PeakMatches);
                SpectraPerCluster = Convert.ToInt32(Settings.Default.SpecPerClust);
            }catch{
                MessageBox.Show("Parametrs must be set.");
            }

            //открываем себе файл для выборочного лога 
            //StreamWriter SW = new StreamWriter("log.log");
            //загрузка данных
            Files = new MGFFile[MGFlist.Items.Count];
            FileCount = MGFlist.Items.Count;
            ProgressMessage = "Loading data...";
            for (int i = 0 ; i < FileCount ; i++){
                Files[i] = new MGFFile();
                Files[i].MGFRead(MGFlist.Items[i].ToString());
                Progress(i,FileCount);
            //если есть информация по RT Apex - используем ее 
                for (int j = 0; j < Files[i].Spectra.Count; j++ ){
                    if (Files[i].Spectra[j].RTApex != 0.0){
                        Files[i].Spectra[j].RT = Files[i].Spectra[j].RTApex;
                    }
                }
            }

            int WorkerThreads, IOThreads;
            ThreadPool.GetMaxThreads(out WorkerThreads, out IOThreads);
            WorkerThreads = Environment.ProcessorCount;
            ThreadPool.SetMaxThreads(WorkerThreads, IOThreads);

            //Очистка лишних сигналов 
            
            try{
                MISignals = Convert.ToInt32(Settings.Default.MS2PeaksNumber);
            }catch { MISignals = 0; }


            SpectrabyRT sr = new SpectrabyRT();
            for (int i = 0 ; i < FileCount ; i++ ) {
                Files[i].Spectra.Sort(sr);
            }

            //Собираем консенсусный набор спектров 
            //матчинг спектров - нетранзитивен!!
            Aligners = new List<Aligner>();

            //Матчинг внутри файла
            MatchedFiles = new List<MGFMatched>[FileCount];
            int LocalCount;
            ProgressMessage = "Intermal Spectra Matching...";
            ExecCount = 0;
            for(int i = 0 ; i < FileCount ; i++){
                ThreadPool.QueueUserWorkItem(new WaitCallback(SingleFileMatching),new IntArg(i));
            }
            while(true){
                lock (ForCountLock){
                    LocalCount = ExecCount;
                }
                Thread.Sleep(200);
                Progress(LocalCount,Files.GetLength(0));
                if (LocalCount >= Files.GetLength(0)) break;
            }

            /*for (int i = 0 ; i < FileCount ; i++ ){
                MatchedFiles[i] = SingleFileMatching(i);
            }*/

            //соединяем и сортируем по времени выхода
            for (int i = 0 ; i < FileCount ; i++ ){
                Merged.AddRange(MatchedFiles[i]);
            }
            Merged.Sort(new MGFMatched.MatchedByRT());
            MaxRT = Merged[Merged.Count - 1].Spectrum.RT;

            //сматчивем горизонтально в три прослойки (операционное поле, буфер спереди, буфер сзади)
            ProgressMessage = "External Spectra Matching...";
            int SliceCount = Convert.ToInt32(Math.Floor(MaxRT / RTMin))+1; 
            ExecCount = 0;
            for(int i = 0 ; i<SliceCount ; i+=3){
                ThreadPool.QueueUserWorkItem(new WaitCallback(HorizontalMatching),new DoubleArg(RTMin * i));
            }
            while(true){
                lock (ForCountLock){
                    LocalCount = ExecCount;
                }
                Thread.Sleep(200);
                Progress(LocalCount,SliceCount);
                if (LocalCount == ((SliceCount-1) / 3) + 1 ) break;
            }

            for(int i = 1 ; i<SliceCount ; i+=3){
                ThreadPool.QueueUserWorkItem(new WaitCallback(HorizontalMatching),new DoubleArg(RTMin * i));
            }
            while(true){
                lock (ForCountLock){
                    LocalCount = ExecCount;
                }
                Thread.Sleep(200);
                Progress(LocalCount,SliceCount);
                if (LocalCount == (((SliceCount-1) / 3) + 1) + ((SliceCount-2)/3 + 1) ) break;
            }

            for(int i = 2 ; i<SliceCount ; i+=3){
                ThreadPool.QueueUserWorkItem(new WaitCallback(HorizontalMatching),new DoubleArg(RTMin * i));
            }
            while(true){
                lock (ForCountLock){
                    LocalCount = ExecCount;
                }
                Thread.Sleep(200);
                Progress(LocalCount,SliceCount);
                if (LocalCount >= SliceCount ) break;
            }

/*            for(int i=0 ; i<SliceCount ; i+=3){
                HorizontalMatching(new DoubleArg(RTMin * i));
            }
            for(int i=1 ; i<SliceCount ; i+=3){
                HorizontalMatching(new DoubleArg(RTMin * i));
            }
            for(int i=2 ; i<SliceCount ; i+=3){
                HorizontalMatching(new DoubleArg(RTMin * i));
            }*/

            Aligner Al;
            for (int i = 0 ; i < Merged.Count ; i++){
                if (Merged[i] != null){
                    Al = new Aligner();
                    Al.Best = Merged[i].Spectrum;
                    Al.Origin = Merged[i].Origin;
                    Al.Indexes = new int[FileCount];
                    for (int j = 0; j < FileCount; j++) Al.Indexes[j] = -1;
                    Al.Indexes[Al.Origin] = Merged[i].Index;
                    foreach(MGFMatched M in Merged[i].InterMatched){
                        Al.Indexes[M.Origin] = M.Index;
                    }
                    Al.SignChanged = false;
                    Aligners.Add(Al);
                }
            }

            //собственно выравнивание 
            //??? - В каком порядке исходно стоят алигнеры
            int Count;
            //оцениваем расстояние и количество спектров 
            ProgressMessage = "Estimating spectra distance...";
            ExecCount = 0;
            for (int i = 0 ; i<Aligners.Count ; i+=1000){
                ThreadPool.QueueUserWorkItem(new WaitCallback(DistanceEst),new IntArg(i));
            }
            while(true){
                lock (ForCountLock){
                    LocalCount = ExecCount;
                }
                Thread.Sleep(200);
                Progress(LocalCount,(Aligners.Count-1)/1000 +1);
                if (LocalCount >= (Aligners.Count-1)/1000 +1 ) break;
            }


            //подвид пузырьковой сортировки 
            //нарезаем кусочки по RTMin 
            Slices = new int[SliceCount];
            Count = 0;
            for(int i = 0 ; i< Aligners.Count ; i++){
                if (Aligners[i].Best.RT>=((double)(Count)*RTMin)){
                    Slices[Count] = i;
                    Count++;
                }
            }

            ProgressMessage = "Aligning spectra...";
            int UpperIndex,LowerIndex;
            for(int i = 0 ; i< SliceCount ; i++){//максимальная процедура - на квадрат 
                bool flag = true;
                Count = 0;
                ExecCount = 0;
                for ( int j = i%2; j < SliceCount ; j+=2){
                    UpperIndex = Slices[j];
                    if (j + 2 < SliceCount){
                        LowerIndex = Slices[j + 2];
                    }else{
                        LowerIndex = Aligners.Count;
                    }
                    int k;
                    for (k = UpperIndex ; k < LowerIndex ; k++){
                        if (!Aligners[k].SignChanged){
                            break;
                        }
                    }
                    if (k<LowerIndex){
                        ThreadPool.QueueUserWorkItem(new WaitCallback(Arrange),new IntArg(j));
                        //Arrange(new IntArg(j));
                        Count++;
                        flag = false;
                    }
                }
                while(true){
                    lock (ForCountLock){
                        LocalCount = ExecCount;
                    }
                    Thread.Sleep(200);
                    Progress(SliceCount - Count , SliceCount );
                    if (LocalCount >= Count) break;
                }

                if (flag) break;
            }


            ProgressMessage = "Writing spectra...";
            Progress(0,100);

            //Меняем FSN в Title - сообразно новому порядку
            for ( int i = 0 ; i < Aligners.Count ; i++){
                string Title = Aligners[i].Best.Title;
                if (Title.Contains("Origin")){ //затычка на повторное включение спектра в коллекцию
                    Aligners[i].Best.Title = null;
                    continue;
                }
                Title = Title.Replace("FinneganScanNumber:",
                    String.Format("Origin: {0} Order: {1} FinneganScanNumber:",
                    Path.GetFileName(MGFlist.Items[Aligners[i].Origin].ToString()),i));
                Aligners[i].Best.Title = Title;
                Aligners[i].Best.Data.Sort(new ChildbyMass());
            }


            //выводим конечную коллекцию 
            //for (SpectraPerCluster = 1 ; SpectraPerCluster<48 ; SpectraPerCluster++){
            MGFFile OutFile = new MGFFile();
            for ( int i = 0 ; i < Aligners.Count ; i++){
                if (Aligners[i].Best.Title == null) continue;
                //считаем наличные индексы
                int IndexCount = 0;
                for (int j = 0 ; j < FileCount ; j++){
                    if (Aligners[i].Indexes[j] != -1) IndexCount++;
                }
                if (IndexCount>=SpectraPerCluster){
                    OutFile.Spectra.Add(Aligners[i].Best);
                }
            }
            //формируем комментарий 
            string Comment = Text+"; Consensus MGF aligned from :";
            for (int i = 0 ; i < FileCount ; i++){
                Comment += MGFlist.Items[i].ToString()+", ";
            }
            OutFile.MGFComments.Add(Text);
            OutFile.MGFComments.Add(String.Format("MS Tol.: {0} ppm, MS/MS Tol: {1} Da, RT Deviation {2} min., Minimum {3} peaks of top {4} matched.",
                MassDevBox.Text,MS2DaBox.Text,RTMinBox.Text,textBox8.Text,MS2PeaksBox.Text));
            OutFile.MGFComments.Add("Source files:");
            for (int i = 0 ; i < FileCount ; i++){
                    OutFile.MGFComments.Add(MGFlist.Items[i].ToString());
            }
            OutFile.MGFComments.Add(String.Format("Consensus MGF aligned from {0} files",FileCount));
            OutFile.MGFWrite(OutMgfBox.Text/*+SpectraPerCluster.ToString()*/);
            //}

            Progress(100,100);
            //SW.Close();
            ProgressMessage = "Processing completed...";
            SystemSounds.Beep.Play();

        }

        public Object ForCountLock = new object();

        int MISignals;

        public int Match(ref MGFMatched First, ref MGFMatched Second){
            //проверка по массе родительского иона
            double F = (First.Spectrum.mz - 1.0078)*First.Spectrum.Charge;
            double S = (Second.Spectrum.mz - 1.0078)*Second.Spectrum.Charge;
            double Diff = (Math.Abs(F - S)/Math.Max(F,S))*1000000.0;
            if (Diff>PPMs ) { return 0; }
            //проверка по retention time
            Diff = (Math.Max(First.Spectrum.RT,Second.Spectrum.RT)/100)*RTPerc+RTMin;
            if (Math.Abs(First.Spectrum.RT-Second.Spectrum.RT) > Diff) { return 0; }

            //проверка по совпадению фрагментов

            int toCompF,toCompS;
             
            if (MISignals>0 && MISignals<First.Spectrum.Data.Count) {
                toCompF = MISignals;
            }else{
                toCompF = First.Spectrum.Data.Count;
            }
          
            if (MISignals>0 && MISignals<Second.Spectrum.Data.Count) {
                toCompS = MISignals;
            }else{
                toCompS = Second.Spectrum.Data.Count;
            }

            int Count = 0;
            for (int i = 0 ; i < toCompF ; i++ ) {
                for (int j = 0 ; j < toCompS ; j++ ) {
                    if (Math.Abs(First.Spectrum.Data[i].Mass - Second.Spectrum.Data[j].Mass)<MS2Acc) {
                        Count++;
                    }
                }
            }

            if (Count<FrMatch) {return 0;}
            //Замер интенсивностей 
            if (First.Weight>Second.Weight){
                return 1;
            }else{
                return -1;
            }
        }

        string ProgressMessage;

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            progressBar1.Value = e.ProgressPercentage;
            ProgressLabel.Text=ProgressMessage;
        }

        private void GoButton_Click(object sender, EventArgs e) {
            //DoWork();
            //SystemSounds.Beep.Play();
            //return;
            AddButton.Enabled = false;
            DeleteButton.Enabled = false;
            GoButton.Enabled = false;
            SaveMGFbutton.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
            //DoWork();
            //backgroundWorker1_RunWorkerCompleted(null,null);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            AddButton.Enabled = true;
            DeleteButton.Enabled = true;
            GoButton.Enabled = true;
            SaveMGFbutton.Enabled = true;
            SystemSounds.Beep.Play();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            DoWork();
        }

        public class MGFMatched{
            public MGFSpectrum Spectrum;
            public Double Weight;
            public int Index;
            public List<int> Matched;
            public List<MGFMatched> InterMatched;
            public int Origin;
            public  int Role;
            static public ChildbyIntensity ci=new ChildbyIntensity();
            static public ChildbyMass cm = new ChildbyMass();

            public MGFMatched(MGFSpectrum Spec){
                Spectrum = Spec;
                Matched = new List<int>();
                InterMatched = new List<MGFMatched>();
                Spectrum.Data.Sort(ci);
                Spectrum.Data.Reverse();
                Weight = 0.0;
                foreach(Childs c in Spectrum.Data){
                    Weight += c.Intensity;
                }
            }

            public class MatchedByMass : IComparer<MGFMatched> {
                public int Compare(MGFMatched x, MGFMatched y){
                    if (x.Spectrum.mz>y.Spectrum.mz) { return 1;} 
                    else if (x.Spectrum.mz == y.Spectrum.mz) { return 0; }
                    else return -1;
                }
            }

            public class MatchedByWeight : IComparer<MGFMatched> {
                public int Compare(MGFMatched x, MGFMatched y){
                    if (x.Weight>y.Weight) { return -1;} 
                    else if (x.Weight == y.Weight) { return 0; }
                    else return 1;
                }
            }

            public class MatchedByRT : IComparer<MGFMatched> {
                public int Compare(MGFMatched x, MGFMatched y){
                    if (x.Spectrum.RT>y.Spectrum.RT) { return 1;} //или таки RT Apex??!!
                    else if (x.Spectrum.RT == y.Spectrum.RT) { return 0; }
                    else return -1;
                }
            }
        }

        public void SingleFileMatching(object FileIDObj){
            int FileID = (FileIDObj as IntArg).Arg;
            //создаем список из 
            MGFFile File = Files[FileID];
            List<MGFMatched> Matches = new List<MGFMatched>();
            MGFMatched M;
            for ( int i = 0 ; i < File.Spectra.Count ; i++){
                M = new MGFMatched(File.Spectra[i]);
                M.Index = i;
                M.Origin = FileID;
                Matches.Add(M);
            }
            //сортируем список по массам 
            Matches.Sort(new MGFMatched.MatchedByMass());
            // пробегаемся по списку и матчим тех кто в пределах массы
            for (int i = 0; i < Matches.Count; i++ ){
                //отступаем 
                int First;
                for (First = i ; First >= 0 ; First--){
                    double Diff = (Math.Abs(Matches[First].Spectrum.mz - Matches[i].Spectrum.mz)/
                        Math.Max(Matches[First].Spectrum.mz,Matches[i].Spectrum.mz))*1000000.0;
                    if (Diff>PPMs){
                        break;
                    }
                }
                First++;
                for (int j = First ; j < Matches.Count ; j++){
                    if (i == j) continue;
                    if (Matches[i].Matched.Contains(Matches[j].Index) || 
                        Matches[j].Matched.Contains(Matches[i].Index)) continue;
                    double Diff = (Math.Abs(Matches[First].Spectrum.mz - Matches[j].Spectrum.mz)/
                        Math.Max(Matches[First].Spectrum.mz,Matches[j].Spectrum.mz))*1000000.0;
                    if (Diff>PPMs) break;
                    MGFMatched IM = Matches[i];
                    MGFMatched JM = Matches[j];
                    int R = Match(ref IM, ref JM);
                    if (R > 0) IM.Matched.Add(JM.Index);
                    if (R < 0) JM.Matched.Add(IM.Index);
                }
            }
            //Вытаскиваем за головку дерева. самые интенсивные - остальных уничтожаем 
            Matches.Sort(new MGFMatched.MatchedByWeight());
            for ( int i = 0 ; i < Matches.Count ; i++){
                if (Matches[i].Matched == null) continue;
                for ( int j = i ; j < Matches.Count ; j++){
                    if (Matches[j].Matched == null) continue;
                    if (Matches[i].Matched.Contains(Matches[j].Index)){
                        Matches[i].Matched.AddRange(Matches[j].Matched);
                        Matches[j].Matched = null;
                    }
                }
            }
            for ( int i = Matches.Count-1 ; i >= 0 ; i--){
                if (Matches[i].Matched == null) {
                    Matches.RemoveAt(i);
                }
            }
            MatchedFiles[FileID] = Matches;
            lock (ForCountLock){
                ExecCount++;
            }
            return;
        }

        Object Limits = new object();

        public void HorizontalMatching(object RTObj){
            //границы поиска - в критической секции 
            double RT = (RTObj as DoubleArg).Arg;
            int OperUpper = 0, OperLower = 0, SearchUpper = 0, SearchLower = 0;
            lock (Limits){
                if (RT - RTMin <= 0.0) SearchLower = 1;
                if (RT == 0.0) OperLower = 1;
                if (RT + RTMin > MaxRT) OperUpper = Merged.Count - 1;
                if (RT + RTMin + RTMin > MaxRT) SearchUpper = Merged.Count - 1;
                for (int i = 0 ; i<Merged.Count ; i++){
                    if (SearchLower == 0 && Merged[i] != null && Merged[i].Spectrum.RT > RT - RTMin) SearchLower = i;
                    if (OperLower == 0 && Merged[i] != null && Merged[i].Spectrum.RT > RT ) OperLower = i;
                    if (OperUpper == 0 && Merged[i] != null && Merged[i].Spectrum.RT > RT + RTMin) OperUpper = i-1;
                    if (SearchUpper == 0 && Merged[i] != null && Merged[i].Spectrum.RT > RT + RTMin + RTMin) {
                        SearchUpper = i-1;
                        break;
                    }
                }
            }
            for (int i = OperLower ; i < OperUpper ; i++){
                if (Merged[i] == null) continue;
                for ( int j = SearchLower ; j < SearchUpper ; j++){
                    if (Merged[j] == null) continue;
                    if (i == j) continue;
                    MGFMatched Mi = Merged[i];
                    MGFMatched Mj = Merged[j];
                    int m = Match(ref Mi, ref Mj);
                    if (m == 0) continue;
                    if (m > 0 ){
                        Merged[i].InterMatched.Add(Merged[j]);
                        Merged[i].InterMatched.AddRange(Merged[j].InterMatched);
                        Merged[j].InterMatched = null;
                        Merged[j] = null;
                    }else{
                        Merged[j].InterMatched.Add(Merged[i]);
                        Merged[j].InterMatched.AddRange(Merged[i].InterMatched);
                        Merged[i].InterMatched = null;
                        Merged[i] = null;
                        break;
                    }
                }
            }
            lock (ForCountLock){
                ExecCount++;
            }
        }

        Object Als = new object();

        public void DistanceEst(object IndexObj){

            int Index = (IndexObj as IntArg).Arg;
            int UpperIndex = Math.Min(Index + 1000, Aligners.Count);

            for (int i = Index ; i<UpperIndex ; i++){
                //для каждого спектра - проходим вних и вверх считая спектры не на месте 
                //на полную длинну файла???
                int Count = 0;
                double MinRT = Aligners[i].Best.RT - RTMin * 2.0;
                double MaxRT = Aligners[i].Best.RT + RTMin * 2.0;
                for (int k = i-1 ; k >=0 ; k--){
                    if (Aligners[k].Best.RT < MinRT) break;
                    for (int l = 0 ; l < FileCount ; l++ ){
                        if (Aligners[i].Indexes[l] != -1 &&
                            Aligners[k].Indexes[l] != -1 &&
                            Aligners[i].Indexes[l]<Aligners[k].Indexes[l]){
                            Count--;
                        }
                    }
                }
                for (int k = i+1 ; k < Aligners.Count ; k++){
                    if (Aligners[k].Best.RT > MaxRT) break;
                    for (int l = 0 ; l < FileCount ; l++ ){
                        if (Aligners[i].Indexes[l] != -1 &&
                            Aligners[k].Indexes[l] != -1 &&
                            Aligners[i].Indexes[l]>Aligners[k].Indexes[l]){
                            Count++;
                        }
                    }
                }
                lock (Als){
                    Aligner Al = Aligners[i];
                    Al.Equalizer = Count;
                    Aligners[i] = Al;
                }
            }

            lock (ForCountLock){
                ExecCount++;
            }
        }

        public void Arrange(object SliceObj ){
            int Slice = (SliceObj as IntArg).Arg;
            Int64 ChEq;
            int Lower = Slices[Slice];
            int Upper = (Slice + 2 >= Slices.GetLength(0)) ? Aligners.Count : Slices[Slice + 2];
            for (int i = Lower ; i<Upper; i++){
                int ChCount =0;
                Aligner Buf1, Buf2;
                for (int j = Lower ; j<Upper-1 ; j++){
                    ChEq = 0; 
                    if (Aligners[j].Equalizer!=0){//если неравновесно оцениваем изме6нение Equalizer после обмена местами с соседями 
                        if (Aligners[j].Equalizer<0){//сосед сверху 
                            for (int l = 0 ; l < FileCount ; l++ ){//расчет изменения equalizer
                                if (Aligners[j].Indexes[l] != -1 &&
                                    Aligners[j-1].Indexes[l] != -1){
                                    ChEq++;
                                }
                            }
                            Buf1 = Aligners[j];
                            Buf2 = Aligners[j-1];
                            Buf1.SignChanged |= Buf1.Equalizer * (Buf1.Equalizer + ChEq) <= 0; //!! в том числе и "если бы!!" поменялись
                            Buf2.SignChanged |= Buf2.Equalizer * (Buf2.Equalizer - ChEq) <= 0;
                            if (Math.Abs(Aligners[j].Equalizer+ChEq)+Math.Abs(Aligners[j-1].Equalizer-ChEq)<= //если сумма модулей после обмена будет 
                                Math.Abs(Aligners[j].Equalizer)+Math.Abs(Aligners[j-1].Equalizer)){           //не хуже чем раньше - производим обмен
                                Buf1.Equalizer += ChEq;
                                Buf2.Equalizer -= ChEq;
                                Aligners[j] = Buf2;
                                Aligners[j-1] = Buf1;
                                ChCount++;
                            }else{
                                Aligners[j] = Buf1;
                                Aligners[j-1] = Buf2;
                            }
                        }else{
                            for (int l = 0 ; l < FileCount ; l++ ){//расчет изменения equalizer
                                if (Aligners[j].Indexes[l] != -1 &&
                                    Aligners[j+1].Indexes[l] != -1){
                                    ChEq--;
                                }
                            }
                            Buf1 = Aligners[j];
                            Buf2 = Aligners[j+1];
                            Buf1.SignChanged |= Buf1.Equalizer * (Buf1.Equalizer + ChEq) <= 0; //!! в том числе и "если бы!!" поменялись
                            Buf2.SignChanged |= Buf2.Equalizer * (Buf2.Equalizer - ChEq) <= 0;
                            if (Math.Abs(Aligners[j].Equalizer+ChEq)+Math.Abs(Aligners[j+1].Equalizer-ChEq)<= //если сумма модулей после обмена будет 
                                Math.Abs(Aligners[j].Equalizer)+Math.Abs(Aligners[j+1].Equalizer)){           //не хуже чем раньше - производим обмен
                                Buf1.Equalizer += ChEq;
                                Buf2.Equalizer -= ChEq;
                                Aligners[j] = Buf2;
                                Aligners[j+1] = Buf1;
                                ChCount++;
                            }else{
                                Aligners[j] = Buf1;
                                Aligners[j+1] = Buf2;
                            }
                        }
                    }
                }
                if (ChCount == 0) break;
            }
            lock (ForCountLock){
                ExecCount++;
            }

       }
    }
}