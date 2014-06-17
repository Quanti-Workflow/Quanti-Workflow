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
using System.Windows.Forms;
using Quanty.Properties;
using Mascot;

namespace Quanty {
    public class MascotsbyFSN: IComparer<Mascot.MascotSpectra> {
        public int Compare(Mascot.MascotSpectra x, Mascot.MascotSpectra y){
            if (x.ScanNumber>y.ScanNumber) { return 1;} 
            else if (x.ScanNumber == y.ScanNumber ) { return 0; }
            else return -1;
        }
    }

    class QMSMSList{
        class QMSMSEntry{
            public bool Approved;
            public string Comment;
            public MascotSpectra ms;
            public QMatch Match;
            public int FSN;
        }

        List<QMSMSEntry> Entries;

        QMSMSEntrybyFSN byFSN;

        public QMSMSList(MascotParser mp){
            Entries = new List<QMSMSEntry>();
            QMSMSEntry Entry;
            for ( int i=0 ; i < mp.Spectra.Count ; i++){
                Entry = new QMSMSEntry();
                Entry.Approved = true;
                Entry.ms = mp.Spectra[i];
                Entry.Match = null;
                Entry.FSN = mp.Spectra[i].ScanNumber;
                Entries.Add(Entry);
            }
            byFSN = new QMSMSEntrybyFSN();
            Entries.Sort(byFSN);
        }

        class QMSMSEntrybyFSN : IComparer<QMSMSEntry> {
            public int Compare(QMSMSEntry x, QMSMSEntry y){
                if (x.FSN>y.FSN) { return 1;} 
                else if (x.FSN == y.FSN ) { return 0; }
                else return -1;
            }
        }
        
        public void SetReason(int FSN, string Comment, bool App){
            QMSMSEntry SearchEntry = new QMSMSEntry();
            SearchEntry.FSN = FSN;
            int index = Entries.BinarySearch(SearchEntry,byFSN);
            Entries[index].Comment = Comment;
            Entries[index].Approved = App;
        }

        public void Out(StreamWriter sw){
            for ( int i = 0 ; i < Entries.Count ; i++){
                sw.WriteLine("{0}\t{1}\t{2}",Entries[i].ms.ScanNumber,Entries[i].ms.mz,Entries[i].Comment);
            }
        }
    }

    public class SpectrabySeq: IComparer<Mascot.MascotSpectra> {
        public int Compare(Mascot.MascotSpectra x, Mascot.MascotSpectra y){
            if (x.Peptides.Count == 0 && y.Peptides.Count == 0 ) {return 0;}
            if (x.Peptides.Count == 0 ) {return -1;}
            if (y.Peptides.Count == 0 ) {return 1;}
            int cp=x.Peptides[0].Sequence.CompareTo(y.Peptides[0].Sequence);
            if (cp!=0) return cp;
            for ( int k = 0 ; k < x.Peptides[0].ModIndex.GetLength(0) ; k++){
                if (x.Peptides[0].ModIndex[k]>y.Peptides[0].ModIndex[k]) {return 1;}
                if (x.Peptides[0].ModIndex[k]<y.Peptides[0].ModIndex[k]) {return -1;}
            }
            if (x.Peptides[0].NModIndex>y.Peptides[0].NModIndex) {return 1;}
            if (x.Peptides[0].NModIndex<y.Peptides[0].NModIndex) {return -1;}
            if (x.Peptides[0].CModIndex>y.Peptides[0].CModIndex) {return 1;}
            if (x.Peptides[0].CModIndex<y.Peptides[0].CModIndex) {return -1;}
            return 0;
        }
    }
    class MascotProc{

        public static ILogAndProgress iLog = null;

        //набор служебных операций над списками строк вместо штатных 
        //string сравнивается как value type, но имеет дефолтную реализацию интерфейса IEquatable как reference type 
        static bool StringContains(ref List<string> L, string str){
            for (int i = 0 ; i < L.Count ; i++ ){
                if (L[i] == str) {
                    return true; 
                }
            }
            return false;
        }

        static void StringRemove(ref List<string> L, string str){
            for (int i = 0 ; i < L.Count ; i++ ){
                if (L[i] == str) {
                    L.Remove(L[i]);
                    return; 
                }
            }
        }

