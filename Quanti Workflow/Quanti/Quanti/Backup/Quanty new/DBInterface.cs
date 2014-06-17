using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Data.SQLite;
using Quanty.Properties;
using Mascot;

namespace Quanty {
    class DBInterface{

        SQLiteConnection con;
        SQLiteTransaction tran;
        public static ILogAndProgress iLog;

        List<int> FileNumbers;

        String FileName;

        public string GetFileName(){
            return FileName;
        }

        public void ConnectTo(string DBFileName){
            FileName = DBFileName;
            con = new SQLiteConnection(String.Format("Data Source ={0}",DBFileName));
            con.Open();
            FileNumbers = new List<int>();
        }

        public void CreateNewDB(string DBFileName){
            FileName = DBFileName;
            FileNumbers = new List<int>();
            SQLiteConnection.CreateFile(DBFileName);
            con = new SQLiteConnection(String.Format("Data Source ={0}",DBFileName));
            con.Open();
            SQLiteCommand com = new SQLiteCommand(
                "CREATE TABLE Proteins (  "+
                    "IPI    VARCHAR( 20 ),  "+
                    "ipis   VARCHAR( 128 ),  "+
                    "Name   VARCHAR( 20 ), "+
                    "[Desc] TEXT( 1024 ) );",con);
            com.ExecuteNonQuery();
            com = new SQLiteCommand(
                "CREATE TABLE Mascots ( "+
                    "MascotScan    INTEGER, "+
                    "MascotMZ      REAL, "+
                    "MascotScore   REAL, "+
                    "MascotRT      REAL, "+
                    "TheorIsoRatio REAL, "+
                    "Charge        INTEGER, "+
                    "IPI           VARCHAR( 20 ), "+
                    "ipis          VARCHAR( 128 ), "+
                    "Sequence      VARCHAR( 64 ), "+
                    "Peptides      INT, "+
                    "ModMass       REAL, "+
                    "ModDesc       VARCHAR( 128 ), "+
                    "[Case]        VARCHAR( 128 )  "+
                    " );",con);
            com.ExecuteNonQuery();
            com = new SQLiteCommand(
                "CREATE TABLE AllMSMS ( "+
                    "MascotScan    INTEGER, "+
                    "MascotMZ      REAL, "+
                    "MascotScore   REAL, "+
                    "MascotRT      REAL, "+
                    "TheorIsoRatio REAL, "+
                    "Charge        INTEGER, "+
                    "IPI           VARCHAR( 20 ), "+
                    "ipis          VARCHAR( 128 ), "+
                    "Sequence      VARCHAR( 64 ), "+
                    "Peptides      INT, "+
                    "ModMass       REAL, "+
                    "ModDesc       VARCHAR( 128 ), "+
                    "[Case]        VARCHAR( 128 )  "+
                    " );",con);
            com.ExecuteNonQuery();
            com = new SQLiteCommand(
                "CREATE TABLE RawFiles (  "+
                "   FileNumber INTEGER,  "+
                "   FileName   VARCHAR( 256 ) ); ",con);
            com.ExecuteNonQuery();
            com = new SQLiteCommand(
                "CREATE TABLE QMatches (  "+
                "    FileNumber   INTEGER, "+
                "    MascotScan     INTEGER, "+
                "    ApexRT       REAL, "+
                "    ApexScore    REAL, "+
                "    ApexMZ       REAL, "+
                "    ApexIndex    INTEGER, "+
                "    Score        REAL, "+
                "    IsotopeRatio REAL, "+
                "    RTDisp REAL);",con);
            com.ExecuteNonQuery();
            com = new SQLiteCommand(
                "CREATE TABLE MSMatches (  "+
                "    FileNumber    INT, "+
                "    MascotScan    INTEGER, "+
                "    Charge        INTEGER, "+
                "    Score         REAL, "+
                "    MZ            REAL, "+
                "    RT            REAL, "+
                "    FirstIsotope  REAL, "+
                "    SecondIsotope REAL, "+
                "    TimeCoeff    REAL );",con);
            com.ExecuteNonQuery();
            com = new SQLiteCommand(
                "CREATE TABLE Settings ( "+
                "    Name  VARCHAR( 40 ), "+
                "    Value VARCHAR( 64 ) ); ",con);
            com.ExecuteNonQuery();
        }
        
