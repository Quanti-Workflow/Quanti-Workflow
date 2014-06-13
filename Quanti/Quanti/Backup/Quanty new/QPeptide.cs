using System;
using System.Collections.Generic;
using Mascot;

namespace Quanty {
    public class QPeptide {

        //базовый пептид
        public double MascotMZ; // масса полученная из файла Маскот                             - pers
        public double MascotRT; // RT полученное из файла Маскот                                - pers
        public double MascotScore;  // Mascot Score                                             - pers
        public double TheorIsotopeRatio; // теоретическое отношение первого и второго изотопов  - pers
        public int MascotScan;  //Finegan Scan Number полученный из Маскота -                   - pers
                                //После ClusterMGF соответствует порядку пептида в выборке
        public int RTNumber;    //номер в соотвествии с RT в данном эксперименте                - no pers
        public int Charge;      //зарядовое состояние пептида                                   - pers
        public string IPI;         //итоговый идентификатор белка                               - pers
        public List<string> IPIs;  //исходный набор идентификаторов белка                       - pers
        public string Sequence; //аминокислотная последовательность                             - pers
        public int peptides;    //количество пептидов в белке                                   - pers    
        public double ModMass;  //полная масса модификаций                                      - pers
        public string ModDesk;  //описане модификаций - текстом                                 - pers
        public string Case;     //описание - к какой группе относится данный файл               - pers


        public double MatrixScore;

        public QMatch[] Matches; //набор неизменных сборок из                                   
        public QMatch RefMatch;  //либо ссылка в массив - либо измененная копия     
        public QMatch RefNumberMatch; //копия матча реферируемого файла 

        public double[,] ReferenceMatrix;

        public bool AlreadySearched;

        public static string ReportString;

        public QPeptide(MascotParser mp, int Index){
            MascotSpectra ms = mp.Spectra[Index];
            string buf;
            MascotMZ = ms.mz;
            MascotRT = ms.RT;
            MascotScan = ms.ScanNumber;
            IPIs = new List<string>();
            Charge = ms.Charge;
            if (ms.Peptides.Count > 0){
                Peptide Pep =  ms.Peptides[0];
                for (int j = 0 ; j <  Pep.ProteinNames.Length ; j++){
                    buf = Pep.ProteinNames[j];
                    if (buf[0] == '\"'){
                        buf = buf.Substring(1);
                    }
                    if (buf[buf.Length-1] == '\"'){
                        buf = buf.Substring(0,buf.Length-1);
                    }
                    IPIs.Add(buf);
                }
                MascotScore = Pep.Score;
                Sequence = Pep.Sequence;
                //модификации
                ModMass = 0.0;
                if (Pep.NModIndex != 0) {
                    ModMass += mp.VaryMods[Pep.NModIndex].Mass;
                    ModDesk += mp.VaryMods[Pep.NModIndex].Name+" on N-Terminus; ";
                }
                for(int k = 0 ; k < Pep.ModIndex.GetLength(0) ; k++){
                    if (Pep.ModIndex[k] != 0){
                        ModMass += mp.VaryMods[Pep.ModIndex[k]].Mass;
                        ModDesk += mp.VaryMods[Pep.ModIndex[k]].Name+" on position "+k.ToString()+"; ";
                    }
                }
                if (Pep.CModIndex != 0) {
                    ModMass += mp.VaryMods[Pep.CModIndex].Mass;
                    ModDesk += mp.VaryMods[Pep.CModIndex].Name+" on C-Terminus; ";
                }
            }
            Matches = null;
            AlreadySearched = false;
        }

        public QPeptide(){
            Matches = null;
            RefMatch = null;
            RefNumberMatch = null;
            AlreadySearched = false;
        }