        static public void FillMSMSLists(
            ref List<QPeptide> Mascots, 
            ref List<QPeptide> MSMSList, 
            ref List<QProtein> Proteins,
            MascotParser mp ){

            Mascots = new List<QPeptide>();
            MSMSList = new List<QPeptide>();
            QPeptide data;
            int Best,i,j,k;
            SpectrabySeq comp = new SpectrabySeq();
            mp.Spectra.Sort(comp);

            for (i = 0 ; i < mp.Spectra.Count ; i++){
                //если нет пептидов 
                if (mp.Spectra[i].Peptides.Count == 0) {
                    data = new QPeptide(mp,i);
                    data.Case = "No peptides identified";
                    MSMSList.Add(data);
                    continue;
                }
                //если у пептидов слишком маленький Score
                if (mp.Spectra[i].Peptides[0].Score < Convert.ToDouble(Settings.Default.MascotScore)){
                    data = new QPeptide(mp,i);
                    data.Case = "Peptides identified under score limit";
                    MSMSList.Add(data);
                    continue;
                }

                Best = i;
                if (Settings.Default.MascotScoreFiltering){
                    for (j = i+1; j< mp.Spectra.Count ; j++){
                        if (mp.Spectra[i].Peptides[0].Sequence != mp.Spectra[j].Peptides[0].Sequence){
                            break;
                        }
                        //определяем идентичность набора модификаций
                        for(k = 0 ; k< mp.Spectra[i].Peptides[0].ModIndex.GetLength(0) ; k++){
                            if (mp.Spectra[i].Peptides[0].ModIndex[k] !=  
                                mp.Spectra[j].Peptides[0].ModIndex[k] ){
                                break;
                            }
                        }
                        if (k<mp.Spectra[i].Peptides[0].ModIndex.GetLength(0) || 
                            mp.Spectra[i].Peptides[0].CModIndex != mp.Spectra[j].Peptides[0].CModIndex || 
                            mp.Spectra[i].Peptides[0].NModIndex != mp.Spectra[j].Peptides[0].NModIndex  ) break;
                        //поиск лучшего по Score
                        if (mp.Spectra[j].Peptides[0].Score > mp.Spectra[Best].Peptides[0].Score){
                            Best = j;
                        }
                    }
                    //отмечаем нелучшие в списке всех MS/MS
                    for (k = i ; k < j ; k++ ){
                        if (k != Best){
                            data = new QPeptide(mp,k);
                            data.Case = string.Format("Not the best score ({0}) for peptide {1} (Max={2})",
                                    mp.Spectra[k].Peptides[0].Score,
                                    mp.Spectra[k].Peptides[0].Sequence,
                                    mp.Spectra[Best].Peptides[0].Score);
                            MSMSList.Add(data);
                        }
                    }
                    i = j-1; 
                }

                //только для лучшего по Score если пептид идентифицирован более одного раза 
                data = new QPeptide(mp,Best);
                Mascots.Add(data);
            }

            //сохраняем списки IPI 
            List<List<string>> IPIS = new List<List<string>>();
            for (i = 0 ; i < Mascots.Count ; i++){
                List<String> lipis = new List<string>();
                lipis.AddRange(Mascots[i].IPIs);
                IPIS.Add(lipis);
            }
            
            //Фильтруем IPI
            //Спектр не может соответствовать нескольким IPI за исключением случая когда 
            //набор спектров идентифицирующих один IPI является подмножеством (возможно полным)
            //спектров идентифицирующих другой IPI

            //собираем набор тех из них которые являются подмножествами других 
            List<string> Processed = new List<string>(); //список в который откладываются 
            List<string> ipiset = new List<string>(); //список всех IPI пептида кроме проверяемого 
            List<string> NotDeps = new List<string>();
            string[] dataexipis = null;
            foreach ( QPeptide dataex in Mascots){ //для каждого пептида 
                dataexipis = new string[dataex.IPIs.Count]; 
                dataex.IPIs.CopyTo(dataexipis);
                foreach (string ipi in dataexipis){
                    if (!StringContains(ref Processed,ipi)){ //если данный ipi еще не находится в списке обработанных
                        ipiset.Clear();                     //составляем множество всех IPI пептида кроме проверяемого 
                        ipiset.AddRange(dataex.IPIs);
                        StringRemove(ref ipiset,ipi);
                        foreach(QPeptide chdata in Mascots){ //составленный список будем соотносить с полным набором пептидов 
                            if (dataex.MascotRT != chdata.MascotRT &&   
                                StringContains(ref chdata.IPIs,ipi)){
                                //в этот момент мы установили что dataex и chdata имеют общий ipi
                                foreach(string ipicheck in ipiset){
                                    //здесь мы отбираем в Nodepts идентификаторы 
                                    //которые не появляются вместе с ipi 
                                    //отбор такого идентификатора в NoDepts означает что существует хотя бы один пептид 
                                    // относящийся к ipi но не относящийся к этому идентификатору 
                                    //и следовательно ipi не является зависимым от такого идентификатора 
                                    if(!StringContains(ref chdata.IPIs,ipicheck) && 
                                       !StringContains(ref NotDeps,ipicheck)){  //если есть хотя бы один пептид с проверяемым IPI 
                                        NotDeps.Add(ipicheck);                  //не несущий какго-то из IPI начального списка
                                                                                //то целевой IPI не является зависимым от отсутствующего
                                    }
                                }
                            }
                        }
                        //если есть хотя бы один идентификатор который всегда встречается вместе с ipi 
                        //то в список Nodepts он никогда не попадет и неполнота списка Nodepts будет признаком 
                        //того что ipi зависит от другого идентификатора и должен быть исключен из списка 
                        //идентификаторов во всех пептидах
                        if (ipiset.Count != NotDeps.Count) { 
                            foreach(QPeptide chdata in Mascots){
                                StringRemove(ref chdata.IPIs,ipi);
                            }
                        }else{
                            Processed.Add(ipi);
                        }
                        NotDeps.Clear();
                    }
                }
            }

            iLog.RepProgress(50);
            //и теперь переносим только непротиворечивые
            List<QPeptide> OldMascots = Mascots;
            Mascots = new List<QPeptide>();

            for (i = 0 ; i < OldMascots.Count ; i++ ){
                if ( OldMascots[i].IPIs.Count == 1){
                    QPeptide remdata = OldMascots[i];
                    remdata.IPI = remdata.IPIs[0];
                    remdata.IPIs = IPIS[i];
                    Mascots.Add(remdata);
                }else{
                    string Mess = string.Format("This entry ambigously defined as part of {0} Proteins: ",OldMascots[i].IPIs.Count);
                    for (j = 0 ; j < OldMascots[i].IPIs.Count ; j++){
                        Mess = Mess + OldMascots[i].IPIs[j]+ ", ";
                    }
                    Mess = Mess.Substring(0,Mess.Length-2);
                    OldMascots[i].Case = Mess;
                    OldMascots[i].IPIs = IPIS[i];
                    MSMSList.Add(OldMascots[i]);
                }
            }
            iLog.RepProgress(60);
            //теперь сортируем по белкам 
            Mascots.Sort(new QPeptide.byIPIs());

            //теперь фильтруем не набравшие достаточного количества пептидов 
            iLog.RepProgress(70);

            QProtein Prot;
            Proteins = new List<QProtein>();

            int Count = 0;
            string preIPI = "";
            for (i = 0 ; i <= Mascots.Count ; i++){
                if (i < Mascots.Count && Mascots[i].IPI == preIPI){
                    Count++;
                }else{
                    for (j = i-1 ; i-j<=Count ; j--){
                        QPeptide temp = Mascots[j];
                        temp.peptides = Count;
                        Mascots[j] = temp;
                    }
                    if ( Count >= Convert.ToInt32(Settings.Default.PeptperProt)){
                        Prot = new QProtein();
                        Prot.Peptides = new List<QPeptide>();
                        //объединяем списки ipi

                        //Prot.ipis = Mascots[i-1].IPIs;

                        Prot.ipi = Mascots[i-1].IPI;
                        Prot.Est = 0.0;
                        Prot.ipis = new List<string>();
                        for (j = i-1 ; i-j<=Count ; j--){
                            Prot.Peptides.Add(Mascots[j]);
                            Mascots[j].PartOfProtein = Prot;
                            for ( k = 0 ; k < Mascots[j].IPIs.Count ; k++){
                                if (!StringContains(ref Prot.ipis,Mascots[j].IPIs[k])){
                                    Prot.ipis.Add(Mascots[j].IPIs[k]);
                                }
                            }
                        }
                        //добываем из файла Маскот описания и имена белка 

                        for ( j = 0 ; j < mp.Proteins.Count ; j++){
                            if (mp.Proteins[j].Name == "\""+Mascots[i-1].IPI+"\""){
                                break;
                            }
                        }
                        if (j < mp.Proteins.Count){
                            Prot.Name = mp.Proteins[j].Name;
                            Prot.Desc = mp.Proteins[j].Descr;
                        }else{
                            Prot.Name = Prot.ipi;
                            Prot.Desc = "This protein is not described in source .dat file. See initial FASTA file for protein description";
                        }
                        Proteins.Add(Prot);
                    }else{
                        for (j = i-1 ; i-j<=Count ; j--){
                            Mascots[j].Case = string.Format("This entry belongs to \"{0}\" protein which have only {1} uniquely identified peptide(s)",Mascots[j].IPI,Count);
                            MSMSList.Add(Mascots[j]);
                        }
                    }
                    Count = 1;
                    if (i < Mascots.Count){
                        preIPI = Mascots[i].IPI;
                    }
                }
            }

            iLog.RepProgress(80);

            OldMascots = Mascots;
            Mascots = new List<QPeptide>();

            for (i = 0 ; i < OldMascots.Count ; i++ ){
                if ( OldMascots[i].peptides >= Convert.ToInt32(Settings.Default.PeptperProt)){
                    QPeptide remdata = OldMascots[i];
                    remdata.Case = string.Format("This entry belongs to \"{0}\" quntified protein",remdata.IPI);
                    Mascots.Add(remdata);
                    MSMSList.Add(remdata);
                }else{
                    continue;
                }
            }
            //пересчитать белки - без REVERSED
            int ProtCount = 0, PeptCount = Mascots.Count;
            for (i = 0; i < Proteins.Count; i++ ){
                if (Proteins[i].ipi.Contains("REVERSED")){
                    PeptCount -= Proteins[i].Peptides.Count;
                }else{
                    ProtCount++;
                }
            }

                iLog.RepProgress(90);

            //теперь сортируем по белкам и внутри них 
            Mascots.Sort( new QPeptide.byMascotSN());
            //и указываем теоретические отношения первых изотопов 

            iLog.RepProgress(100);

        }