        public void SaveProteins(List<QProtein> Proteins){
            tran = con.BeginTransaction();
            SQLiteCommand Insert = new SQLiteCommand(
                "INSERT INTO Proteins (IPI, ipis , Name , [Desc] ) "+
                "Values ( @IPI , @Ipis , @Name , @Desc)",con);
            SQLiteParameter IPI = new SQLiteParameter("@IPI");
            Insert.Parameters.Add(IPI);
            SQLiteParameter Ipis = new SQLiteParameter("@Ipis");
            Insert.Parameters.Add(Ipis);
            SQLiteParameter Name = new SQLiteParameter("@Name");
            Insert.Parameters.Add(Name);
            SQLiteParameter Desc = new SQLiteParameter("@Desc");
            Insert.Parameters.Add(Desc);

            for (int i = 0 ; i < Proteins.Count ; i++){
                string IpisStr = "";
                for (int j = 0 ; j < Proteins[i].ipis.Count ; j++){
                    IpisStr += Proteins[i].ipis[j]+";";
                }
                IPI.Value = Proteins[i].ipi;
                Ipis.Value = IpisStr;
                Name.Value = Proteins[i].Name;
                Desc.Value = Proteins[i].Desc;
                Insert.ExecuteNonQuery();
            }
            tran.Commit();
        }

        public void SaveMSMS(List<QPeptide> Peptides, bool Mascot){
            tran = con.BeginTransaction();
            SQLiteCommand Insert = new SQLiteCommand(
                "INSERT INTO "+(Mascot?"Mascots":"AllMSMS")+" (MascotScan, MascotMZ, MascotScore, "+
                "MascotRT, TheorIsoRatio, Charge, IPI, ipis, Sequence, Peptides, "+
                "ModMass, ModDesc, [Case]) "+
                "Values (  @MascotScan, @MascotMZ, @MascotScore, "+
                "@MascotRT, @TheorIsoRatio, @Charge, @IPI, @ipis, @Sequence, @Peptides, "+
                "@ModMass, @ModDesc, @Case)",con);

            SQLiteParameter MascotScan = new SQLiteParameter("@MascotScan");
            SQLiteParameter MascotMZ = new SQLiteParameter("@MascotMZ");
            SQLiteParameter MascotScore = new SQLiteParameter("@MascotScore");
            SQLiteParameter MascotRT = new SQLiteParameter("@MascotRT");
            SQLiteParameter TheorIsoRatio = new SQLiteParameter("@TheorIsoRatio");
            SQLiteParameter Charge = new SQLiteParameter("@Charge");
            SQLiteParameter IPI = new SQLiteParameter("@IPI");
            SQLiteParameter ipis = new SQLiteParameter("@ipis");
            SQLiteParameter Sequence = new SQLiteParameter("@Sequence");
            SQLiteParameter PeptidesNum = new SQLiteParameter("@Peptides");
            SQLiteParameter ModMass = new SQLiteParameter("@ModMass");
            SQLiteParameter ModDesc = new SQLiteParameter("@ModDesc");
            SQLiteParameter Case = new SQLiteParameter("@Case");

            Insert.Parameters.Add(MascotScan);
            Insert.Parameters.Add(MascotMZ);
            Insert.Parameters.Add(MascotScore);
            Insert.Parameters.Add(MascotRT);
            Insert.Parameters.Add(TheorIsoRatio);
            Insert.Parameters.Add(Charge);
            Insert.Parameters.Add(IPI);
            Insert.Parameters.Add(ipis);
            Insert.Parameters.Add(Sequence);
            Insert.Parameters.Add(PeptidesNum);
            Insert.Parameters.Add(ModMass);
            Insert.Parameters.Add(ModDesc);
            Insert.Parameters.Add(Case);

            for (int i = 0 ; i < Peptides.Count ; i++){
                string IpisStr = "";
                for (int j = 0 ; j < Peptides[i].IPIs.Count ; j++){
                    IpisStr += Peptides[i].IPIs[j]+";";
                }
                MascotScan.Value = Peptides[i].MascotScan;
                MascotMZ.Value = Peptides[i].MascotMZ;
                MascotScore.Value = Peptides[i].MascotScore;
                MascotRT.Value = Peptides[i].MascotRT;
                TheorIsoRatio.Value = Peptides[i].TheorIsotopeRatio;
                Charge.Value = Peptides[i].Charge; 
                IPI.Value = Peptides[i].IPI;
                ipis.Value = IpisStr;
                Sequence.Value = Peptides[i].Sequence;
                PeptidesNum.Value = Peptides[i].peptides;
                ModMass.Value = Peptides[i].ModMass; 
                ModDesc.Value = Peptides[i].ModDesk??""; 
                Case.Value = Peptides[i].Case;
                Insert.ExecuteNonQuery();
            }
            tran.Commit();
        }