        public static string TableCaption(){
            return 
                ((ReportString[0]!='1')?"":"SEQUENCE\t")+
                ((ReportString[1]!='1')?"":"RT MASCOT\t")+
                ((ReportString[2]!='1')?"":"RT APEX\t")+
                ((ReportString[3]!='1')?"":"RT SMD\t")+
                ((ReportString[4]!='1')?"":"tSHIFT\t")+
                ((ReportString[5]!='1')?"":"SVERT\t")+
                ((ReportString[6]!='1')?"":"SMD Ratio\t")+
                ((ReportString[7]!='1')?"":"ESI CORRECTION\t")+
                ((ReportString[8]!='1')?"":"INTEGRATED INTENSITY\t")+
                ((ReportString[9]!='1')?"":"REF.INT.\t")+
                ((ReportString[10]!='1')?"":"Log1\t")+
                ((ReportString[11]!='1')?"":"Log2\t")+
                ((ReportString[12]!='1')?"":"HIGHEST  INTENSITY\t")+
                ((ReportString[13]!='1')?"":"MZ MASCOT\t")+
                ((ReportString[14]!='1')?"":"MZ QUANTI\t")+
                ((ReportString[15]!='1')?"":"CHARGE\t")+
                ((ReportString[16]!='1')?"":"PI\t")+
                ((ReportString[17]!='1')?"":"MS SPECTRA\t")+
                ((ReportString[18]!='1')?"":"IPI\t")+
                ((ReportString[19]!='1')?"":"MASCOT SCORE\t")+
                ((ReportString[20]!='1')?"":"MODIFICATION MASS\t")+
                ((ReportString[21]!='1')?"":"ISOTOPE RATIO\t")+
                ((ReportString[22]!='1')?"":"ISOTOPE RATIO THEOR\t")+
                ((ReportString[23]!='1')?"":"COMMENTS");
        }

        public override string ToString(){
            string comment;
            QMatch Match;
            if(Matches == null || RefMatch == null) { 
                Match = new QMatch();
                comment = "ERROR: No main peak";
            }else{ 
                Match = RefMatch;
                comment = "";
                if (ModMass != 0.0){
                    comment = ModDesk;
                }
            }
            QMatch Refs;
            if (RefNumberMatch == null){
                Refs = new QMatch();
            }else{
                Refs = RefNumberMatch;
            }
              
            string Out = 
                ((ReportString[0]!='1')?"":String.Format("{0:20} \t",(Sequence ?? "NOSEQUENCE")))+
                ((ReportString[1]!='1')?"":String.Format("{0:f4}\t",MascotRT))+
                ((ReportString[2]!='1')?"":String.Format("{0:f4}\t",Match.ApexRT))+
                ((ReportString[3]!='1')?"":String.Format("{0:F6}\t",Match.RTDisp))+
                ((ReportString[4]!='1')?"":String.Format("{0}\t",Match.Shift))+
                ((ReportString[5]!='1')?"":String.Format("{0}\t",Match.MaxConv))+
                ((ReportString[6]!='1')?"":String.Format("{0}\t",Match.RTDisp/Refs.RTDisp))+
                ((ReportString[7]!='1')?"":String.Format("{0:f4}\t",Match.LongESICoef))+
                ((ReportString[8]!='1')?"":String.Format("{0:f2}\t",Match.Score))+
                ((ReportString[9]!='1')?"":String.Format("{0}\t",Refs.Score))+
                ((ReportString[10]!='1')?"":String.Format("{0}\t",Math.Log10(Match.Score)))+
                ((ReportString[11]!='1')?"":String.Format("{0}\t",Math.Log10(Refs.Score)))+
                ((ReportString[12]!='1')?"":String.Format("{0:f2}\t",Match.ApexScore))+
                ((ReportString[13]!='1')?"":String.Format("{0:f5}\t",MascotMZ))+
                ((ReportString[14]!='1')?"":String.Format("{0:f5}\t",Match.ApexMZ))+
                ((ReportString[15]!='1')?"":String.Format("{0}\t",Charge))+
                ((ReportString[16]!='1')?"":String.Format("{0}\t",PI))+
                ((ReportString[17]!='1')?"":String.Format("{0}\t",Match.MSMatches==null?0:Match.MSMatches.Count-2))+
                ((ReportString[18]!='1')?"":String.Format("{0}\t",(IPI ?? "NOPROTEIN")))+
                ((ReportString[19]!='1')?"":String.Format("{0:f2}\t",MascotScore))+
                ((ReportString[20]!='1')?"":String.Format("{0:f5}\t",ModMass))+
                ((ReportString[21]!='1')?"":String.Format("{0:f5}\t",Match.IsotopeRatio))+
                ((ReportString[22]!='1')?"":String.Format("{0:f5}\t",TheorIsotopeRatio))+
                ((ReportString[23]!='1')?"":String.Format("{0}",comment));
 

//              Out = String.Format("{0}\t{1}\t"+Out,RTNumber,MascotScan);
            return Out;

        }
        public bool CheckExist(){
            return RefMatch != null && RefNumberMatch != null && RefMatch.Score != 0.0 && RefNumberMatch.Score != 0.0;                        
        }