        static public bool FillMSMSLists(
            ref List<QPeptide> Mascots, 
            ref List<QPeptide> MSMSList, 
            ref List<QProtein> Proteins,
            StreamReader sr ){


            int LineCount = 0;
            //пока табов меньше пяти - это строки заголовка - отматываем их
            List<string> Tokens = new List<string>();
            while( !sr.EndOfStream){
                string str = sr.ReadLine();
                LineCount++;
                Tokens = new List<string>(str.Split(new char[] {'\t'}));
                if (Tokens.Count > 3 && !Tokens.Contains("")){
                    break;
                }
            }

            if (sr.EndOfStream){
                MessageBox.Show("Wrong text file format. No data found");
                return false;
            }

            //в верхний регистр и обрезать
            for ( int i = 0 ; i < Tokens.Count ; i++){
                Tokens[i] = Tokens[i].ToUpper().Trim();
            }
            //на выходе - заголовок таблицы 
            int[] Indexes = new int[9];
            //0 - SEQUENCE || ID - ID of feature  
            Indexes[0] = Tokens.IndexOf("SEQUENCE")+Tokens.IndexOf("ID") + 1;
            //1 - MOD DESCR. || DESC - Description of feature (ID & Desc combines composite unique index of feature inside of group) – optional
            Indexes[1] = Tokens.IndexOf("MOD DESCR.")+Tokens.IndexOf("DESC") + 1;
            //2 - IPI || GROUP ID - ID of feature group (have to be unique) – if not defined – all the features will be treated separately
            Indexes[2] = Tokens.IndexOf("IPI")+Tokens.IndexOf("GROUP ID") + 1;
            //3 - IPI DESC || GROUP DESC - ID of feature group (have to be unique) – if not defined – all the features will be treated separately
            Indexes[3] = Tokens.IndexOf("IPI DESC")+Tokens.IndexOf("GROUP DESC") + 1;
            //4 - RT ORDER || ORDER - Average elution order (optional but desired)
            Indexes[4] = Tokens.IndexOf("RT ORDER")+Tokens.IndexOf("ORDER") + 1;
            //5 - RT APEX || RT - Elution time
            Indexes[5] = Tokens.IndexOf("RT APEX")+Tokens.IndexOf("RT") + 1;
            //6 - MZ MASCOT || MZ - Feature m/z
            Indexes[6] = Tokens.IndexOf("MZ MASCOT")+Tokens.IndexOf("MZ") + 1;
            //7 - CHARGE - Charge
            Indexes[7] = Tokens.IndexOf("CHARGE");
            //8 - MASCOT SCORE || Score – optional
            Indexes[8] = Tokens.IndexOf("MASCOT SCORE")+Tokens.IndexOf("SCORE") + 1;

            //Check obligatory fields
            if (Indexes[0]<0 || Indexes[5]<0 || Indexes[6]<0 || Indexes[7]<0 ) {
                MessageBox.Show("File parsing error. Please, check input file for obligatory columns which are SEQUENCE|ID, RT APEX|RT, MZ MASCOT|MZ, CHARGE.");
                return false;
            }


            Mascots = new List<QPeptide>();

            int IPICount = 0; 

            while( !sr.EndOfStream){
                string str = sr.ReadLine();
                LineCount++;
                Tokens = new List<string>(str.Split(new char[] {'\t'}));
                if (Tokens.Count <= 3){
                    continue;
                }
                try{
                    QPeptide P = new QPeptide();
                    P.MascotMZ = Convert.ToDouble(Tokens[Indexes[6]]);
                    P.MascotRT = Convert.ToDouble(Tokens[Indexes[5]]);
                    P.MascotScore = (Indexes[8]<0)?Convert.ToDouble(Settings.Default.MascotScore):Convert.ToDouble(Tokens[Indexes[8]]);
                    if (P.MascotScore < Convert.ToDouble(Settings.Default.MascotScore)) {
                        continue;
                    }
                    P.MascotScan = (Indexes[4]<0)?0:Convert.ToInt32(Tokens[Indexes[4]]);
                    P.Charge = Convert.ToInt32(Tokens[Indexes[7]]);
                    P.IPI = (Indexes[2]<0)?(IPICount++).ToString():Tokens[Indexes[2]];
                    P.IPIs = new List<string>();
                    P.Sequence = Tokens[Indexes[0]];
                    P.ModDesk = (Indexes[1]<0)?"":Tokens[Indexes[1]];
                    //временно помещаем описание белка в Case
                    P.Case = (Indexes[3]<0)?"":Tokens[Indexes[3]];
                    Mascots.Add(P);
                }catch(IndexOutOfRangeException e){
                    MessageBox.Show("File parsing error","Check column consistency.");
                    return false;
                }catch{
                    MessageBox.Show("File parsing error","Check data format from string "+LineCount.ToString()+".");
                    return false;
                }
            }

            Mascots.Sort(new QPeptide.byIPIs());
            QProtein Prot;
            Proteins = new List<QProtein>();

            //Теперь собираем белки 
            string PrevIPI = "";
            int PrevIPIInd = 0;
            for (int i = 0 ; i <= Mascots.Count ; i++){
                if (i==Mascots.Count || (Mascots[i].IPI != PrevIPI && PrevIPI != "") ){
                    if (i-PrevIPIInd >= Convert.ToInt32(Settings.Default.PeptperProt)){
                        Prot = new QProtein();
                        Prot.ipi = PrevIPI;
                        Prot.ipis = new List<string>();
                        Prot.Desc = Mascots[PrevIPIInd].Case;
                        Prot.Peptides = new List<QPeptide>();
                        for ( int j = PrevIPIInd ; j < i ; j++ ){
                            Mascots[j].PartOfProtein = Prot;
                            Mascots[j].Case = "";
                            Prot.Peptides.Add(Mascots[j]);
                        }
                        Proteins.Add(Prot);
                    }
                    PrevIPIInd = i;
                }
                PrevIPI = i < Mascots.Count ? Mascots[i].IPI : "";
            }
            //чистим Mascots от пептидов без белков и зодно ставим MascotOrder если его там не было 
            
            if (Indexes[4] == -1) {
                Mascots.Sort(new QPeptide.byMascotRT());
            }

            for (int i = Mascots.Count-1 ; i >= 0 ; i--){
                if (Indexes[4] == -1) {
                    Mascots[i].MascotScan = i;
                }
                if (Mascots[i].PartOfProtein == null ){
                    Mascots.RemoveAt(i);
                }
            }
            Mascots.Sort(new QPeptide.byIPIs());

            MSMSList = Mascots;

            return true;

        }
    }


}