        public void AddFile(int FileNumber, string FileName){

            int MaxNumber = 0;
            for (int i = 0 ; i<FileNumbers.Count ; i++ ){
                if (MaxNumber <= FileNumbers[i]) MaxNumber = FileNumbers[i]+1;
            }
            FileNumbers.Add(MaxNumber);

            tran = con.BeginTransaction();
            SQLiteCommand Insert = new SQLiteCommand(String.Format(
                "INSERT INTO RawFiles (FileNumber, FileName)"+
                "VALUES ({0}, \"{1}\" )",MaxNumber,FileName),con);
            Insert.ExecuteNonQuery();
            tran.Commit();
        }

        public void SaveQMatches(List<QPeptide> Peptides, int FileNum){ //с точки зрения дизайна эта таблица не является необходимой
            tran = con.BeginTransaction();
            SQLiteCommand Insert = new SQLiteCommand(
                "INSERT INTO QMatches( FileNumber, MascotScan, ApexRT, ApexScore, ApexMZ, ApexIndex, Score, IsotopeRatio, RTDisp)"+    
                "VALUES (@FileNumber, @MascotScan, @ApexRT, @ApexScore, @ApexMZ, @ApexIndex, @Score, @IsotopeRatio, @RTDisp)",con);

            SQLiteParameter FileNumber = new SQLiteParameter("@FileNumber");
            Insert.Parameters.Add(FileNumber);
            int dbFileNumber;
            dbFileNumber = FileNumbers[FileNum];
            FileNumber.Value = dbFileNumber;
            SQLiteParameter MascotScan = new SQLiteParameter("@MascotScan");
            Insert.Parameters.Add(MascotScan);
            SQLiteParameter ApexRT = new SQLiteParameter("@ApexRT");
            Insert.Parameters.Add(ApexRT);
            SQLiteParameter ApexScore = new SQLiteParameter("@ApexScore");
            Insert.Parameters.Add(ApexScore);
            SQLiteParameter ApexMZ = new SQLiteParameter("@ApexMZ");
            Insert.Parameters.Add(ApexMZ);
            SQLiteParameter ApexIndex = new SQLiteParameter("@ApexIndex");
            Insert.Parameters.Add(ApexIndex);
            SQLiteParameter Score = new SQLiteParameter("@Score");
            Insert.Parameters.Add(Score);
            SQLiteParameter IsotopeRatio = new SQLiteParameter("@IsotopeRatio");
            Insert.Parameters.Add(IsotopeRatio);
            SQLiteParameter RTDisp = new SQLiteParameter("@RTDisp");
            Insert.Parameters.Add(RTDisp);

            for (int i = 0 ; i < Peptides.Count ; i++){
                MascotScan.Value = Peptides[i].MascotScan;
                if (Peptides[i].Matches[FileNum] == null ) continue;
                ApexRT.Value =  Peptides[i].Matches[FileNum].ApexRT;
                ApexScore.Value =  Peptides[i].Matches[FileNum].ApexScore;
                ApexMZ.Value =  Peptides[i].Matches[FileNum].ApexMZ;
                ApexIndex.Value =  Peptides[i].Matches[FileNum].ApexIndex;
                Score.Value =  Peptides[i].Matches[FileNum].Score;
                IsotopeRatio.Value =  Peptides[i].Matches[FileNum].IsotopeRatio;
                RTDisp.Value =  Peptides[i].Matches[FileNum].RTDisp;
                Insert.ExecuteNonQuery();
            }
            tran.Commit();
        }