        public class byIPIs : IComparer<QPeptide> {
            public int Compare(QPeptide x, QPeptide y){
                return string.Compare(x.IPI,y.IPI);
            }
        }

        public class byMascotSN : IComparer<QPeptide> {
            public int Compare(QPeptide x, QPeptide y){
                if (x.MascotScan>y.MascotScan) { return 1;} 
                else if (x.MascotScan == y.MascotScan) { return 0; }
                else return -1;
            }
        }

        public class byApexRT : IComparer<QPeptide> {
            public int Compare(QPeptide x, QPeptide y){
                //if (string.Compare(x.Sequence,y.Sequence)>0) { return -1;} 
                //if (string.Compare(x.Sequence,y.Sequence)<0) { return 1;} 
                if (x.RefMatch == null && y.RefMatch == null ) return 0;
                if (x.RefMatch == null ) return -1;
                if (y.RefMatch == null ) return 1;
                if (x.RefMatch.ApexRT>y.RefMatch.ApexRT) { return 1;} 
                else if (x.RefMatch.ApexRT == y.RefMatch.ApexRT) { 
                    if (x.RefMatch.Score > y.RefMatch.Score) {return 1;}
                    else if (x.RefMatch.Score == y.RefMatch.Score) {return 0;}
                    return -1; 
                }
                else return -1;
            }
        }

        public class bySet: IComparer<QPeptide> {
            public int Compare(QPeptide x, QPeptide y){
                if (string.Compare(x.IPI,y.IPI)<0) { return -1;} 
                if (string.Compare(x.IPI,y.IPI)>0) { return 1;} 
                if (x.Sequence.Length < y.Sequence.Length) { return -1;} 
                if (x.Sequence.Length > y.Sequence.Length) { return 1;} 
                for (int i = 0 ; i < x.Sequence.Length ; i++){
                    if (x.Sequence[i] < y.Sequence[i]) { return -1;} 
                    if (x.Sequence[i] > y.Sequence[i]) { return 1;} 
                }
                if (x.ModMass < y.ModMass) { return -1;} 
                if (x.ModMass > y.ModMass) { return 1;} 
                if (string.Compare(x.ModDesk,y.ModDesk)>0) { return -1;} 
                if (string.Compare(x.ModDesk,y.ModDesk)<0) { return 1;} 
                //if (x.RefMatch.ApexRT < y.RefMatch.ApexRT) { return -1;} 
                //if (x.RefMatch.ApexRT > y.RefMatch.ApexRT) { return 1;} 

                return 0;
            }
        }

        public class byScore : IComparer<QPeptide> {
            public int Compare(QPeptide x, QPeptide y){
                if (x.RefMatch.Score>y.RefMatch.Score) { return -1;} 
                else if (x.RefMatch.Score == y.RefMatch.Score) { return 0; }
                else return 1;
            }
        }

        public double PI{      //изоэлектрическая точка - свойство считаемое по запросу 
            get {
                int DCount = 0, ECount = 0 , CCount = 0 , YCount= 0 , HCount= 0 , KCount= 0 , RCount= 0 ;
                if (Sequence == null) return 0.0;
                for (int i = 0 ; i < Sequence.Length ; i ++){
                    switch (Sequence[i]){
                        case 'D': DCount++; break;
                        case 'E': ECount++; break;
                        case 'C': CCount++; break;
                        case 'Y': YCount++; break;
                        case 'H': HCount++; break;
                        case 'K': KCount++; break;
                        case 'R': RCount++; break;
                    }
                }
                double NQ;
                for (double pH = 0 ; pH < 14.0 ; pH+=0.01){
                    NQ =(-1 / (1 + Math.Pow(10, 3.65 - pH))) + 
                        (1 / (1 + Math.Pow(10, pH - 8.2))) + 
                        (-DCount / (1 + Math.Pow(10, 3.9 - pH))) + 
                        (-ECount / (1 + Math.Pow(10, 4.07 - pH))) +
                        (-CCount / (1 + Math.Pow(10, 8.18 - pH))) +  
                        (-YCount / (1 + Math.Pow(10, 10.46 - pH))) + 
                        (HCount / (1 + Math.Pow(10, pH - 6.04))) +
                        (KCount / (1 + Math.Pow(10, pH - 10.54))) +
                        (RCount / (1 + Math.Pow(10, pH - 12.48)));
                    if (NQ<=0.0){
                        return pH;
                    }
                }
                return 0.0;
            }
        }


    };
}