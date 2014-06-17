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
using System.Linq;
using System.Text;
using System.IO;

namespace QuantiProcess
{

    public interface ILogAndProgress
    {
        void Log(string Message);
        void Log(string Message, System.Windows.Forms.MessageBoxIcon WarrningLevel, string StackInfo);
        void ProgressMessage(string Message);
        void RepProgress(int Perc);
    }

    class Program
    {

        //три параметра - 
        //1 - имя базы данных 
        //2 - имя RawFile
        //3 - идентификатор файла в базе данных
        public static TProcessor Processor;
        static int Main(string[] args)
        {
            try{
                //проверить существование и расширения 
                Console.ReadLine();
                Processor = new TProcessor(args[0], args[1], Convert.ToInt32(args[2]));
                Processor.DBLoad();
                Processor.LoadRaw();
                Processor.FeaturesSearch();
                Processor.DBSave();
                Console.WriteLine("Completed");
                Console.ReadLine();
            }catch(Exception e){
                Console.Write("Error:");
                Console.Write(e.Message);
                Console.WriteLine("STACKINFO:"+e.StackTrace);
                return 1;
            }
            return 0;
        }
    }

    public class TProcessor : ILogAndProgress{
        //параметры Quanti
        public double MascotScore;
        public double MassError;
        public double RTErrorProc;
        public double RTErrorMinutes;
        public int RTOrder;
        public bool Deconvolution;
        public bool QuantyAll;

        public bool StickMode;
        public bool MetaProfile;
        public double MaxRTWidth;
        public double MinRTWidth;
        public double RTRes;

        string DBName;
        string RawFileName;
        int RawFileNumber;

        List<QPeptide> Mascots;
        List<QPeptide> MSMSList;


        public TProcessor(String DBName, String RawFileName, int RawFileNumber){
            this.DBName = DBName;
            this.RawFileName = RawFileName;
            this.RawFileNumber = RawFileNumber;
            Mascots = new List<QPeptide>();
        }

        DBInterface db;
        RawFileService MSFile;

        public void DBLoad(){
            DBInterface.iLog = this;
            db = new DBInterface();
            db.ConnectTo(DBName);
            LoadSettings();
            Mascots = new List<QPeptide>();
            db.LoadMascots(Mascots);
            if (QuantyAll ){
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
                MSMSList = Mascots;
            }

        }

        private void LoadSettings(){
            List<string> Names = new List<string>();
            List<string> Values = new List<string>();
            db.LoadSettings(ref Names, ref Values);

            MascotScore     = Convert.ToDouble(Values[Names.IndexOf("MascotScoreThres")]);
            MassError       = Convert.ToDouble(Values[Names.IndexOf("MassError")]);
            RTErrorProc     = Convert.ToDouble(Values[Names.IndexOf("RTPercents")]);
            RTErrorMinutes  = Convert.ToDouble(Values[Names.IndexOf("RTMinutes")]);
            RTOrder         = Convert.ToInt32(Values[Names.IndexOf("RTOrder")]);

            Deconvolution = (Values[Names.IndexOf("ChargeDeconvo")] == "Yes");
            QuantyAll = Values[Names.IndexOf("QuantiAll")] == "Yes";

            StickMode = Values[Names.IndexOf("StickMode")] == "Yes";
            MetaProfile = Values[Names.IndexOf("MetaProfile")] == "Yes";
            MaxRTWidth = Convert.ToDouble(Values[Names.IndexOf("MaxRTWidth")]);
            MinRTWidth = Convert.ToDouble(Values[Names.IndexOf("MinRTWidth")]);
            RTRes = Convert.ToDouble(Values[Names.IndexOf("RTRes")]);
            if (MaxRTWidth == 0.0) MaxRTWidth = 100.0;
        }

        public void LoadRaw(){

            RawFileService.RepProgress = RepProgress;
            MSFile = new RawFileService();
            MSFile.LoadIndex(RawFileName);
            MSFile.RTCorrection = true;

            Log(string.Format("Loading..."));

            //на выходе пептиды упорядочены по FSN (в смыслше по порядку выхода)
            //обработка пептидов
            double MinRT,MaxRT;

            MinRT = 0.0;
            int MaxInd = MSFile.RawSpectra.GetLength(0)-1;
            if (MSFile.RawSpectra[MaxInd].RT == 0){
                MaxInd = MSFile.ms2index[MaxInd];
            }
            MaxRT = MSFile.RawSpectra[MaxInd].RT;

            MSFile.LoadInterval(MinRT,MaxRT);

        }