        public void SaveMSMatches(List<QPeptide> Peptides, int FileNum){
            tran = con.BeginTransaction();
            SQLiteCommand Insert = new SQLiteCommand(
                "INSERT INTO MSMatches (FileNumber, MascotScan, Charge, Score, MZ, RT, FirstIsotope, SecondIsotope, TimeCoeff)"+
                "VALUES (@FileNumber, @MascotScan, @Charge, @Score, @MZ, @RT, @FirstIsotope, @SecondIsotope, @TimeCoeff)",con);

            SQLiteParameter FileNumber = new SQLiteParameter("@FileNumber");
            Insert.Parameters.Add(FileNumber);
            int dbFileNumber = FileNumbers[FileNum];
            FileNumber.Value = dbFileNumber;
            SQLiteParameter MascotScan = new SQLiteParameter("@MascotScan");
            Insert.Parameters.Add(MascotScan);
            SQLiteParameter Charge = new SQLiteParameter("@Charge");
            Insert.Parameters.Add(Charge);
            SQLiteParameter Score = new SQLiteParameter("@Score");
            Insert.Parameters.Add(Score);
            SQLiteParameter MZ = new SQLiteParameter("@MZ");
            Insert.Parameters.Add(MZ);
            SQLiteParameter RT = new SQLiteParameter("@RT");
            Insert.Parameters.Add(RT);
            SQLiteParameter FirstIsotope = new SQLiteParameter("@FirstIsotope");
            Insert.Parameters.Add(FirstIsotope);
            SQLiteParameter SecondIsotope = new SQLiteParameter("@SecondIsotope");
            Insert.Parameters.Add(SecondIsotope);
            SQLiteParameter TimeCoeff = new SQLiteParameter("@TimeCoeff");
            Insert.Parameters.Add(TimeCoeff);

            for (int i = 0 ; i < Peptides.Count ; i++){
                MascotScan.Value = Peptides[i].MascotScan;
                if (Peptides[i].Matches[FileNum] == null ) continue;
                for (int j = 0 ; j < Peptides[i].Matches[FileNum].MSMatches.Count ; j++ ){
                    Charge.Value = Peptides[i].Charge;
                    Score.Value =  Peptides[i].Matches[FileNum].MSMatches[j].Score;
                    MZ.Value =  Peptides[i].Matches[FileNum].MSMatches[j].MZ;
                    RT.Value =  Peptides[i].Matches[FileNum].MSMatches[j].RT;
                    FirstIsotope.Value =  Peptides[i].Matches[FileNum].MSMatches[j].FirstIsotope;
                    SecondIsotope.Value =  Peptides[i].Matches[FileNum].MSMatches[j].SecondIsotope;
                    TimeCoeff.Value =  Peptides[i].Matches[FileNum].MSMatches[j].TimeCoeff;
                    Insert.ExecuteNonQuery();
                    if (Peptides[i].Matches[FileNum].MSMatches[j].LowerCharges != null) {
                        Charge.Value = Peptides[i].Charge-1;
                        Score.Value =  Peptides[i].Matches[FileNum].MSMatches[j].LowerCharges.Score;
                        MZ.Value =  Peptides[i].Matches[FileNum].MSMatches[j].LowerCharges.MZ;
                        RT.Value =  Peptides[i].Matches[FileNum].MSMatches[j].LowerCharges.RT;
                        FirstIsotope.Value =  Peptides[i].Matches[FileNum].MSMatches[j].LowerCharges.FirstIsotope;
                        SecondIsotope.Value =  Peptides[i].Matches[FileNum].MSMatches[j].LowerCharges.SecondIsotope;
                        TimeCoeff.Value =  Peptides[i].Matches[FileNum].MSMatches[j].LowerCharges.TimeCoeff;
                        Insert.ExecuteNonQuery();
                    }
                    if (Peptides[i].Matches[FileNum].MSMatches[j].UpperCharges!= null) {
                        Charge.Value = Peptides[i].Charge+1;
                        Score.Value =  Peptides[i].Matches[FileNum].MSMatches[j].UpperCharges.Score;
                        MZ.Value =  Peptides[i].Matches[FileNum].MSMatches[j].UpperCharges.MZ;
                        RT.Value =  Peptides[i].Matches[FileNum].MSMatches[j].UpperCharges.RT;
                        FirstIsotope.Value =  Peptides[i].Matches[FileNum].MSMatches[j].UpperCharges.FirstIsotope;
                        SecondIsotope.Value =  Peptides[i].Matches[FileNum].MSMatches[j].UpperCharges.SecondIsotope;
                        TimeCoeff.Value =  Peptides[i].Matches[FileNum].MSMatches[j].UpperCharges.TimeCoeff;
                        Insert.ExecuteNonQuery();
                    }
                }
            }
            tran.Commit();
        }

