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
//using Quanty.Properties;
//using Mascot;

namespace QuantiProcess{
    class DBInterface{

        SQLiteConnection con;
        SQLiteTransaction tran;
        public static ILogAndProgress iLog;

        String FileName;

        public string GetFileName(){
            return FileName;
        }

        public void ConnectTo(string DBFileName){
            FileName = DBFileName;
            con = new SQLiteConnection(String.Format("Data Source = {0}",DBFileName));
            con.Open();
        }

        public void BeginTran(){
            tran = con.BeginTransaction();
        }

        public void Commit(){
            tran.Commit();
        }


        public void AddFile(int FileNumber, string FileName){

            SQLiteCommand Insert = new SQLiteCommand(String.Format(
                "INSERT INTO RawFiles (FileNumber, FileName)"+
                "VALUES ({0}, \"{1}\" )",FileNumber,FileName),con);
            Insert.ExecuteNonQuery();
        }

        public void SaveQMatches(List<QPeptide> Peptides, int FileNum){ //с точки зрения дизайна эта таблица не является необходимой
            SQLiteCommand Insert = new SQLiteCommand(
                "INSERT INTO QMatches( FileNumber, MascotScan, ApexRT, ApexScore, ApexMZ, ApexIndex, Score, IsotopeRatio, RTDisp)"+    
                "VALUES (@FileNumber, @MascotScan, @ApexRT, @ApexScore, @ApexMZ, @ApexIndex, @Score, @IsotopeRatio, @RTDisp)",con);

            SQLiteParameter FileNumber = new SQLiteParameter("@FileNumber");
            Insert.Parameters.Add(FileNumber);
            FileNumber.Value = FileNum;
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
                if (Peptides[i].Match == null ) continue;
                ApexRT.Value =  Peptides[i].Match.ApexRT;
                ApexScore.Value =  Peptides[i].Match.ApexScore;
                ApexMZ.Value =  Peptides[i].Match.ApexMZ;
                ApexIndex.Value =  Peptides[i].Match.ApexIndex;
                Score.Value =  Peptides[i].Match.Score;
                IsotopeRatio.Value =  Peptides[i].Match.IsotopeRatio;
                RTDisp.Value =  Peptides[i].Match.RTDisp;
                Insert.ExecuteNonQuery();
            }
        }

        public void SaveMSMatches(List<QPeptide> Peptides, int FileNum){
            SQLiteCommand Insert = new SQLiteCommand(
                "INSERT INTO MSMatches (FileNumber, MascotScan, Charge, Score, MZ, RT, FirstIsotope, SecondIsotope, TimeCoeff)"+
                "VALUES (@FileNumber, @MascotScan, @Charge, @Score, @MZ, @RT, @FirstIsotope, @SecondIsotope, @TimeCoeff)",con);

            SQLiteParameter FileNumber = new SQLiteParameter("@FileNumber");
            Insert.Parameters.Add(FileNumber);
            FileNumber.Value = FileNum;
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
                if (Peptides[i].Match == null ) continue;
                for (int j = 0 ; j < Peptides[i].Match.MSMatches.Count ; j++ ){
                    Charge.Value = Peptides[i].Charge;
                    Score.Value =  Peptides[i].Match.MSMatches[j].Score;
                    MZ.Value =  Peptides[i].Match.MSMatches[j].MZ;
                    RT.Value =  Peptides[i].Match.MSMatches[j].RT;
                    FirstIsotope.Value =  Peptides[i].Match.MSMatches[j].FirstIsotope;
                    SecondIsotope.Value =  Peptides[i].Match.MSMatches[j].SecondIsotope;
                    TimeCoeff.Value =  Peptides[i].Match.MSMatches[j].TimeCoeff;
                    Insert.ExecuteNonQuery();
                    if (Peptides[i].Match.MSMatches[j].LowerCharges != null) {
                        Charge.Value = Peptides[i].Charge-1;
                        Score.Value =  Peptides[i].Match.MSMatches[j].LowerCharges.Score;
                        MZ.Value =  Peptides[i].Match.MSMatches[j].LowerCharges.MZ;
                        RT.Value =  Peptides[i].Match.MSMatches[j].LowerCharges.RT;
                        FirstIsotope.Value =  Peptides[i].Match.MSMatches[j].LowerCharges.FirstIsotope;
                        SecondIsotope.Value =  Peptides[i].Match.MSMatches[j].LowerCharges.SecondIsotope;
                        TimeCoeff.Value =  Peptides[i].Match.MSMatches[j].LowerCharges.TimeCoeff;
                        Insert.ExecuteNonQuery();
                    }
                    if (Peptides[i].Match.MSMatches[j].UpperCharges!= null) {
                        Charge.Value = Peptides[i].Charge+1;
                        Score.Value =  Peptides[i].Match.MSMatches[j].UpperCharges.Score;
                        MZ.Value =  Peptides[i].Match.MSMatches[j].UpperCharges.MZ;
                        RT.Value =  Peptides[i].Match.MSMatches[j].UpperCharges.RT;
                        FirstIsotope.Value =  Peptides[i].Match.MSMatches[j].UpperCharges.FirstIsotope;
                        SecondIsotope.Value =  Peptides[i].Match.MSMatches[j].UpperCharges.SecondIsotope;
                        TimeCoeff.Value =  Peptides[i].Match.MSMatches[j].UpperCharges.TimeCoeff;
                        Insert.ExecuteNonQuery();
                    }
                }
            }
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


         public void LoadMascots(List<QPeptide> Mascots) {
            iLog.Log("Loading Mascot Peptides");
            iLog.RepProgress(0);
            SQLiteCommand Select = new SQLiteCommand(
                "Select MascotScan, MascotMZ, MascotScore, MascotRT, TheorIsoRatio, Charge, "+
                "IPI, ipis, Sequence, Peptides, ModDesc, ModMass, [Case] From Mascots "+
                "Order by IPI",con);

            SQLiteDataReader Reader = Select.ExecuteReader();

            while (Reader.Read()){
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
                Pept.IPI = Reader.IsDBNull(6) ? "" : Reader.GetString(6);
                //Pept.IPIs = Reader.GetString(7);
                Pept.Sequence = Reader.IsDBNull(8) ? "" : Reader.GetString(8);
                Pept.peptides = Reader.GetInt32(9);
                Pept.ModDesk = Reader.GetString(10);
                Pept.ModMass = Reader.GetDouble(11);
                Pept.Case = Reader.GetString(12);
                AllMSMS.Add(Pept);
                Count++;
                //if (Count%100 == 0){
                //    iLog.RepProgress(Count*100/Reader.RecordsAffected);
                //}
            }
        }

    }
}