        public void FeaturesSearch(){
            FeatureSearch.iLog = this;
            FeatureSearch fs = new FeatureSearch(MSFile);
            Log("Initial search");
            fs.FirstSearch(Mascots);
            if (RTOrder>0){
                Log("Outlayers Alignment - First iteration");
                fs.AlignOutLayers(Mascots,RTOrder/4,0.5);
                Log("Outlayers Alignment - Second iteration");
                fs.AlignOutLayers(Mascots,RTOrder/2,0.25);
                Log("Outlayers Alignment - Third iteration");
                fs.AlignOutLayers(Mascots,RTOrder,0.0);
            }
            //PrintList(Mascots);
            if (QuantyAll){
                Log("Initial search of all MS/MS spectra");
                //пометить Mascots как уже отысканные 
                for (int j = 0 ; j < Mascots.Count ; j++){
                    Mascots[j].AlreadySearched = true;
                }
                fs.FirstSearch(MSMSList);
                if (RTOrder>0){
                    Log("Outlayers Alignment of all MS/MS spectra - First iteration");
                    fs.AlignOutLayers(MSMSList,RTOrder/2,0.5);
                    Log("Outlayers Alignment of all MS/MS spectra - Second iteration");
                    fs.AlignOutLayers(MSMSList,RTOrder,0.25);
                    Log("Outlayers Alignment of all MS/MS spectra - Third iteration");
                    fs.AlignOutLayers(MSMSList,RTOrder*2,0.0);
                }
            }
        }

        //чисто отладочный принт 
        public void PrintList(List<QPeptide> List){
            List.Sort(new QPeptide.byMascotSN());
            StreamWriter sw = new StreamWriter(Path.GetFileNameWithoutExtension(RawFileName)+".pept");
            for (int i = 0; i < List.Count; i++ ){
                if (List[i].Match != null){
                    if (List[i].MinSearch != 0.0){
                        sw.WriteLine("{0}\t{1}\t{2}\t{3}", List[i].Sequence, List[i].Match.ApexRT, List[i].Match.ApexRT - List[i].MinSearch, List[i].MaxSearch - List[i].Match.ApexRT);
                    }else{
                        sw.WriteLine("{0}\t{1}\t{2}\t{3}", List[i].Sequence, List[i].Match.ApexRT, 0.0, 0.0);
                    }
                }else{
                    sw.WriteLine("{0}\t{1}\t{2}\t{3}", List[i].Sequence, 0.0, 0.0, 0.0); 
                }
            }
            sw.Close();

        }

        public void DBSave(){
            int RepeatCount = 0; 
            while(RepeatCount < 10){
                try{
                    db.BeginTran();
                    db.AddFile(RawFileNumber,RawFileName);
                    db.SaveQMatches(MSMSList,RawFileNumber);
                    db.SaveMSMatches(MSMSList,RawFileNumber);
                    db.Commit();
                    RepeatCount = 10;
                }catch(System.Data.SQLite.SQLiteException sqle){
                    if (sqle.ErrorCode == System.Data.SQLite.SQLiteErrorCode.Busy && RepeatCount<10){
                        Log(sqle.Message, System.Windows.Forms.MessageBoxIcon.Warning, null);
                        System.Threading.Thread.Sleep(1000);
                        RepeatCount++;
                    }else{
                        throw sqle;
                    }
                }
            }
        }



        //Log Implementation 
        public void Log(string Message){
            Log(Message,System.Windows.Forms.MessageBoxIcon.Information,null);
        }

        public void Log(string Message, System.Windows.Forms.MessageBoxIcon WarrningLevel, string StackInfo){
            switch (WarrningLevel){
                case System.Windows.Forms.MessageBoxIcon.Information :
                    Console.Write("Information:");
                    break;
                case System.Windows.Forms.MessageBoxIcon.Warning :
                    Console.Write("Warning:");
                    break;
                case System.Windows.Forms.MessageBoxIcon.Error :
                    Console.Write("Error:");
                    break;
            }
            Console.Write(Message);
            if (StackInfo != null){
                Console.WriteLine("STACKINFO:"+StackInfo);
            }else{
                Console.WriteLine();
            }
        }

        public void ProgressMessage(string Message){
            Console.WriteLine(Message);
        }

        public void RepProgress(int Perc){
            Console.WriteLine("{0}%...",Perc);
        }


    }
}
