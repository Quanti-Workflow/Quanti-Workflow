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
            Bands = -1;
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
                string IpisStr = Utils.IPIListtoString(Proteins[i].ipis);
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
                MascotScan.Value = Peptides[i].MascotScan;
                MascotMZ.Value = Peptides[i].MascotMZ;
                MascotScore.Value = Peptides[i].MascotScore;
                MascotRT.Value = Peptides[i].MascotRT;
                TheorIsoRatio.Value = Peptides[i].TheorIsotopeRatio;
                Charge.Value = Peptides[i].Charge; 
                IPI.Value = Peptides[i].IPI;
                ipis.Value = Utils.IPIListtoString(Peptides[i].IPIs);
                Sequence.Value = Peptides[i].Sequence;
                PeptidesNum.Value = Peptides[i].peptides;
                ModMass.Value = Peptides[i].ModMass; 
                ModDesc.Value = Peptides[i].ModDesk??""; 
                Case.Value = Peptides[i].Case;
                Insert.ExecuteNonQuery();
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

        int [,] BandList;
        int Bands;
        int SampleNumber;

        public void LoadFileList(ref List<String> FileNames){
            FileNames = new List<string>();
            FileNumbers.Clear();

            //проверяем FileList на наличие фракций
            SQLiteCommand Select = new SQLiteCommand(
                "Select * From RawFiles Order by FileNumber",con);
            SQLiteDataReader Reader = Select.ExecuteReader();
            Bands = Reader.GetOrdinal("BandID");

            if (Bands < 0) {//фракций нет
                Select = new SQLiteCommand(
                    "Select FileNumber, FileName From RawFiles Order by FileNumber",con);

                Reader = Select.ExecuteReader();

                while(Reader.Read()){
                    FileNames.Add(Reader[1].ToString());
                    FileNumbers.Add(Reader.GetInt32(0));
                }
            }else{//фракции есть
                //определяем число фракций и число самплов 
                Select = new SQLiteCommand(
                    "Select max(BandID), max(SampleID) From RawFiles ",con);
                Reader = Select.ExecuteReader();
                Reader.Read();
                Bands = Reader.GetInt32(0)+1;
                SampleNumber = Reader.GetInt32(1)+1;
                BandList = new int[Bands,SampleNumber];

                for (int i = 0 ; i < SampleNumber ; i++){
                    Select = new SQLiteCommand(
                        "Select FileNumber, FileName, BandID, SampleID From RawFiles Where SampleID = "+i.ToString()+" Order by BandID ",con);
                    Reader = Select.ExecuteReader();

                    string SampleName = "{Sample "+i.ToString()+"} - ";
                    while(Reader.Read()){
                        SampleName = SampleName + Reader.GetString(1) + "; ";
                        BandList[Reader.GetInt32(2),i] = Reader.GetInt32(0);
                    }
                    FileNames.Add(SampleName);
                    FileNumbers.Add(i);
                }
                //надо заблокировать кнопки Add и Delete для случая фракций
            }

        }

        public int GetNextDbNumber(){
            int MaxNumber = 0;
            for (int i = 0 ; i<FileNumbers.Count ; i++ ){
                if (MaxNumber <= FileNumbers[i]) MaxNumber = FileNumbers[i]+1;
            }
            FileNumbers.Add(MaxNumber);
            return MaxNumber;
        }

        public void LoadProteins(List<QProtein> Proteins){

            SQLiteCommand Select = new SQLiteCommand(
                "Select IPI, Desc, ipis, Name From Proteins Order by 1",con);

            SQLiteDataReader Reader = Select.ExecuteReader();

            while(Reader.Read()){
                QProtein Prot = new QProtein();
                Prot.ipi = Reader[0].ToString();
                Prot.Desc = Reader[1].ToString();
                Prot.ipis = Utils.IPIStringtoList(Reader[2].ToString());
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
                "IPI, ipis, Sequence, Peptides, ModDesc, ModMass, [Case]"+(Bands<0?", -1":", BandID")+" From Mascots "+
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
                    Pept.IPIs = Utils.IPIStringtoList(Reader.GetString(7)); 
                    Pept.Sequence = Reader.GetString(8);
                    Pept.peptides = Reader.GetInt32(9);
                    Pept.ModDesk = Reader.GetString(10);
                    Pept.ModMass = Reader.GetDouble(11);
                    Pept.Case = Reader.IsDBNull(12)?"":Reader.GetString(12);
                    Pept.BandID = Reader.GetInt32(13);
                    Mascots.Add(Pept);
                    Proteins[i].Peptides.Add(Pept);
                    if (!Reader.Read()) break;
                }
                iLog.RepProgress(i,Proteins.Count);
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
                if (!Reader.IsDBNull(6)){
                    Pept.IPI = Reader.GetString(6);
                }else{
                    Pept.IPI = "";
                }
                Pept.IPIs = Utils.IPIStringtoList(Reader.GetString(7));
                if (!Reader.IsDBNull(8)){
                    Pept.Sequence = Reader.GetString(8);
                }else{
                    Pept.Sequence = "";
                }
                Pept.peptides = Reader.GetInt32(9);
                Pept.ModDesk = Reader.GetString(10);
                Pept.ModMass = Reader.GetDouble(11);
                Pept.Case = Reader.GetString(12);
                AllMSMS.Add(Pept);
                Count++;
                if (Count%100 == 0){
                    //iLog.RepProgress(Count*100/Reader.RecordsAffected);
                }
            }

        }


        public void LoadQMatches(List<QPeptide> Peptides) {
            //может быть как  Mascots так и AllMSMS 
            //соответствеено в Matches могут быть записи которые не подходят для Peptides

            iLog.Log("Loading Peptide Quantification");
            iLog.RepProgress(0);
            Peptides.Sort(new QPeptide.byMascotSN());

            int FileNumber;
            int BandID = -1;
            int SampleID = 0;

            if (Bands < 0 ){
                FileNumber = FileNumbers.Count;
            }else{
                FileNumber = BandList.GetLength(0) * BandList.GetLength(1);
            }


            for ( int f = 0 ; f < FileNumber ; f++){

                int FileID;
                if (Bands < 0 ){
                    FileID = FileNumbers[f];
                    SampleID = f;
                }else{
                    FileID = f;
                    BandID = f / SampleNumber ;
                    SampleID = f % SampleNumber;
                }
    
                SQLiteCommand Select = new SQLiteCommand(
                    "Select FileNumber, MascotScan, ApexRT, ApexScore, ApexMZ, ApexIndex, Score, IsotopeRatio, RTDisp "+
                    String.Format("From QMatches Where FileNumber = {0} ",FileID)+
                    "Order by MascotScan",con);

                SQLiteDataReader Reader = Select.ExecuteReader();
                bool flag = true;

                Reader.Read();
                iLog.RepProgress(f,FileNumber);
                if (!Reader.HasRows) break;

                for ( int i = 0 ; i < Peptides.Count ; i++){
                    while(Peptides[i].BandID != BandID){
                        i++;
                        flag = (i < Peptides.Count);
                        if (!flag) break;
                    }
                    if (!flag) break;
                    while(Reader.GetInt32(1)<Peptides[i].MascotScan) {
                        flag = Reader.Read();
                        if (!flag) break;
                    }
                    if (!flag) break;
                    //такое впечатление что этот  цикл - лишний - то есть проходит всегда 1 раз
                    while(Reader.GetInt32(1)==Peptides[i].MascotScan){
                        Peptides[i].Matches[SampleID] = new QMatch();
                        Peptides[i].Matches[SampleID].ApexRT = Reader.GetDouble(2);
                        Peptides[i].Matches[SampleID].ApexScore = Reader.GetDouble(3);
                        Peptides[i].Matches[SampleID].ApexMZ = Reader.GetDouble(4);
                        Peptides[i].Matches[SampleID].ApexIndex = Reader.GetInt32(5);
                        Peptides[i].Matches[SampleID].Score = Reader.GetDouble(6);
                        Peptides[i].Matches[SampleID].IsotopeRatio = Reader.GetDouble(7);
                        Peptides[i].Matches[SampleID].RTDisp = Reader.GetDouble(8);
                        Peptides[i].Matches[SampleID].MSMatches = new List<MSMatch>();
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
            int FileNumber;
            int BandID = -1;
            int SampleID = 0;

            if (Bands < 0 ){
                FileNumber = FileNumbers.Count;
            }else{
                FileNumber = BandList.GetLength(0) * BandList.GetLength(1);
            }


            for ( int f = 0 ; f < FileNumber ; f++){

                int FileID;
                if (Bands < 0 ){
                    FileID = FileNumbers[f];
                    SampleID = f;
                }else{
                    FileID = f;
                    BandID = f / SampleNumber ;
                    SampleID = f % SampleNumber;
                }

                SQLiteCommand Select = new SQLiteCommand(
                    "Select FileNumber, MascotScan, Charge, Score, MZ, RT, FirstIsotope, SecondIsotope, TimeCoeff "+
                    String.Format("From MSMatches Where FileNumber = {0} ",FileID)+
                    "Order by MascotScan,RT ",con);

                SQLiteDataReader Reader = Select.ExecuteReader();
                MSMatch[] Charges = new MSMatch[12];
                bool flag = true;

                Reader.Read();
                if (!Reader.HasRows) break;
                iLog.RepProgress(f,FileNumber);


                for ( int i = 0 ; i < Peptides.Count ; i++){

                    while(Peptides[i].BandID != BandID){
                        i++;
                        flag = (i < Peptides.Count);
                        if (!flag) break;
                    }
                    if (!flag) break;
                    while(Reader.GetInt32(1)<Peptides[i].MascotScan) {
                        flag = Reader.Read();
                        if (!flag) break;
                    }
                    if (!flag) break;

                    while(Reader.GetInt32(1)==Peptides[i].MascotScan){

                        for ( int j = 0 ; j<12 ; j++) Charges[j] = null;

                        //FileNumber = Reader.GetInt32(0);
                        double RT = Reader.GetDouble(5);

                        while(//Reader.GetInt32(0) == FileNumber &&
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
                            //порпавить в версии 2.6 - должно быть:
                            Charges[Peptides[i].Charge].UpperCharges= Charges[Peptides[i].Charge+1];
                            //Charges[Peptides[i].Charge].LowerCharges = Charges[Peptides[i].Charge+1];
                        Peptides[i].Matches[SampleID].MSMatches.Add(Charges[Peptides[i].Charge]);
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
