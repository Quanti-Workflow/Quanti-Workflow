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
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Data.SQLite;

namespace FractionMerger
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e){
            if(openDB3Dialog.ShowDialog() != DialogResult.OK)  {
                return;
            }
            for (int i = 0 ; i < openDB3Dialog.FileNames.Length ; i++){
                if (DB3Box.Items.IndexOf(openDB3Dialog.FileNames[i]) == -1){
                    DB3Box.Items.Add(openDB3Dialog.FileNames[i]);
                }
            }
            progressBar1.Value = 0;
            OutDB3Box.Text = Path.GetDirectoryName(openDB3Dialog.FileName)+Path.DirectorySeparatorChar+"Merged.db3";
        }

        private void button2_Click(object sender, EventArgs e){
            ListBox.SelectedObjectCollection ToDelete = DB3Box.SelectedItems;
            for ( int i = 0 ; i < ToDelete.Count ; i++){
                DB3Box.Items.Remove( ToDelete[i]);
            }
            DB3Box.Items.Remove(DB3Box.SelectedItem);
            progressBar1.Value = 0;
        }

        private void button3_Click(object sender, EventArgs e){
            if(saveDB3Dialog.ShowDialog() != DialogResult.OK)  {
                return;
            }
            OutDB3Box.Text = saveDB3Dialog.FileName;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (DB3Box.Items.Count == 0 ) return;
            SQLiteConnection DestCon = null;
            SQLiteTransaction tran = null;

                if (File.Exists(OutDB3Box.Text)){
                    DialogResult Res = MessageBox.Show("File "+OutDB3Box.Text+" already exists. \n Would you like overwrite it?","File exsists...",MessageBoxButtons.OKCancel);
                    if ( Res == DialogResult.OK){
                        File.Delete(OutDB3Box.Text);
                    }else{
                        return; 
                    }
                }

            try{
                //скопировать файл первой базы данных
                File.Copy(DB3Box.Items[0].ToString(), OutDB3Box.Text);

                //изменить таблицы 
                DestCon= new SQLiteConnection(String.Format("Data Source ={0}", OutDB3Box.Text));

                DestCon.Open();
                tran = DestCon.BeginTransaction();
                SQLiteCommand com = new SQLiteCommand(
                    "ALTER TABLE RawFiles ADD COLUMN BandID INTEGER;",DestCon);
                com.ExecuteNonQuery();

                com = new SQLiteCommand(
                    "UPDATE RawFiles Set BandID = 0;",DestCon);
                com.ExecuteNonQuery();

                com = new SQLiteCommand(
                    "ALTER TABLE RawFiles ADD COLUMN SampleID INTEGER;",DestCon);
                com.ExecuteNonQuery();

                com = new SQLiteCommand(
                    "UPDATE RawFiles Set SampleID = FileNumber;",DestCon);
                com.ExecuteNonQuery();

                com = new SQLiteCommand(
                    "UPDATE Settings Set Value = \"Merged\" Where Name=\"DatFileName\" ;",DestCon);
                com.ExecuteNonQuery();

                com = new SQLiteCommand(
                    "ALTER TABLE Mascots ADD COLUMN BandID INTEGER;",DestCon);
                com.ExecuteNonQuery();

                com = new SQLiteCommand(
                    "UPDATE Mascots Set BandID = 0;",DestCon);
                com.ExecuteNonQuery();

                com = new SQLiteCommand(
                    "UPDATE Mascots Set [Case] = NULL;",DestCon);
                com.ExecuteNonQuery();

                com = new SQLiteCommand(
                    "UPDATE Mascots Set ModDesc = \"Band 0; \" || ModDesc",DestCon);
                com.ExecuteNonQuery();

                //запомним сколько было самплов 
                com = new SQLiteCommand(
                    "SELECT count(FileNumber) FROM RawFiles;",DestCon);
                SQLiteDataReader Reader = com.ExecuteReader();
                Reader.Read();
                int SampleNumber = Reader.GetInt32(0);

                tran.Commit();

                DestCon.Close();


                //в цикле добавлять данные из остальных баз данных

                for (int i = 1 ; i < DB3Box.Items.Count ; i++ ){
                    progressBar1.Value = (int)(((double)i/(double)DB3Box.Items.Count)*100.0);
                    Application.DoEvents();
                    DestCon= new SQLiteConnection(String.Format("Data Source ={0}", OutDB3Box.Text));

                    DestCon.Open();

                    com = new SQLiteCommand(
                        "ATTACH \""+DB3Box.Items[i].ToString()+"\" AS Src;",DestCon);

                    com.ExecuteNonQuery();

                    tran = DestCon.BeginTransaction();

                    //выясняем максимальный номер файла 
                    com = new SQLiteCommand(
                        "SELECT max(FileNumber)+1,count(FileNumber) FROM Src.RawFiles;",DestCon);

                    Reader = com.ExecuteReader();
                    Reader.Read();
                    int FileNumber = Reader.GetInt32(0);
                    if (Reader.GetInt32(0) != SampleNumber && Reader.GetInt32(1) != SampleNumber){
                        throw new Exception(String.Format("Number of files in {0} doesn't correspont to number of samples = {1}",DB3Box.Items[i].ToString(),SampleNumber));
                    }

                    //Втаскиваем данные 
                    com = new SQLiteCommand(
                        String.Format(  "INSERT INTO RawFiles (FileNumber, FileName, BandID, SampleID) "+
                                        "SELECT FileNumber+{0}, FileName, {1}, FileNumber FROM Src.RawFiles;",i*SampleNumber,i),DestCon);
                    com.ExecuteNonQuery();

                    com = new SQLiteCommand(
                        String.Format(  "INSERT INTO Mascots ( MascotScan, MascotMZ,  MascotScore, MascotRT, TheorIsoRatio, Charge, IPI, ipis, Sequence, Peptides, ModMass, ModDesc, BandID) "+
                                        "SELECT MascotScan, MascotMZ,  MascotScore, MascotRT, TheorIsoRatio, Charge, IPI, ipis, Sequence, Peptides, ModMass, \"Band {0}; \"||ModDesc, {0} FROM Src.Mascots;",i),DestCon);
                    com.ExecuteNonQuery();

                    com = new SQLiteCommand(
                        String.Format(  "INSERT INTO QMatches ( FileNumber,  MascotScan, ApexRT, ApexScore, ApexMZ, ApexIndex, Score, IsotopeRatio, RTDisp) "+ 
                                        "SELECT FileNumber+{0},  MascotScan, ApexRT, ApexScore, ApexMZ, ApexIndex, Score, IsotopeRatio, RTDisp FROM Src.QMatches;",i*SampleNumber),DestCon);
                    com.ExecuteNonQuery();

                    com = new SQLiteCommand(
                        String.Format(  "INSERT INTO MSMatches ( FileNumber, MascotScan, Charge, Score, MZ, RT, FirstIsotope, SecondIsotope, TimeCoeff) "+
                                        "SELECT FileNumber+{0},  MascotScan, Charge, Score, MZ, RT, FirstIsotope, SecondIsotope, TimeCoeff FROM Src.MSMatches;",i*SampleNumber),DestCon);
                    com.ExecuteNonQuery();

                    //слияние списков белков - сложнее и оказывает влияние на mascots 
                    com = new SQLiteCommand("SELECT IPI, ipis, Name, Desc FROM Src.Proteins;",DestCon);
                    Reader = com.ExecuteReader();

                    while(Reader.Read()){
                        //проверяем наличие белка в базе данных 
                        string IPI = Reader.GetString(0);
                        com = new SQLiteCommand(String.Format("SELECT IPI FROM Proteins WHERE IPI=\"{0}\" OR ipis LIKE \"%{0}%\" ;",IPI),DestCon);
                        SQLiteDataReader IPIReader = com.ExecuteReader();
                        if (IPIReader.Read()){
                            //белок был добавлен к базе данных раньше 
                            //меняем новодобавленные mascots так чтобы указать на старый белок 
                            String NewIPI = IPIReader.GetString(0);
                            IPIReader.Close();
                            if (NewIPI != IPI){
                                com = new SQLiteCommand(String.Format("UPDATE Mascots Set IPI = \"{0}\" where IPI = \"{1}\" and BandID = {2}",NewIPI,IPI,i),DestCon);
                                com.ExecuteNonQuery();
                            }
                        }else{
                            //белок в базе данных отсутствовал 
                            //добавляем белок 
                            com = new SQLiteCommand(String.Format("INSERT INTO Proteins (IPI, ipis, Name, Desc) Select IPI, ipis, Name, Desc From Src.Proteins WHERE IPI = \"{0}\" ",IPI),DestCon);
                            com.ExecuteNonQuery();
                        }
                    }
                    Reader.Close();
                    tran.Commit();
                    DestCon.Close();
                    progressBar1.Value = 100;

                    //com = new SQLiteCommand(
                    //    "DETACH Src;",DestCon);
                    //com.ExecuteNonQuery();
                }
            }catch(Exception ex){
                if (tran != null && tran.Connection != null) tran.Rollback();
                if (DestCon!= null && DestCon.State == ConnectionState.Open ){
                    DestCon.Close();
                }
                File.Delete(OutDB3Box.Text);
                MessageBox.Show(ex.Message,ex.Source);
            }
        }
    }
}
