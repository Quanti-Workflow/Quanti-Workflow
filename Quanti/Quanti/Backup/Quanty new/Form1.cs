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
using XRAWFILE2Lib;
using Quanty.Properties;
using Mascot;

namespace Quanty {
    public interface ILogAndProgress {
        void Log(string Message);
        void Log(string Message, System.Windows.Forms.MessageBoxIcon WarrningLevel, string StackInfo);
        void ProgressMessage(string Message);
        void RepProgress(int Perc);
    }


    public partial class Form1 : Form, Quanty.ILogAndProgress {

        MascotParser mp;

        public Form1() {
            InitializeComponent();
        }

        private void DATFileButton_Click(object sender, EventArgs e) {
            if(DatFileDialog.ShowDialog() != DialogResult.OK)  {
                return;
            }
            DatFileName.Text = DatFileDialog.FileName;
            if (File.Exists(Path.GetDirectoryName(DatFileDialog.FileName)+Path.DirectorySeparatorChar+Path.GetFileNameWithoutExtension(DatFileDialog.FileName)+".db3")){
                if (MessageBox.Show("There is a .db3 file for selected .dat. \n This file contains processing results "+
                                    "and will be overwriten.\n Are you sure you want to overwrite this file?","Result file already exsists",MessageBoxButtons.OKCancel,MessageBoxIcon.Exclamation) == DialogResult.Cancel){
                    DatFileName.Text="";
                    return; 
                };
            }
            mp = new Mascot.MascotParser();
            mp.ParseFile(DatFileDialog.FileName);
            //меняем маскот-scan на Order - если таковой имеется
            for (int i = 0; i < mp.Spectra.Count; i++) {
                if (mp.Spectra[i].Title.Contains("Order")) {
                    string FSN = mp.Spectra[i].Title.Substring(mp.Spectra[i].Title.IndexOf("Order")+11);
                    FSN = FSN.Substring(0, FSN.IndexOf("Finnegan")-3);
                    mp.Spectra[i].ScanNumber = Convert.ToInt32(FSN);
                }
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

            if (File.Exists(Settings.Default.DatFileName)){
                DatFileName.Text = Settings.Default.DatFileName;
                mp = new Mascot.MascotParser();
                mp.ParseFile(Settings.Default.DatFileName);
                OutPathName.Text = Path.GetDirectoryName(DatFileName.Text);
                folderBrowserDialog1.SelectedPath = Path.GetDirectoryName(DatFileName.Text);
            }else{
                DatFileName.Text = "";
            }
            ReferenceFile = "None";
            LoadReportVector();
            TimeColumn.Width = 75;
            MessageHeader.Width = LogList.Width-100;
            RefCombo.SelectedIndex = 0;
            LockRefFeatures(true);
            RefNumber = -1;
            db = null;
            DBInterface.iLog = this;
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

        List<QPeptide> MSMSList;


        public double GetStringValue(string Str){
            try{
	            return Convert.ToDouble(Str);
            }catch{
                return 0.0;
            }
        }

        RawFileBox MSFile;

        public void RepProgress(int Perc){
            backgroundWorker1.ReportProgress(Perc);
            Application.DoEvents();
        }

        public void ProgressMessage(string Message){
            ProgressLabel.Text = Message;
            Application.DoEvents();
        }

        int RefNumber;
        DBInterface db;

        //Главная кнопка
        private void DoWork() {

            RawFileBox.iLog = this;
            MascotProc.iLog = this;
            DBInterface.iLog = this;

            backgroundWorker1.ReportProgress(0);
            if (mp!=null && db == null){
                Log("Parsing and grouping Mascot results:");
                MascotProc.FillMSMSLists(ref Mascots, ref MSMSList, ref Proteins, mp);
            }
            //сохранение разбора маскот в базу данных
            String dbName = DBFileName.Text;
            if (db ==null || db.GetFileName()!=DBFileName.Text){
                db = new DBInterface();
                db.CreateNewDB(dbName);
                SaveSettingsDB();
                db.SaveProteins(Proteins);
                db.SaveMSMS(Mascots,true);
                if (Settings.Default.QuantiAll){
                    db.SaveMSMS(MSMSList,false);
                }
            }


            for (int i = 0 ; i < MSMSList.Count ; i++ ){
                if (MSMSList[i].Matches == null){
                    MSMSList[i].Matches = new QMatch[RawList.Items.Count];
                }else{
                    if (MSMSList[i].Matches.GetLength(0) !=  RawList.Items.Count){
                        QMatch[] Temp = MSMSList[i].Matches;
                        MSMSList[i].Matches = new QMatch[RawList.Items.Count];
                        for (int j=0 ; j < Temp.GetLength(0) ; j++ ){
                            MSMSList[i].Matches[j] = Temp[j];
                        }
                    }
                }
                MSMSList[i].ReferenceMatrix = new double[RawList.Items.Count,RawList.Items.Count];
            }

            for (int i = 0 ; i < Proteins.Count ; i++ ){
                Proteins[i].CreateMatches(RawList.Items.Count);
            }


            for (int i = 0 ; i < RawList.Items.Count ; i++){
                //открываем RAW-файл 
                try {

                    string FileName = RawList.Items[i].ToString();

                    if (FileName[0] == '+') continue;

                    Log(string.Format("Loading {0} File Index:",FileName));
                    backgroundWorker1.ReportProgress(0);

                    for (int j = 0 ; j < MSMSList.Count ; j++ ){
                        MSMSList[j].RefMatch = MSMSList[j].Matches[i];
                        if (RefNumber != -1){
                            MSMSList[j].RefNumberMatch = MSMSList[j].Matches[RefNumber];
                        }
                    }

                    MSFile = new RawFileBox();
                    MSFile.LoadIndex(FileName);
                    MSFile.RTCorrection = true;
                    MSFile.ESICorrection = false;

                    Log(string.Format("Loading {0} File:", FileName));
                    backgroundWorker1.ReportProgress(0);

                    //на выходе пептиды упорядочены по FSN (в смыслше по порядку выхода)
                    //обработка пептидов
                    double MinRT,MaxRT;
                    QPeptide.byMascotSN MSNcmp = new QPeptide.byMascotSN();

                    MinRT = 0.0;
                    int MaxInd = MSFile.RawSpectra.GetLength(0)-1;
                    if (MSFile.RawSpectra[MaxInd].RT == 0){
                        MaxInd = MSFile.ms2index[MaxInd];
                    }
                    MaxRT = MSFile.RawSpectra[MaxInd].RT;

                    MSFile.LoadInterval(MinRT,MaxRT);

                    Log("Initial search");
                    FirstSearch(Mascots,i);
                    Log("Outlayers Alignment - First iteration");
                    int RTOrder = Convert.ToInt32(GetStringValue(Settings.Default.RTOrder));
                    AlignOutLayers(Mascots,i,RTOrder/4,0.5);
                    Log("Outlayers Alignment - Second iteration");
                    AlignOutLayers(Mascots,i,RTOrder/2,0.25);
                    Log("Outlayers Alignment - Third iteration");
                    AlignOutLayers(Mascots,i,RTOrder,0.0);
                    //AlignOutLayers(Mascots,i,RTOrder/2,0.5);
                    //Log("Outlayers Alignment - Second iteration");
                    ////AlignOutLayers(Mascots,i,20,0.25);
                    //AlignOutLayers(Mascots,i,RTOrder,0.0);
                    //AlignOutLayers(Mascots,i,50,0.0);

                    if (Settings.Default.QuantiAll){
                        Log("Initial search of all MS/MS spectra");
                        //пометить Mascots как уже отысканные 
                        for (int j = 0 ; j < Mascots.Count ; j++){
                            Mascots[j].AlreadySearched = true;
                        }
                        FirstSearch(MSMSList,i);
                        Log("Outlayers Alignment of all MS/MS spectra - First iteration");
                        AlignOutLayers(MSMSList,i,RTOrder/2,0.5);
                        Log("Outlayers Alignment of all MS/MS spectra - Second iteration");
                        AlignOutLayers(MSMSList,i,RTOrder,0.25);
                        Log("Outlayers Alignment of all MS/MS spectra - Third iteration");
                        AlignOutLayers(MSMSList,i,RTOrder*2,0.0);
                        if (Settings.Default.QuantiAll){
                            //вывод всех Mascot Entries 
                            StreamWriter AllMSMS = new StreamWriter(OutPathName.Text+"\\"+Path.GetFileNameWithoutExtension(FileName)+".all");
                            WriteHeader(AllMSMS,FileName);
                            AllMSMS.Write(QPeptide.TableCaption());
                            AllMSMS.WriteLine("\tCASE");
                            for ( int i1 = 0 ; i1 < MSMSList.Count ; i1++ ){
                                AllMSMS.Write(MSMSList[i1].ToString());
                                AllMSMS.WriteLine("\t"+MSMSList[i1].Case);
                                MSMSList[i1].AlreadySearched = false;
                            }
                            AllMSMS.Close();
                        }

                    }
                    db.AddFile(i,FileName);
                    db.SaveQMatches(Mascots,i);
                    db.SaveMSMatches(Mascots,i);
                    FilePlus(i);

                    Log("File "+FileName+" was successfully processed.");

                }
                catch(Exception e){
                    Log(e.Message,MessageBoxIcon.Error,e.StackTrace);
                }
            }
            ReportWork();

        }

        private void ReferenceProcessing(int Index){
            //только для первого и второго файлов в списке 
            double Sum0 = 0.0, Sum1 = 0.0;

            for (int i=0 ; i < Mascots.Count ; i++){
                if (Mascots[i].Matches[Index]!= null && Mascots[i].Matches[Index].Score > 0.0){
                    Mascots[i].RefMatch = new QMatch(Mascots[i].Matches[Index]);
                    Mascots[i].Matches[Index].MaxConv = 0.0;
                    Mascots[i].Matches[Index].Shift = 0.0;
                    Mascots[i].Matches[Index].LongESICoef = 0.0;
                    Mascots[i].Matches[Index].ShitRatio = 0.0;
                }else{
                    Mascots[i].RefMatch = null;
                }
                if (Mascots[i].Matches[RefNumber]!= null && Mascots[i].Matches[RefNumber].Score > 0.0){
                    Mascots[i].RefNumberMatch = new QMatch(Mascots[i].Matches[RefNumber]);
                }else{
                    Mascots[i].RefNumberMatch= null;
                }

                if (Mascots[i].RefMatch != null && Mascots[i].RefNumberMatch!=null){
                    if (Settings.Default.CutTails){
                        Mascots[i].RefNumberMatch.CutTails(Mascots[i].RefMatch);
                    }
                    //Раньше это было настройкой 
                    double DispThres = 2;
                    //double DispThres = GetStringValue(Settings.Default.DispThres);
                    if ( Settings.Default.ShapeFilter){
                        if (Mascots[i].RefMatch.RTDisp != 0.0 && Mascots[i].RefNumberMatch.RTDisp != 0){
                            double Ratio = Math.Max(Mascots[i].RefMatch.RTDisp/Mascots[i].RefNumberMatch.RTDisp,
                                Mascots[i].RefNumberMatch.RTDisp/Mascots[i].RefMatch.RTDisp);
                            if (Ratio > DispThres){
                                Mascots[i].RefMatch = null;
                            }
                        }else{
                            Mascots[i].RefMatch = null;
                        }
                    }

                    //Раньше это было настройкой
                    //double ConvThres = GetStringValue(Settings.Default.ConvThres);
                    double ConvThres = 0.6;
                    if (Settings.Default.ShapeFilter && !Settings.Default.Matrix && Mascots[i].RefMatch != null){
                        Mascots[i].RefMatch.MaxConv = QMatch.CalcMaxConv(Mascots[i].RefMatch,Mascots[i].RefNumberMatch, ref Mascots[i].RefMatch.Shift);
                        Mascots[i].Matches[Index].Shift = Mascots[i].RefMatch.Shift;
                        if (Mascots[i].RefMatch.MaxConv < ConvThres){
                            Mascots[i].RefMatch = null;
                        }
                    }
                }
            }

            for (int i=0 ; i < Mascots.Count ; i++){
                if (Mascots[i].RefNumberMatch==null || Mascots[i].RefMatch==null) continue;
                Mascots[i].RefMatch.LongESICoef = 1.0;
                Sum0+=Mascots[i].RefNumberMatch.Score;
                Sum1+=Mascots[i].RefMatch.Score;
            }
            double NormFactor = Sum1/Sum0;

            if (Settings.Default.Norm){
                for (int i=0 ; i < Mascots.Count ; i++){
                    if (Mascots[i].RefNumberMatch==null || Mascots[i].RefMatch==null) continue;
                    Mascots[i].RefMatch.Score = Mascots[i].RefMatch.Score / NormFactor;
                }
            }


            //Log(String.Format("Intensity alignment - horisontal {0} versus {1}",Index,RefNumber));

            double Interval = GetStringValue(Settings.Default.ESIInterval);

            if (Interval>0.0){
                LongESIAlignment(Interval);
            }

            //распихиваем ShitRatios
            for (int i = 0 ; i <  Mascots.Count ; i++){
                if (Mascots[i].Matches[Index] != null && Mascots[i].RefMatch != null && Mascots[i].RefNumberMatch != null){
                    Mascots[i].Matches[Index].ShitRatio = Mascots[i].RefMatch.Score / Mascots[i].RefNumberMatch.Score;
                    Mascots[i].ReferenceMatrix[Index,RefNumber] = Mascots[i].Matches[Index].ShitRatio;
                }else{
                    if (Mascots[i].Matches[Index] != null ){
                        Mascots[i].Matches[Index].ShitRatio = 0.0;
                    }
                    Mascots[i].ReferenceMatrix[Index,RefNumber] = 0.0;
                }
            }

            //выработка статистики по белкам 
            for ( int i1 = 0 ; i1 < Proteins.Count ; i1++ ){
                Proteins[i1].MakeStats(RefNumber>=0);
                Proteins[i1].AddMatch(Index);
                Proteins[i1].MatrixSlope[Index,RefNumber] = Proteins[i1].Slope;
                Proteins[i1].MatrixMedian[Index,RefNumber] = Proteins[i1].Median;
                Proteins[i1].SlopeError[Index,RefNumber] = Proteins[i1].SlopeDelta;
            }
        }

        void PrintFiles(int Index){
            //вывод файлов 
            StreamWriter pepts = null;
            StreamWriter prots = null;
            string FileName = RawList.Items[Index].ToString();
            QPeptide.bySet setcomp = new QPeptide.bySet();

            QProtein.byIPI sprot = new QProtein.byIPI();
            Proteins.Sort(sprot);

            if (PrintPeptides){
                pepts = new StreamWriter(OutPathName.Text+"\\"+Path.GetFileNameWithoutExtension(FileName)+".pept");
                WriteHeader(pepts,FileName);
                pepts.WriteLine(QPeptide.TableCaption());
            }

            if (PrintProteins){
                prots = new StreamWriter(OutPathName.Text+"\\"+Path.GetFileNameWithoutExtension(FileName)+".prot");
                WriteHeader(prots,FileName);
                prots.WriteLine(QProtein.TableCaption());
            }

            for ( int i1 = 0 ; i1 < Proteins.Count ; i1++ ){
                QProtein Prot=Proteins[i1];
                Prot.Peptides.Sort(setcomp);
                for (int j = 0 ; j < Proteins[i1].Peptides.Count ; j++){
                    if (PrintPeptides){
                        pepts.WriteLine(Proteins[i1].Peptides[j].ToString());
                    }
                }
                if (PrintProteins)
                    prots.WriteLine(Proteins[i1].ToString());
            }
            if (PrintProteins) prots.Close();
            if (PrintPeptides) pepts.Close();
        }

        void PrintPeptideShit(){
            string FileName = RawList.Items[RefNumber].ToString();
            StreamWriter pepshit = new StreamWriter(
                OutPathName.Text+"\\on_"+Path.GetFileNameWithoutExtension(FileName)+".pepsht");
            WriteHeader(pepshit,FileName);
            pepshit.Write(
                ((ReportAllPeptString[0]!='1')?"":"SEQUENCE\t")+
                ((ReportAllPeptString[1]!='1')?"":"PROTEIN ID\t")+
                ((ReportAllPeptString[2]!='1')?"":"REF ABUNDANCE\t")+
                ((ReportAllPeptString[3]!='1')?"":"MODIF\t"));
            for (int i = 0 ; i < RawList.Items.Count ; i++){
                pepshit.Write(Path.GetFileName(RawList.Items[i].ToString())+"\t");
            }
            pepshit.WriteLine();

            Mascots.Sort(new QPeptide.bySet());

            for(int j = 0; j<Mascots.Count ; j++){
                pepshit.Write(
                    ((ReportAllPeptString[0]!='1')?"":String.Format("{0}\t",Mascots[j].Sequence))+
                    ((ReportAllPeptString[0]!='1')?"":String.Format("{0}\t",Mascots[j].IPI))+
                    ((ReportAllPeptString[0]!='1')?"":String.Format("{0:f2}\t",(Mascots[j].Matches[RefNumber]==null?0.0:Mascots[j].Matches[RefNumber].Score)))+
                    ((ReportAllPeptString[0]!='1')?"":String.Format("{0}\t",(Mascots[j].ModMass!=0.0 ? Mascots[j].ModDesk : ""))));
                for (int i = 0 ; i < RawList.Items.Count ; i++){
                    if (Mascots[j].Matches[i]!=null){
                        //pepshit.Write("{0:f5}\t",Mascots[j].Matches[i].Shift);
                        //pepshit.Write("{0:f5}\t",Mascots[j].Matches[i].ApexRT);
                        pepshit.Write("{0:f5}\t",Mascots[j].Matches[i].ShitRatio);
                    }else{
                        pepshit.Write("0.0\t");
                    }
                }
                pepshit.WriteLine();
            }
            pepshit.Close();
        }

        void PrintProteinShit(){
            string FileName = RawList.Items[RefNumber].ToString();
            StreamWriter protshit = new StreamWriter(
                OutPathName.Text+"\\on_"+Path.GetFileNameWithoutExtension(FileName)+".protsht");
            WriteHeader(protshit,FileName);

            protshit.Write(
                ((ReportAllProtString[0]!='1')?"":"PROTEIN ID\t")+
                ((ReportAllProtString[1]!='1')?"":"REF ABUNDANCE\t")+
                ((ReportAllProtString[2]!='1')?"":"DESCRIPTION\t")+
                ((ReportAllProtString[3]!='1')?"":"PEPTIDES\t"));

            for (int i = 0 ; i < RawList.Items.Count ; i++){
                if (ReportAllProtString[4]=='1')
                    protshit.Write(Path.GetFileName(RawList.Items[i].ToString())+" Median\t");
                if (ReportAllProtString[5]=='1')
                    protshit.Write(Path.GetFileName(RawList.Items[i].ToString())+" Slope\t");
                if (ReportAllProtString[6]=='1')
                    protshit.Write(Path.GetFileName(RawList.Items[i].ToString())+" Average\t");
                if (ReportAllProtString[7]=='1')
                    protshit.Write(Path.GetFileName(RawList.Items[i].ToString())+" Min.\t");
                if (ReportAllProtString[8]=='1')
                    protshit.Write(Path.GetFileName(RawList.Items[i].ToString())+" Max.\t");
                if (ReportAllProtString[9]=='1')
                    protshit.Write(Path.GetFileName(RawList.Items[i].ToString())+" R-Square.\t");
            }
            protshit.WriteLine();

            Proteins.Sort(new QProtein.byIPI());

            for(int j = 0; j<Proteins.Count ; j++){
                protshit.Write(
                    ((ReportAllProtString[0]!='1')?"":String.Format("{0}\t",Proteins[j].ipi))+
                    ((ReportAllProtString[1]!='1')?"":String.Format("{0:f2}\t",Proteins[j].RefScore))+
                    ((ReportAllProtString[2]!='1')?"":String.Format("{0}\t",Proteins[j].Desc))+
                    ((ReportAllProtString[3]!='1')?"":String.Format("{0}\t",Proteins[j].Peptides.Count)));

                for (int i = 0 ; i < RawList.Items.Count ; i++){

                    protshit.Write(
                        ((ReportAllProtString[4]!='1')?"":String.Format("{0:f5}\t",Proteins[j].Matches[i].Median))+
                        ((ReportAllProtString[5]!='1')?"":String.Format("{0:f5}\t",Proteins[j].Matches[i].Slope))+
                        ((ReportAllProtString[6]!='1')?"":String.Format("{0:f5}\t",Proteins[j].Matches[i].PrAverage))+
                        ((ReportAllProtString[7]!='1')?"":String.Format("{0:f5}\t",Proteins[j].Matches[i].MinInterval))+
                        ((ReportAllProtString[8]!='1')?"":String.Format("{0:f5}\t",Proteins[j].Matches[i].MaxInterval))+
                        ((ReportAllProtString[9]!='1')?"":String.Format("{0:f5}\t",Proteins[j].Matches[i].RSquare)));
                }
                protshit.WriteLine();
            }
            protshit.Close();

        }

        class RatioScores{
            public double Ratio;
            public double Score;
            public double RT;
            public RatioScores(double inRatio, double InScore, double inRT){
                Ratio = inRatio;
                Score = InScore;
                RT = inRT;
            }
            public class byRat:IComparer<RatioScores>  {
                public int Compare(RatioScores x, RatioScores y){
                    if (x.Ratio>y.Ratio) { return 1;} 
                    else if (x.Ratio == y.Ratio) { return 0; }
                    else return -1;
                }
            }
        }

        StreamWriter sw;
        void LongESIAlignment(double Interval){
            Mascots.Sort(new QPeptide.byApexRT());

            sw = new StreamWriter("Ratios.txt");

            for (int i=0 ; i < Mascots.Count ; i++){
                if (!Mascots[i].CheckExist()) continue;
                //ищем медиану от Ratio
                List<RatioScores> Ratios = new List<RatioScores>();
                for (int j = 0 ; j<Mascots.Count ; j++){
                    if (!Mascots[j].CheckExist()) continue;
                    if (Math.Abs(Mascots[j].RefMatch.ApexRT - Mascots[i].RefMatch.ApexRT)>Interval)
                        continue;
                    //Рандомизация
                    //int Rand = R.Next(Mascots.Count);
                    //if (Mascots[Rand].Matches[1].Score == 0) {
                    //    Ratios.Add(1.0/NormFactor);
                    //    continue;
                    //}
                    //Ratios.Add(Mascots[Rand].Matches[0].Score/Mascots[Rand].Matches[1].Score);

                    Ratios.Add(new RatioScores(Mascots[j].RefNumberMatch.Score/Mascots[j].RefMatch.Score,Mascots[j].RefNumberMatch.Score,Mascots[j].RefNumberMatch.ApexRT));
                }
                if (Ratios.Count>3){
                    Ratios.Sort(new RatioScores.byRat());
                    double Sum = 0.0;
                    double Count = 0.0;

                    //weigted median on log
                    //считаем полную сумму весов 
                    //for (int j = 0; j < Ratios.Count; j++) {
                    //    Sum += Math.Log10(Ratios[j].Score)-3;//!!
                    //}
                    //double HSum = Sum / 2.0; //берем от нее половину 
                    //Sum = 0;
                    //double SumP = 0.0; //Math.Pow(Ratios[0].Score, 0.0);//!!
                    //for (int j = 0; j < Ratios.Count; j++) {
                    //    Sum = SumP;
                    //    SumP += Math.Log10(Ratios[j].Score) - 3;//!!
                    //    if (Sum <= HSum && SumP > HSum) {//доходим до половины суммы весов - если перевалили через полусумму то в пределах +/-0.5 от точки
                    //        //определяем к какой точке ближе
                    //        SumP = Sum + (Math.Log10(Ratios[j].Score) - 3) * 0.5; //середина столбика
                    //        if (SumP > HSum) { //медиана - на первой половине столбика
                    //            Sum = Sum - (Math.Log10(Ratios[j-1].Score)-3)  * 0.5; //до середины предыдущего столбика 
                    //            double med = (HSum - Sum) / (SumP - Sum);
                    //            Mascots[i].RefMatch.LongESICoef = Ratios[j - 1].Ratio * (1 - med) + Ratios[j].Ratio * med;
                    //            break;
                    //        } else { //медиана - на второй половине столбика
                    //            Sum = SumP + (Math.Log10(Ratios[j].Score)-3)  * 0.5 + (Math.Log10(Ratios[j+1].Score)-3) * 0.5; //до середины следующего столбика 
                    //            double med = (HSum - SumP) / (Sum - SumP);
                    //            Mascots[i].RefMatch.LongESICoef = Ratios[j].Ratio * (1 - med) + Ratios[j + 1].Ratio * med;
                    //            break;
                    //        }
                    //    }
                    //}

                    //weigted median on pow
                    //считаем полную сумму весов 
                    double Pow = 0.5;
                    for (int j = 0; j < Ratios.Count; j++) {
                        Sum += Math.Pow(Ratios[j].Score, Pow);//!!
                    }
                    double HSum = Sum / 2.0; //берем от нее половину 
                    Sum = 0;
                    double SumP = 0.0; //Math.Pow(Ratios[0].Score, 0.0);//!!
                    for (int j = 0; j < Ratios.Count; j++) {
                        Sum = SumP;
                        SumP += Math.Pow(Ratios[j].Score, Pow);//!!
                        if (Sum <= HSum && SumP > HSum) {//доходим до половины суммы весов - если перевалили через полусумму то в пределах +/-0.5 от точки
                            //определяем к какой точке ближе
                            SumP = Sum + Math.Pow(Ratios[j].Score, Pow) * 0.5; //середина столбика
                            if (SumP > HSum) { //медиана - на первой половине столбика
                                Sum = Sum - Math.Pow(Ratios[j - 1].Score, Pow) * 0.5; //до середины предыдущего столбика 
                                double med = (HSum - Sum) / (SumP - Sum);
                                Mascots[i].RefMatch.LongESICoef = Ratios[j - 1].Ratio * (1 - med) + Ratios[j].Ratio * med;
                                break;
                            } else { //медиана - на второй половине столбика
                                Sum = SumP + Math.Pow(Ratios[j].Score, Pow) * 0.5 + Math.Pow(Ratios[j + 1].Score, Pow) * 0.5; //до середины следующего столбика 
                                double med = (HSum - SumP) / (Sum - SumP);
                                Mascots[i].RefMatch.LongESICoef = Ratios[j].Ratio * (1 - med) + Ratios[j + 1].Ratio * med;
                                break;
                            }
                        }
                    }

                    //среднее взвешенное на логарифм интенсивности
                    //for (int j = 0; j < Ratios.Count; j++) {
                    //    Sum += Ratios[j].Ratio * (Math.Log10(Ratios[j].Score)-3);
                    //    Count += Math.Log10(Ratios[j].Score) - 3; 
                    //}
                    //Mascots[i].RefMatch.LongESICoef = Sum / Count;
                    //взвешенное среднее на степень
                    //for (int j = 0; j < Ratios.Count; j++) {
                    //    Sum += Ratios[j].Ratio * Math.Pow(Ratios[j].Score,0.0);
                    //    Count += Math.Pow(Ratios[j].Score, 0.0);
                    //}
                    //Mascots[i].RefMatch.LongESICoef = Sum / Count;
                    //взвешенное среднее 
                    //for (int j = 0; j < Ratios.Count; j++) {
                    //    Sum += Ratios[j].Ratio*Ratios[j].Score;
                    //    Count += Ratios[j].Score;
                    //}
                    //Mascots[i].RefMatch.LongESICoef = Sum / Count;
                    //среднее
                    //for (int j = 0; j < Ratios.Count; j++) {
                    //    Sum += Ratios[j].Ratio;
                    //}
                    //Mascots[i].RefMatch.LongESICoef = Sum / (double)Ratios.Count;
                    //Медиана
                    //if (Ratios.Count % 2 != 0) {
                    //    Mascots[i].RefMatch.LongESICoef = Ratios[Ratios.Count / 2].Ratio;
                    //} else {
                    //    Mascots[i].RefMatch.LongESICoef = (Ratios[Ratios.Count / 2 - 1].Ratio + Ratios[Ratios.Count / 2].Ratio) / 2;
                    //}
                    sw.Write("{0}\t{1}\t", i, Mascots[i].MascotRT);
                    for (int j = 0; j < Ratios.Count; j++) {
                        sw.Write("{0}\t", Ratios[j].Ratio);
                    }
                    sw.WriteLine();
                    sw.Write("\t\t");
                    for (int j = 0; j < Ratios.Count; j++) {
                        sw.Write("{0}\t", (Math.Log10(Ratios[j].Score) - 4) / 10.0);
                    }
                    sw.WriteLine();
                    sw.Write("\t\t");
                    for (int j = 0; j < Ratios.Count; j++) {
                        sw.Write("{0}\t", Ratios[j].Score);
                    }
                    sw.WriteLine();
                    sw.Write("\t\t");
                    for (int j = 0; j < Ratios.Count; j++) {
                        sw.Write("{0}\t", Math.Pow(Ratios[j].Score, Pow));
                    }
                    sw.WriteLine();

                } else {
                    Mascots[i].RefMatch.LongESICoef=0.0;
                }
            }
            //если не удалось найти подобающее - ищем ближайшие для которых удалось
            for (int i=0 ; i < Mascots.Count ; i++){
                if (!Mascots[i].CheckExist()) continue;
                if (Mascots[i].RefMatch.LongESICoef == 0.0){
                    double Pred =0.0;
                    for (int j = i-1 ; j>=0  ;j-- ){
                        if (!Mascots[j].CheckExist()) continue;
                        if (Mascots[j].RefMatch.LongESICoef != 0.0){
                            Pred = Mascots[j].RefMatch.LongESICoef;
                            break;
                        }
                    }
                    double Post =0.0;
                    for (int j = i+1 ; j<Mascots.Count ; j++){
                        if (!Mascots[j].CheckExist()) continue;
                        if (Mascots[j].RefMatch.LongESICoef != 0.0){
                            Post = Mascots[j].RefMatch.LongESICoef;
                            break;
                        }
                    }
                    if (Post == 0.0) Post = Pred;
                    if (Pred == 0.0) Pred = Post;
                    Mascots[i].RefMatch.LongESICoef = (Post+Pred)/2.0;
                }
            }

            //накладываем изменения на Score

            for (int i=0 ; i < Mascots.Count ; i++){
                if (!Mascots[i].CheckExist()) continue;
                Mascots[i].RefMatch.Score *=  Mascots[i].RefMatch.LongESICoef;
            }

            sw.Close();
        }


        void FirstSearch(List<QPeptide> Peptides, int MatchIndex){
            double RTPercDelta = Convert.ToDouble(Settings.Default.RTErrorProc)/100;
            double RTMinutesDelta = Convert.ToDouble(Settings.Default.RTErrorMinutes);
            double rtMAX, rtMIN;
            for (int i1 = 0 ; i1 < Peptides.Count ; i1++ ){
                QPeptide data = Peptides[i1];
                if (data.AlreadySearched) continue;
                RepProgress((int)(((double)i1/(double)Peptides.Count)*100.0));
                if (data.Sequence != null){
                    data.TheorIsotopeRatio = Utils.IsotopicScore(data.Sequence);
                }else{
                    data.TheorIsotopeRatio = Utils.AveragineIsotopicScore((data.MascotMZ-1)*data.Charge);
                }
                rtMAX = data.MascotRT*(1+RTPercDelta)+RTMinutesDelta; 
                rtMIN = data.MascotRT*(1-RTPercDelta)-RTMinutesDelta; 
                data.Matches[MatchIndex] =  MSFile.FindBestScore(data.MascotMZ,data.Charge,data.TheorIsotopeRatio,rtMIN,rtMAX);
                data.RefMatch = data.Matches[MatchIndex];
                if (RefNumber >= 0 ) {
                    data.RefNumberMatch = data.Matches[RefNumber];
                }else{
                    data.RefNumberMatch = null;
                }
            }
        }

        void AlignOutLayers(List<QPeptide> Peptides, int MatchIndex, int Step, double Quota){
            Peptides.Sort(new QPeptide.byApexRT());
            //оценить - кто не на месте
            //имеет в виду - целиком загруженный файл
            double rtMAX, rtMIN;
            QPeptide data;
            int i1,Count;
            for (i1 = 0 ; i1 < Peptides.Count ; i1++){
                data = Peptides[i1];
                data.RTNumber = 0;
                for (int j = 0 ; j < Peptides.Count ; j++){
                    if (Peptides[j].Matches[MatchIndex] == null) continue;
                    if ((Peptides[j].MascotScan > data.MascotScan && j < i1  /*Matches[MatchIndex].ApexRT != 0.0 && Math.Abs(j-i1)<15*/ )||
                        (Peptides[j].MascotScan < data.MascotScan && j > i1  /*Matches[MatchIndex].ApexRT != 0.0 && Math.Abs(j-i1)<15*/ )){
                        data.RTNumber += Math.Abs(Peptides[j].MascotScan-data.MascotScan);
                    }
                }
            }

            //исключеаем с данные превышающие локальную медиану в окрестности +/-10 спектров
            for (i1 = 0 ; i1 < Peptides.Count ; i1++){
                Count = 0; 
                data = Peptides[i1];
                if (data.AlreadySearched) continue;
                for (int j = i1-Step ; j < i1 + Step ; j++){
                    if (j>=0 && j<Peptides.Count){
                        if (data.RTNumber > Peptides[j].RTNumber){
                            Count++;
                        }
                    }
                }
                if ((double)(Count)/(double)(Step*2) > 1-Quota ){
                    data.Matches[MatchIndex]= null;
                }
            }

            Peptides.Sort(new QPeptide.byMascotSN());

            double[] ApexRTs = new double[Peptides.Count];
            for (i1 = 0 ; i1 < Peptides.Count ; i1++ ){
                if (Peptides[i1].Matches[MatchIndex]==null){
                    ApexRTs[i1] = 0.0;
                }else{
                    ApexRTs[i1] = Peptides[i1].Matches[MatchIndex].ApexRT;
                }
            }

            for (i1 = 0 ; i1 < Peptides.Count ; i1++ ){
                data = Peptides[i1];
                if ((data.Matches[MatchIndex] != null) || data.AlreadySearched)  continue;
                RepProgress((int)(((double)i1/(double)Peptides.Count)*100.0));
                rtMIN = 1000000.0;
                rtMAX = 0.0;
                Count = 0;
                int MinCount;
                for (MinCount = i1 ; Count < Step ; MinCount--){
                    if (MinCount == 0) break;
                    if (ApexRTs[MinCount]!=0.0) Count++;
                }
                int MaxCount;
                Count = 0;
                for (MaxCount = i1 ; Count < Step ; MaxCount++){
                    if (MaxCount == Peptides.Count-1) break;
                    if (ApexRTs[MaxCount]!=0.0) Count++;
                }

                if (MinCount == 0) rtMIN = 0.0;
                if (MaxCount == Peptides.Count-1) rtMAX = 10000.0;

                for (int j = MinCount ; j <= MaxCount ; j++){
                    if (ApexRTs[j] != 0.0){
                        if (ApexRTs[j] > rtMAX ){
                            rtMAX = ApexRTs[j];
                        }
                        if (ApexRTs[j] < rtMIN  ){
                            rtMIN = ApexRTs[j];
                        }
                    }
                }

                data.Matches[MatchIndex] = MSFile.FindBestScore(data.MascotMZ,data.Charge,data.TheorIsotopeRatio,rtMIN,rtMAX);
                data.RefMatch = data.Matches[MatchIndex];
                if (RefNumber >= 0 ) {
                    data.RefNumberMatch = data.Matches[RefNumber];
                }else{
                    data.RefNumberMatch = null;
                }

            }
        }


        List<QPeptide> Mascots;
        List<QProtein> Proteins;

        public delegate void FilePlusDelegate(int FileNumber);

        public void FilePlus(int FileNumber){
           if (InvokeRequired) {
                // We're not in the UI thread, so we need to call BeginInvoke
                Invoke(new FilePlusDelegate(FilePlus), new object[]{FileNumber});
                return;
            }
            string FileName = RawList.Items[FileNumber].ToString();
            FileName = "+"+FileName;
            RawList.Items[FileNumber] = FileName;
            if (FileNumber>0){
                RawList.TopIndex = FileNumber-1;
            }
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

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e != null && e.Error != null){
                Log(e.Error.Message,MessageBoxIcon.Error,e.Error.StackTrace);
                ProgressLabel.Text = "Processing aborted! See log for details...";
                ProgressLabel.BackColor = Color.Red;
            }else{
                if (ErrorCounter == 0){
                    ProgressLabel.Text = "Processing complete!";
                    //RecalcButton.Enabled = true;
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
            //backgroundWorker1.RunWorkerAsync();
            try {
                backgroundWorker1_DoWork(null,null);
                backgroundWorker1_RunWorkerCompleted(null, null);
            }
            catch (Exception ex){
                RunWorkerCompletedEventArgs Ev= new RunWorkerCompletedEventArgs(null,ex,false);
                backgroundWorker1_RunWorkerCompleted(null, Ev);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
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
        }

        private void LockRefFeatures(bool Lock){
            textBox6.Enabled = !Lock;
            checkBox2.Enabled = !Lock;
            checkBox3.Enabled = !Lock;
            checkBox4.Enabled = !Lock;
            textBox7.Enabled  = !Lock;
        }

        private void WriteHeader(StreamWriter sw, string RawFileName){
	        sw.WriteLine(Text);
            sw.WriteLine("RAW: {0}\r\nDAT: {1}", RawFileName, DatFileName.Text);
            sw.WriteLine("REF: {0}", ReferenceFile);

	        sw.WriteLine("Filter Options: Mascot Score >=: {0}; Mass +/- ppm: {1}; RT: +/- ({2} %, {3} min); RT Order +/- {4}; Pept. per prot. >= {5} ", 
                Settings.Default.MascotScore,
                Settings.Default.MassError,
                Settings.Default.RTErrorProc,
                Settings.Default.RTErrorMinutes,
                Settings.Default.RTOrder,
                Settings.Default.PeptperProt);

            sw.WriteLine("Misc Options: {0} {1}", 
                Settings.Default.Deconvolution?"Charge deconvolution;":"",
                Settings.Default.MascotScoreFiltering?"Using Best Mascot Peptides;":"Ignoring Best Mascot Peptides;");

            sw.WriteLine("Ref Options: ESI Interval Correction (min):{1}; Dispersion Ratio threshold :{2}; Convolution threshold: {3}; {0} {5}", 
                Settings.Default.CutTails?"Cut Tails;":"",
                GetStringValue(Settings.Default.ESIInterval)==0.0?"None":Settings.Default.ESIInterval,
                GetStringValue(Settings.Default.DispThres)==0.0?"None":Settings.Default.DispThres,
                GetStringValue(Settings.Default.ConvThres)==0.0?"None":Settings.Default.ConvThres,
                Settings.Default.ZeroPValue,
                Settings.Default.Norm?"Normalization;":"");
        }

        private void AddRawButton_Click(object sender, EventArgs e) {
            if (RAWFileDialog.ShowDialog() != DialogResult.OK)  {
                return;
            }
            for (int i = 0 ; i < RAWFileDialog.FileNames.Length ; i++){
                if (RawList.Items.IndexOf(RAWFileDialog.FileNames[i]) == -1 && 
                    RawList.Items.IndexOf("+"+RAWFileDialog.FileNames[i]) == -1 ){
                    RawList.Items.Add(RAWFileDialog.FileNames[i]);
                }
            }
            
            RefCombo.Items.Clear();
            RefCombo.Items.Add("Matrix");
            RefCombo.Items.Add("None");
            for ( int i = 0 ; i < RawList.Items.Count ; i++){
                if (RawList.Items[i].ToString()[0] == '+'){
                    RefCombo.Items.Add(RawList.Items[i].ToString().Substring(1));
                }else{
                    RefCombo.Items.Add(RawList.Items[i]);
                }
            }
            RefCombo.Text = "Matrix";
            ReferenceFile = "Matrix";
            //RecalcButton.Enabled = false;
            progressBar1.Value = 0;
        }


        private void DeleteRawButton_Click(object sender, EventArgs e) {
            ListBox.SelectedObjectCollection ToDelete = RawList.SelectedItems;
            if ( RawList.SelectedItem != null && !RawList.SelectedItem.ToString().Contains("+")){
                ToDelete.Add(RawList.SelectedItem);
            }
            //проверка на наличие уже обработанных
            bool Flag = false;
            for ( int i = 0 ; i < ToDelete.Count ; i++){
                if (ToDelete[i].ToString().Contains("+") && !Flag){
                    Flag = (MessageBox.Show("You are going to delete from the list already processed file(s).\n"+
                                    "All quantification results for this file(s) will be lost.\n"+
                                    "Are you sure you wish to delete this file(s)?","Warning",
                                    MessageBoxButtons.OKCancel,MessageBoxIcon.Exclamation) == DialogResult.OK);
                    if (!Flag) return;
                }
            }

            //цикл удаления из базы данных 
            for ( int i = 0 ; i < ToDelete.Count ; i++){
                if (ToDelete[i].ToString().Contains("+")){
                    db.DeleteFile(RawList.Items.IndexOf(ToDelete[i]));
                }
            }

            //если данные были удалены из базы данных - то следует перезагрузка всех данных 
            if (Flag){
                MessageBox.Show("After deleting already processed files Quanty will be closed down. \n You should restart it.","Quanty is closing down",
                    MessageBoxButtons.OK,MessageBoxIcon.Information);
                Application.Exit();
            }


            //цикл удаления из списка для обработки 
            for ( int i = 0 ; i < ToDelete.Count ; i++){
                RawList.Items.Remove( ToDelete[i]);
            }

            //цикл пересоставления возможных ссылочных файлов            
            progressBar1.Value = 0;
            RefCombo.Items.Clear();
            RefCombo.Items.Add("Matrix");
            RefCombo.Items.Add("None");
            for ( int i = 0 ; i < RawList.Items.Count ; i++){
                if (RawList.Items[i].ToString()[0] == '+'){
                    RefCombo.Items.Add(RawList.Items[i].ToString().Substring(1));
                }else{
                    RefCombo.Items.Add(RawList.Items[i]);
                }
            }
            RefCombo.Text = "Matrix";
            ReferenceFile = "Matrix";
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


        private void ReportWork(){
            Log("Report generation started...");
            for (int i = 0 ; i < Proteins.Count ; i++){
                Proteins[i].AllPeptCount = 0;
                Proteins[i].ZeroCount = 0;
            }
            if (Settings.Default.Matrix){
                for (int i = 0 ; i < RawList.Items.Count ; i++){
                    RefNumber = i;
                    for (int j = 0 ; j < RawList.Items.Count ; j++){
                        ReferenceProcessing(j);
                    }
                    Log(String.Format("Reference processing sample {0} ",i));
                    //if (PrintAllPeptides)
                    //    PrintPeptideShit();
                    //if (PrintAllProteins)
                    //    PrintProteinShit();
                }
                PrintPeptideMatrix();
                PrintProteinMatrix();
            }else{
                for (int i = 0 ; i < RawList.Items.Count ; i++){
                    if (RefNumber < 0 ){
                        for (int j = 0 ; j < Mascots.Count ; j++){
                            if (Mascots[j].Matches[i]!=null){
                                Mascots[j].Matches[i].MaxConv = 0.0;
                                Mascots[j].Matches[i].Shift = 0.0;
                                Mascots[j].Matches[i].LongESICoef = 0.0;
                                Mascots[j].Matches[i].ShitRatio = 0.0;
                            }
                            Mascots[j].RefMatch = Mascots[j].Matches[i];
                            Mascots[j].RefNumberMatch = null;
                        }
                        for ( int i1 = 0 ; i1 < Proteins.Count ; i1++ ){
                            Proteins[i1].MakeStats(false);
                        }
                        PrintFiles(i);
                    }else{
                        ReferenceProcessing(i);
                        PrintFiles(i);
                    }
                    Log(String.Format("Report processing sample {0} ",i));
                }
                if (RefNumber>=0){
                    if (PrintAllPeptides)
                        PrintPeptideShit();
                    if (PrintAllProteins)
                        PrintProteinShit();
                }

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
                if (  String.Compare(RefCombo.Text,RawList.Items[i].ToString()) == 0
                    ||String.Compare(RefCombo.Text,RawList.Items[i].ToString().Substring(1)) == 0 ){
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
        private string ReportAllPeptString;
        private bool PrintAllProteins;
        private string ReportAllProtString;
        private bool RepSettingsLoading;

        private void SaveReportVector(){
            Settings.Default.ReportVector = "";
            for (int i = 0 ; i < ReportTree.Nodes.Count ; i++){
                Settings.Default.ReportVector += (ReportTree.Nodes[i].Checked)?"1":"0";
                for (int j = 0 ; j <ReportTree.Nodes[i].Nodes.Count ; j++){
                    Settings.Default.ReportVector += (ReportTree.Nodes[i].Nodes[j].Checked)?"1":"0";
                }
            }
            PrintPeptides = (Settings.Default.ReportVector[0] == '1');
            PrintProteins = (Settings.Default.ReportVector[25] == '1');
            PrintAllPeptides = (Settings.Default.ReportVector[39] == '1');
            PrintAllProteins = (Settings.Default.ReportVector[45] == '1');
            QPeptide.ReportString = Settings.Default.ReportVector.Substring(1,24);
            QProtein.ReportString = Settings.Default.ReportVector.Substring(26,13);
            ReportAllPeptString = Settings.Default.ReportVector.Substring(40,5);
            ReportAllProtString = Settings.Default.ReportVector.Substring(46,10);
        }

        private void LoadReportVector(){
            RepSettingsLoading = true;
            int Count = 0;
            for (int i = 0 ; i < ReportTree.Nodes.Count  ; i++){
                if ( Count < Settings.Default.ReportVector.Length ){
                    ReportTree.Nodes[i].Checked = (Settings.Default.ReportVector[Count] == '1');
                    Count++;
                    for (int j = 0 ; j < ReportTree.Nodes[i].Nodes.Count  ; j++){
                        if ( Count < Settings.Default.ReportVector.Length ){
                            ReportTree.Nodes[i].Nodes[j].Checked = (Settings.Default.ReportVector[Count] == '1');
                            Count++;
                        }
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
            Names.Add("RTOrder");
            Values.Add(textBox8.Text);
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
            if (Names.IndexOf("RTOrder")!=-1) {
                textBox8.Text = Values[Names.IndexOf("RTOrder")];
            }else{
                textBox8.Text = "30";
            }
        }

        private void  LoadFileList(){
            List<string> FileNames = null;
            db.LoadFileList(ref FileNames);
            RawList.Items.Clear();
            for (int i = 0 ; i<FileNames.Count ; i++ ){
                RawList.Items.Add("+"+FileNames[i]);
            }
            RefCombo.Items.Clear();
            RefCombo.Items.Add("Matrix");
            RefCombo.Items.Add("None");
            for ( int i = 0 ; i < RawList.Items.Count ; i++){
                RefCombo.Items.Add(RawList.Items[i].ToString().Substring(1));
            }
            RefCombo.Text = "Matrix";
            ReferenceFile = "Matrix";
            //RecalcButton.Enabled = true;
            progressBar1.Value = 0;
        }

        private void LoadData(){
            Proteins = new List<QProtein>();
            db.LoadProteins(Proteins);
            Mascots = new List<QPeptide>();
            if (false /*Settings.Default.QuantiAll*/ ){
                db.LoadMascots(Proteins,Mascots);
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
            for (int i = 0 ; i < MSMSList.Count ; i++ ){
                MSMSList[i].Matches = new QMatch[RawList.Items.Count];
                for (int j = 0 ; j < RawList.Items.Count ; j++){
                    MSMSList[i].Matches[j] = null;
                }
                MSMSList[i].ReferenceMatrix = new double[RawList.Items.Count,RawList.Items.Count];
            }
            db.LoadQMatches(MSMSList);
            db.LoadMSMatches(MSMSList);
            mp = null;
            for (int i = 0 ; i < Proteins.Count ; i++ ){
                Proteins[i].CreateMatches(RawList.Items.Count);
            }
            Log("Loading completed");
            RepProgress(0);

        }

        private double[] SolveMatrix(double[,] Matrix, ref double[,] ErrorMatrix){
            int Len = Matrix.GetLength(0);
            double[] Res = new double[Len];
            for (int i = 0 ; i < Len ; i++){
                Res[i] = 1.0;
            }
            int Count,SCount = 0;
            for (int i = 0 ; i < Len ; i++){
                Count = 0;
                for (int j = 0 ; j < Len ; j++){
                    if (Matrix[i,j]!=0.0){
                        Res[i] *=  Matrix[i,j];
                        Count++;
                    }
                }
                if (Count>0){
                    Res[i] = Math.Pow(Res[i],1.0/(double)Count);
                    if (Res[i] == 1.0) {
                        SCount++;
                    }
                }else{
                    Res[i] = 0.0;
                }
            }
            double[,] Errors = new double[Len,Len];
            double[] Adj = new double[Len];
            for (int i = 0 ; i < Len ; i++){
                for (int j = 0 ; j < Len ; j++){
                    if (Matrix[i,j]!=0.0){
                        Errors[i,j] = Matrix[i,j]*Res[j]-Res[i];
                    }else{
                        Errors[i,j] = 0.0;
                    }
                }
            }
            if (ErrorMatrix != null){
                ErrorMatrix = Errors;
            }

            return Res;
        }

        private void PrintPeptideMatrix(){
            string FileName = OutPathName.Text+Path.DirectorySeparatorChar+Path.GetFileNameWithoutExtension(DatFileName.Text)+"_PeptMatrix.txt";
            StreamWriter pepshit = new StreamWriter(FileName);
            WriteHeader(pepshit,"Peptide Matrix file");
            pepshit.Write(
                ((ReportAllPeptString[0]!='1')?"":"SEQUENCE\t")+
                ((ReportAllPeptString[1]!='1')?"":"PROTEIN ID\t")+
                "PROTEIN DESC\t"+
                ((ReportAllPeptString[2]!='1')?"":"REF ABUNDANCE\t")+
                ((ReportAllPeptString[3]!='1')?"":"MODIF\t"));
            for (int i = 0 ; i < RawList.Items.Count ; i++){
                pepshit.Write(Path.GetFileName(RawList.Items[i].ToString())+"\t");
            }
            pepshit.WriteLine();

            Mascots.Sort(new QPeptide.bySet());

            for(int j = 0; j<Mascots.Count ; j++){
                int Count = 0;
                Mascots[j].MatrixScore = 0.0;
                for (int i = 0 ; i < RawList.Items.Count ; i++){
                    if (Mascots[j].Matches[i] != null && Mascots[j].Matches[i].Score > 0.0){
                        Mascots[j].MatrixScore+=Mascots[j].Matches[i].Score;
                        Count++;
                    }
                }
                Mascots[j].MatrixScore = Mascots[j].MatrixScore/(double)Count;
                string ProtName = "";
                for (int i = 0 ; i < Proteins.Count ; i++){
                    if (Proteins[i].ipi == Mascots[j].IPI) {
                        ProtName = Proteins[i].Desc;
                    }
                }
                pepshit.Write(
                    ((ReportAllPeptString[0]!='1')?"":String.Format("{0}\t",Mascots[j].Sequence))+
                    ((ReportAllPeptString[0]!='1')?"":String.Format("{0}\t",Mascots[j].IPI))+
                    ProtName+"\t"+
                    ((ReportAllPeptString[0]!='1')?"":String.Format("{0:f2}\t",Mascots[j].MatrixScore))+
                    ((ReportAllPeptString[0]!='1')?"":String.Format("{0}\t",(Mascots[j].ModMass!=0.0 ? Mascots[j].ModDesk : ""))));

                double [,] FakeMatrix = null;
                double[] Res = SolveMatrix(Mascots[j].ReferenceMatrix,ref FakeMatrix);

                for (int i = 0 ; i < RawList.Items.Count ; i++){
                    pepshit.Write("{0:f5}\t",Res[i]);
                    if (Mascots[j].Matches[i] != null){
                        Mascots[j].Matches[i].ShitRatio = Res[i];
                    }
                }
                pepshit.WriteLine();
            }
            pepshit.Close();
        }

        private void PrintProteinMatrix(){
            string FileName = OutPathName.Text+Path.DirectorySeparatorChar+Path.GetFileNameWithoutExtension(DatFileName.Text)+"_ProtMatrix.txt";;
            StreamWriter protshit = new StreamWriter(FileName);
            WriteHeader(protshit,FileName);

            protshit.Write(
                ((ReportAllProtString[0]!='1')?"":"PROTEIN ID\t")+
                ((ReportAllProtString[1]!='1')?"":"REF ABUNDANCE\t")+
                ((ReportAllProtString[2]!='1')?"":"DESCRIPTION\t")+
                ((ReportAllProtString[3]!='1')?"":"PEPTIDES\t"));

            for (int i = 0 ; i < RawList.Items.Count ; i++){
               protshit.Write(Path.GetFileName(RawList.Items[i].ToString())+"\t");
            }

            protshit.Write("\t");
            for (int i = 0 ; i < RawList.Items.Count ; i++){
               protshit.Write(Path.GetFileName(RawList.Items[i].ToString())+"-Maximum \t");
            }

            protshit.Write("\t");
            for (int i = 0 ; i < RawList.Items.Count ; i++){
               protshit.Write(Path.GetFileName(RawList.Items[i].ToString())+"-Minimum \t");
            }

            protshit.WriteLine();

            Proteins.Sort(new QProtein.byIPI());

            for(int j = 0; j<Proteins.Count ; j++){
                double AveScore = 0.0;
                int Count = 0;
                bool flag;

                for ( int i = 0 ; i < RawList.Items.Count ; i++){
                    flag = false;
                    for (int k = 0 ; k < Proteins[j].Peptides.Count ; k++){
                        if (Proteins[j].Peptides[k].Matches[i] != null && 
                            Proteins[j].Peptides[k].Matches[i].Score > 0.0){
                            AveScore += Proteins[j].Peptides[k].Matches[i].Score;
                            flag = true;
                        }
                    }
                    if (flag) Count++;
                }
                AveScore = AveScore / (double)Count;

                protshit.Write(
                    String.Format("{0}\t",Proteins[j].ipi)+
                    String.Format("{0:f2}\t",AveScore)+
                    String.Format("{0}\t",Proteins[j].Desc)+
                    String.Format("{0}\t",Proteins[j].Peptides.Count));

                double[] Res;
                double[] Slopes;
                double [,] FakeMatrix = null;
                //вариант вычисления матрицы из белков 
                Res = SolveMatrix(Proteins[j].MatrixMedian,ref FakeMatrix );
                Slopes = SolveMatrix(Proteins[j].MatrixSlope,ref Proteins[j].MatrixError);
                double[] Errs = SolveMatrix(Proteins[j].SlopeError,ref FakeMatrix);
                double[] MErrs = new double[RawList.Items.Count];
                //обрабатываем матрицу ошибок матрицы
                for ( int i = 0 ; i < RawList.Items.Count ; i++){
                    double AveX = 0.0;
                    Count = 0;
                    for (int k = 0 ; k < RawList.Items.Count ; k++){
                        if ( Proteins[j].MatrixError[i,k] != 0.0 ) { 
                            AveX += Proteins[j].MatrixError[i,k];
                            Count++;
                        }
                    }
                    if (Count<2){
                        MErrs[i] = 0.0;
                        continue;
                    }
                    AveX = AveX/Count;
                    double SumD = 0.0; 
                    for (int k = 0 ; k < RawList.Items.Count ; k++){
                        if ( Proteins[j].MatrixError[i,k] != 0.0 ) { //уточнить !! [i,k] или [k,i]
                            SumD += (Proteins[j].MatrixError[i,k]-AveX)*(Proteins[j].MatrixError[i,k]-AveX);
                        }
                    }
                    MErrs[i]= (Math.Sqrt(SumD)/(Count-1))*alglib.studenttdistr.invstudenttdistribution(Count-1,0.833);
                }

                //вариант матрицы из матриц пептидов - линейной регрессией 
                //расчет наклона 
                //Res = new double[RawList.Items.Count];
                //double[] Delta = new double[RawList.Items.Count];
                //for( int i = 0 ; i < RawList.Items.Count; i++){
                //    double Sum1= 0.0, Sum2 = 0.0, SumX = 0.0, X, Y;
                //    Count = 0;
                //    for (int k = 0 ; k < Proteins[j].Peptides.Count ; k++){
                //        if (Proteins[j].Peptides[k].Matches[i] != null && 
                //            Proteins[j].Peptides[k].Matches[i].ShitRatio != 0.0){
                //            X = Proteins[j].Peptides[k].MatrixScore;
                //            Y = X * Proteins[j].Peptides[k].Matches[i].ShitRatio;
                //            Sum1 += X*Y;
                //            Sum2 += Y*Y;
                //            SumX += X;
                //            Count++;
                //        }
                //    }
                //    if (Sum2 != 0){
                //        Res[i]= Sum2/Sum1;
                //        if (Count >= 2){
                //            double SumXD = 0.0, SumYD = 0.0;
                //            double AveX = SumX / Count;
                //            for( int k = 0 ; k < Proteins[j].Peptides.Count; k++){
                //                if (Proteins[j].Peptides[k].Matches[i] != null && 
                //                    Proteins[j].Peptides[k].Matches[i].ShitRatio != 0.0){
                //                    X = Proteins[j].Peptides[k].MatrixScore;
                //                    Y = X * Proteins[j].Peptides[k].Matches[i].ShitRatio;
                //                    SumXD += (X-AveX)*(X-AveX);
                //                    SumYD += (Y - X*Res[i])*(Y - X*Res[i]);
                //                }
                //            }
                //            Delta[i] = Math.Sqrt((SumYD)/(SumXD*(Count-1)))*alglib.studenttdistr.invstudenttdistribution(Count-1,0.975);
                //        }else{
                //            Delta[i] = 0.0;
                //        }
                //    }else{
                //        Res[i] = 0.0;
                //        Delta[i] = 0.0;
                //    }
                //
                //}



                double ZeroSubst = 0.0;

                for (int i = 0 ; i < Res.GetLength(0) ; i++){
                    double PValue = (double)Proteins[j].ZeroCount/(double)Proteins[j].AllPeptCount;
                    PValue = Math.Pow(PValue,Proteins[j].Peptides.Count);
                    if (PValue > GetStringValue(Settings.Default.ZeroPValue)){
                        ZeroSubst = 1.0;
                    }
                }
                for (int i = 0 ; i < RawList.Items.Count ; i++){
                    if (Res[i]!=0.0){
                        protshit.Write("{0:f5}\t",Res[i]);
                    }else{
                        protshit.Write("{0:f5}\t",ZeroSubst);
                    }
                }
                protshit.Write("\t");
                for (int i = 0 ; i < RawList.Items.Count ; i++){
                    protshit.Write("{0:f5}\t",Slopes[i]*(1+Errs[i]+MErrs[i])-Res[i]);
                    //protshit.Write("{0:f5}\t",Slopes[i]*(1+Errs[i]+MErrs[i])-Slopes[i]);
                }
                protshit.Write("\t");
                for (int i = 0 ; i < RawList.Items.Count ; i++){
                    protshit.Write("{0:f5}\t",Res[i]-Slopes[i]*(1-Errs[i]-MErrs[i]));
                    //protshit.Write("{0:f5}\t",Slopes[i]-Slopes[i]*(1-Errs[i]-MErrs[i]));
                }

                protshit.WriteLine();
            }
            protshit.Close();

        }



    }
}




//TODO:
//1 - сделать распределение R-square по выходному файлу - DONE
//2 - вытащить на уровень RawFileBox значения тока и промежутка между сканами - 
//3 - проверить что происходит с распределением R-Square после коррекции 
//4 - Вытащить сырой матч на уровень QMatch 
//5 - начать нарабатывать библиотеку сопоставления QMatch
//6 - FDR
//Матрица несовместности в cluster MGF - если значения в ней достаточно велики - выравниваения не произошло 