        public void SaveSettings(List<String> Names, List<String> Values){
            tran = con.BeginTransaction();
            SQLiteCommand Insert = new SQLiteCommand(
                "INSERT INTO Settings (Name, Value) "+
                "Values ( @Name, @Value)",con);

            SQLiteParameter Name = new SQLiteParameter("@Name");
            Insert.Parameters.Add(Name);
            SQLiteParameter Value = new SQLiteParameter("@Value");
            Insert.Parameters.Add(Value);

            for (int i = 0 ; i < Names.Count ; i++){
                Name.Value = Names[i];
                Value.Value = Values[i];
                Insert.ExecuteNonQuery();
            }
            tran.Commit();
        }

        public void LoadSettings(ref List<String> Names, ref List<String> Values){
            Names = new List<string>();
            Values = new List<string>();

            SQLiteCommand Select = new SQLiteCommand(
                "Select Name,Value From Settings",con);

            SQLiteDataReader Reader = Select.ExecuteReader();

            while(Reader.Read()){
                Names.Add(Reader[0].ToString());
                Values.Add(Reader[1].ToString());
            }

        }

        public void LoadFileList(ref List<String> FileNames){
            FileNames = new List<string>();

            SQLiteCommand Select = new SQLiteCommand(
                "Select FileNumber, FileName From RawFiles Order by FileNumber",con);

            SQLiteDataReader Reader = Select.ExecuteReader();

            while(Reader.Read()){
                FileNames.Add(Reader[1].ToString());
                FileNumbers.Add(Reader.GetInt32(0));
            }

        }

        public void LoadProteins(List<QProtein> Proteins){

            SQLiteCommand Select = new SQLiteCommand(
                "Select IPI, Desc, ipis, Name From Proteins Order by 1",con);

            SQLiteDataReader Reader = Select.ExecuteReader();

            while(Reader.Read()){
                QProtein Prot = new QProtein();
                Prot.ipi = Reader[0].ToString();
                Prot.Desc = Reader[1].ToString();
                //Prot.ipis = Reader[2].ToString();
                Prot.Name = Reader[3].ToString();
                Prot.Peptides = new List<QPeptide>();
                Proteins.Add(Prot);
            }
        }


        public void LoadMascots(List<QProtein> Proteins, List<QPeptide> Mascots) {
            iLog.Log("Loading Mascot Peptides");
            iLog.RepProgress(0);
            SQLiteCommand Select = new SQLiteCommand(
                "Select MascotScan, MascotMZ, MascotScore, MascotRT, TheorIsoRatio, Charge, "+
                "IPI, ipis, Sequence, Peptides, ModDesc, ModMass, [Case] From Mascots "+
                "Order by IPI",con);

            SQLiteDataReader Reader = Select.ExecuteReader();

            Reader.Read();
            for (int i = 0 ; i < Proteins.Count ; i++){
                while(Reader["IPI"].ToString() == Proteins[i].ipi){
                    QPeptide Pept = new QPeptide();
                    Pept.MascotScan = Reader.GetInt32(0);
                    Pept.MascotMZ = Reader.GetDouble(1);
                    Pept.MascotScore = Reader.GetDouble(2);
                    Pept.MascotRT = Reader.GetDouble(3);
                    Pept.TheorIsotopeRatio = Reader.GetDouble(4);
                    Pept.Charge = Reader.GetInt32(5);
                    Pept.IPI = Reader.GetString(6);
                    //Pept.IPIs = Reader.GetString(7);
                    Pept.Sequence = Reader.GetString(8);
                    Pept.peptides = Reader.GetInt32(9);
                    Pept.ModDesk = Reader.GetString(10);
                    Pept.ModMass = Reader.GetDouble(11);
                    Pept.Case = Reader.GetString(12);
                    Mascots.Add(Pept);
                    Proteins[i].Peptides.Add(Pept);
                    if (!Reader.Read()) break;
                }
                iLog.RepProgress(i*100/Proteins.Count);
            }
        }

        public void LoadAllMSMS(List<QPeptide> AllMSMS) {

            iLog.Log("Loading All MS/MS Entries");
            iLog.RepProgress(0);

            SQLiteCommand Select = new SQLiteCommand(
                "Select MascotScan, MascotMZ, MascotScore, MascotRT, TheorIsoRatio, Charge, "+
                "IPI, ipis, Sequence, Peptides, ModDesc, ModMass, [Case] From AllMSMS "+
                "Order by IPI",con);

            SQLiteDataReader Reader = Select.ExecuteReader();
            int Count = 0;

            while(Reader.Read()){
                QPeptide Pept = new QPeptide();
                Pept.MascotScan = Reader.GetInt32(0);
                Pept.MascotMZ = Reader.GetDouble(1);
                Pept.MascotScore = Reader.GetDouble(2);
                Pept.MascotRT = Reader.GetDouble(3);
                Pept.TheorIsotopeRatio = Reader.GetDouble(4);
                Pept.Charge = Reader.GetInt32(5);
                Pept.IPI = Reader.GetString(6);
                //Pept.IPIs = Reader.GetString(7);
                Pept.Sequence = Reader.GetString(8);
                Pept.peptides = Reader.GetInt32(9);
                Pept.ModDesk = Reader.GetString(10);
                Pept.ModMass = Reader.GetDouble(11);
                Pept.Case = Reader.GetString(12);
                AllMSMS.Add(Pept);
                Count++;
                if (Count%100 == 0){
                    iLog.RepProgress(Count*100/Reader.RecordsAffected);
                }
            }

        }


        public void LoadQMatches(List<QPeptide> Peptides) {
            //может быть как  Mascots так и AllMSMS 
            //соответствеено в Matches могут быть записи которые не подходят для Peptides

            iLog.Log("Loading Peptide Quantification");
            iLog.RepProgress(0);
            Peptides.Sort(new QPeptide.byMascotSN());

            for ( int f = 0 ; f < Peptides[0].Matches.GetLength(0) ; f++){

                int FileNumber;
                FileNumber = FileNumbers[f];

                SQLiteCommand Select = new SQLiteCommand(
                    "Select FileNumber, MascotScan, ApexRT, ApexScore, ApexMZ, ApexIndex, Score, IsotopeRatio, RTDisp "+
                    String.Format("From QMatches Where FileNumber = {0} ",FileNumber)+
                    "Order by MascotScan",con);

                SQLiteDataReader Reader = Select.ExecuteReader();
                bool flag = true;

                Reader.Read();
                iLog.RepProgress(f*100/Peptides[0].Matches.GetLength(0));

                for ( int i = 0 ; i < Peptides.Count ; i++){
                    while(Reader.GetInt32(1)<Peptides[i].MascotScan) {
                        flag = Reader.Read();
                        if (!flag) break;
                    }
                    if (!flag) break;
                    while(Reader.GetInt32(1)==Peptides[i].MascotScan){
                        Peptides[i].Matches[f] = new QMatch();
                        Peptides[i].Matches[f].ApexRT = Reader.GetDouble(2);
                        Peptides[i].Matches[f].ApexScore = Reader.GetDouble(3);
                        Peptides[i].Matches[f].ApexMZ = Reader.GetDouble(4);
                        Peptides[i].Matches[f].ApexIndex = Reader.GetInt32(5);
                        Peptides[i].Matches[f].Score = Reader.GetDouble(6);
                        Peptides[i].Matches[f].IsotopeRatio = Reader.GetDouble(7);
                        Peptides[i].Matches[f].RTDisp = Reader.GetDouble(8);
                        Peptides[i].Matches[f].MSMatches = new List<MSMatch>();
                        flag = Reader.Read();
                        if (!flag) break;
                    }
                    if (!flag) break;
                }
            }
        }

        public void LoadMSMatches(List<QPeptide> Peptides) {

            iLog.Log("Loading Peak Shapes");
            iLog.RepProgress(0);

            for ( int f = 0 ; f < Peptides[0].Matches.GetLength(0) ; f++){

                int FileNumber = FileNumbers[f];

                SQLiteCommand Select = new SQLiteCommand(
                    "Select FileNumber, MascotScan, Charge, Score, MZ, RT, FirstIsotope, SecondIsotope, TimeCoeff "+
                    String.Format("From MSMatches Where FileNumber = {0} ",FileNumber)+
                    "Order by MascotScan,RT ",con);

                SQLiteDataReader Reader = Select.ExecuteReader();
                MSMatch[] Charges = new MSMatch[12];
                bool flag = true;

                Reader.Read();
                iLog.RepProgress(f*100/Peptides[0].Matches.GetLength(0));

                for ( int i = 0 ; i < Peptides.Count ; i++){
                    while(Reader.GetInt32(1)<Peptides[i].MascotScan) {
                        flag = Reader.Read();
                        if (!flag) break;
                    }
                    if (!flag) break;
                    while(Reader.GetInt32(1)==Peptides[i].MascotScan){

                        for ( int j = 0 ; j<12 ; j++) Charges[j] = null;

                        FileNumber = Reader.GetInt32(0);
                        double RT = Reader.GetDouble(5);

                        while(Reader.GetInt32(0) == FileNumber &&
                              Reader.GetInt32(1) == Peptides[i].MascotScan &&
                              RT == Reader.GetDouble(5) ){

                            int Charge= Reader.GetInt32(2);
                            Charges[Charge] = new MSMatch();
                            Charges[Charge].Score = Reader.GetDouble(3);
                            Charges[Charge].MZ    = Reader.GetDouble(4);
                            Charges[Charge].RT    = Reader.GetDouble(5);
                            Charges[Charge].FirstIsotope  = Reader.GetDouble(6);
                            Charges[Charge].SecondIsotope = Reader.GetDouble(7);
                            Charges[Charge].TimeCoeff     = Reader.GetDouble(8);
                            flag = Reader.Read();
                            if (!flag) break;
                        }
                        if (Charges[Peptides[i].Charge-1] != null )
                            Charges[Peptides[i].Charge].LowerCharges = Charges[Peptides[i].Charge-1];
                        if (Charges[Peptides[i].Charge+1] != null )
                            Charges[Peptides[i].Charge].LowerCharges = Charges[Peptides[i].Charge+1];
                        Peptides[i].Matches[f].MSMatches.Add(Charges[Peptides[i].Charge]);
                        if (!flag) break;
                    }
                    if (!flag) break;
                }
            }
        }

        public void DeleteFile(int FileNumber){
            int dbFileNumber = FileNumbers[FileNumber];
            tran = con.BeginTransaction();
            SQLiteCommand com = new SQLiteCommand("DELETE FROM RawFiles "+
                "   WHERE FileNumber = "+dbFileNumber.ToString(),con);
            com.ExecuteNonQuery();
            com = new SQLiteCommand("DELETE FROM QMatches "+
                "   WHERE FileNumber = "+dbFileNumber.ToString(),con);
            com.ExecuteNonQuery();
            com = new SQLiteCommand("DELETE FROM MSMatches "+
                "   WHERE FileNumber = "+dbFileNumber.ToString(),con);
            com.ExecuteNonQuery();
            tran.Commit();
        }
    }
}
