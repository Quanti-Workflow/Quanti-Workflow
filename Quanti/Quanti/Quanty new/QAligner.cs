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
using System.Threading;
using System.Windows.Forms;
using Quanty.Properties;

namespace Quanty
{
    public abstract class QAligner
    {
        public List<Object> AlList;
        static public ILogAndProgress Log;
        public void Run(){
            Log.RepProgress(0);
            for(int i = 0 ; i < AlList.Count ; i++){
                ThreadProc(AlList[i]);
                //AlList[i].Run();
                Log.RepProgress(100*i/AlList.Count);
            }
        } 

        public void RunFast(){
            int WorkerThreads, IOThreads;
            ThreadPool.GetMaxThreads(out WorkerThreads, out IOThreads);
            WorkerThreads = Environment.ProcessorCount;
            ThreadPool.SetMaxThreads(WorkerThreads, IOThreads);
            Count = 0;
            int LocalCount;
            for(int i = 0 ; i < AlList.Count ; i++){
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadProc),AlList[i]);
            }
            while(true){
                lock (ForCountLock){
                    LocalCount = Count;
                }
                Thread.Sleep(200);
                Log.RepProgress((LocalCount*100)/AlList.Count);
                if (LocalCount == AlList.Count) break;
            }
        }

        public Object ForCountLock = new object();
        public Int32 Count;

        virtual public void ThreadProc(Object stateInfo) {
            (stateInfo as QAlignment).Run();
            lock (ForCountLock){
                Count++;
            }
        }

        abstract public double[] GetPeptRatios(QPeptide Pept);
        abstract public PeptidePair GetPeptPair(QPeptide Pept, int Left, int Right);
        abstract public ProteinPair GetProtPair(QProtein Prot, int Left, int Right);
    }

    public class MatrixAligner : QAligner{
        QAlInfo[,] AlMatrix;
        int FileCount;
        double[,][] PeptideRatios;
        double[, ,] ProteinMedians;
        double[, ,] ProteinSlopes;
        double[, ,] ProteinDeltas;
        List<QPeptide> Peptides;
        List<QProtein> Proteins;

        override public void ThreadProc(Object stateInfo){
            int i, j;
            i = (stateInfo as QAlInfo).i;
            j = (stateInfo as QAlInfo).j;
            QAlignment Align = new QAlignment(Peptides, Proteins, i, j);
            Align.Run();
            Align.Proteins.Sort(new ProteinPair.byIPI());
            Align.Peptides.Sort(new PeptidePair.bySet());
            //копирование итоговых значений в итоговые массивы
            for (int k = 0 ; k < Peptides.Count ; k++){ 
                PeptidePair PP = Align.Peptides[k];
                if (PP.Ratio == 0.0){
                    PeptideRatios[i,j][k] = 0.0;
                }else{
                    PeptideRatios[i,j][k] = PP.Ratio;
                }
            }
            for (int k = 0 ; k < Proteins.Count ; k++){ 
                ProteinPair PP = Align.Proteins[k];
                ProteinMedians[i, j, k] = PP.Median;
                ProteinSlopes[i, j, k] = PP.Slope;
                ProteinDeltas[i, j, k] = PP.SlopeDelta;
            }
            lock (ForCountLock){
                Count++;
            }
        }

        class QAlInfo{
            public int i;
            public int j;
        }

        public MatrixAligner(List<QPeptide> Peptides, List<QProtein> Proteins,int FileCount){
            AlList = new List<Object>();
            AlMatrix = new QAlInfo[FileCount, FileCount];
            //этот массив иногда оказывается больше 2 гиг
            PeptideRatios = new double[FileCount, FileCount][];
            for ( int i = 0 ; i < FileCount ; i++){
                for ( int j = 0 ; j < FileCount ; j++){
                    PeptideRatios[i,j] = new double[Peptides.Count];
                }
            }
            ProteinMedians = new double[FileCount, FileCount, Proteins.Count];
            ProteinSlopes = new double[FileCount, FileCount, Proteins.Count];
            ProteinDeltas = new double[FileCount, FileCount, Proteins.Count];
            this.Peptides = Peptides;
            this.Proteins = Proteins;
            this.FileCount = FileCount;
            Peptides.Sort(new QPeptide.byIPIs());
            Proteins.Sort(new QProtein.byIPI());
            for (int i = 0 ; i < FileCount ; i++){
                for (int j = 0 ; j < FileCount ; j++){
                    AlMatrix[i, j] = new QAlInfo();
                    AlMatrix[i, j].i = i;
                    AlMatrix[i, j].j = j;
                    AlList.Add(AlMatrix[i,j]);
                }
            }
        }

        public int GetPeptIndex(QPeptide Pept){
            int Index=Peptides.BinarySearch(Pept, new QPeptide.bySet());
            return Index;
        }

        public int GetProtIndex(QProtein Prot){
            int Index=Proteins.BinarySearch(Prot, new QProtein.byIPI());
            return Index;
        }

        //надеемся что они синхронизированы
        public override double[] GetPeptRatios(QPeptide Pept){
            double[,] RatioMatrix = new double[FileCount, FileCount];
            int Index = GetPeptIndex(Pept);
            for ( int i = 0 ; i < FileCount ; i++){
                for ( int j = 0 ; j < FileCount ; j++){
                    RatioMatrix[i, j] = PeptideRatios[i,j][Index];
                }
            }
            double[,] ErrorMatrix = null;
            double[] Res = new double[FileCount];
            Res = SolveMatrix(RatioMatrix, ref ErrorMatrix);
            return Res;
        }

        //надеемся что они синхронизированы
        public double[] GetProtRatios(QProtein Prot,bool Errors){
            double[,] MedianMatrix = new double[FileCount, FileCount];
            double[,] SlopeMatrix = new double[FileCount, FileCount];
            double[,] SlopeDeltas = new double[FileCount, FileCount];
            int Index = GetProtIndex(Prot);

            for ( int i = 0 ; i < FileCount ; i++){
                for ( int j = 0 ; j < FileCount ; j++){
                    MedianMatrix[i, j] = ProteinMedians[i,j,Index];
                    SlopeMatrix[i, j] = ProteinSlopes[i,j,Index];
                    SlopeDeltas[i, j] = ProteinDeltas[i,j,Index];
                }
            }

            double[,] ErrorMatrix = null;
            double[,] FakeMatrix = null;

            double[] Res = SolveMatrix(MedianMatrix,ref FakeMatrix );
            if (!Errors) return Res;

            double[] Slopes = SolveMatrix(SlopeMatrix,ref ErrorMatrix);
            double[] Errs = SolveMatrix(SlopeDeltas,ref FakeMatrix);
            double[] MErrs = new double[FileCount];
            //обрабатываем матрицу ошибок матрицы
            for ( int i = 0 ; i < FileCount ; i++){
                double AveX = 0.0;
                int Count = 0;
                for (int k = 0 ; k < FileCount ; k++){
                    if ( ErrorMatrix[i,k] != 0.0 ) { 
                        AveX += ErrorMatrix[i,k];
                        Count++;
                    }
                }
                if (Count<2){
                    MErrs[i] = 0.0;
                    continue;
                }
                AveX = AveX/Count;
                double SumD = 0.0; 
                for (int k = 0 ; k < FileCount ; k++){
                    if ( ErrorMatrix[i,k] != 0.0 ) { //уточнить !! [i,k] или [k,i]
                        SumD += (ErrorMatrix[i,k]-AveX)*(ErrorMatrix[i,k]-AveX);
                    }
                }
                MErrs[i]= (Math.Sqrt(SumD)/(Count-1))*alglib.studenttdistr.invstudenttdistribution(Count-1,0.833);
            }
            double[] ResErr = new double[FileCount*3];
            for (int i = 0 ; i < FileCount ; i++){
                ResErr[i] = Res[i]; 
            }

            for (int i = 0 ; i < FileCount ; i++){
                ResErr[i+FileCount] = Slopes[i]*(1+Errs[i]+MErrs[i])-Res[i];
            }

            for (int i = 0 ; i < FileCount ; i++){
                ResErr[i+FileCount+FileCount] = Res[i]-Slopes[i]*(1-Errs[i]-MErrs[i]);
            }

            return ResErr;
        }

        public override PeptidePair GetPeptPair(QPeptide Pept, int Left, int Right){
            return null;
        }

       

        public override ProteinPair GetProtPair(QProtein Prot, int Left, int Right){
            return null;
        }


        static public double[] SolveMatrix(double[,] Matrix, ref double[,] ErrorMatrix){
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
                if (Count>1){
                    Res[i] = Math.Pow(Res[i],1.0/(double)Count);
                    if (Res[i] == 1.0) {
                        SCount++;
                    }
                }else{
                    //работа с диагональными единицами
                    //Res[i] = 0.0;
                    if (Count == 1){
                        Res[i] = 1.0;
                    }else{
                        Res[i] = 0.0;
                    }
                }
            }
            double[,] Errors = new double[Len,Len];
            double[] Adj = new double[Len];
            for (int i = 0 ; i < Len ; i++){
                for (int j = 0 ; j < Len ; j++){
                    if (Matrix[i,j]!=0.0 && i != j){
                        Errors[i,j] = Matrix[i,j]*Res[j]-Res[i];
                    }else{
                        Errors[i,j] = 0.0;
                    }
                }
            }
            ErrorMatrix = Errors;

            return Res;
        }
    }

    public class RefAligner : QAligner{
        QAlignment[] AlLine;
        int FileCount;
        int RefNumber;

        public RefAligner(List<QPeptide> Peptides, List<QProtein> Proteins,int FileCount, int RefNumber){
            AlList = new List<Object>();
            AlLine = new QAlignment[FileCount];
            this.FileCount = FileCount;
            this.RefNumber = RefNumber;
            for (int i = 0 ; i < FileCount ; i++){
                AlLine[i] = new QAlignment(Peptides, Proteins, i, RefNumber);
                AlList.Add(AlLine[i]);
            }
        }

        public override PeptidePair GetPeptPair(QPeptide Pept, int Left, int Right){
            if (Right != RefNumber || Left > FileCount - 1) return null;
            QAlignment TargetAl = AlLine[Left];
            for (int i = 0; i < TargetAl.Peptides.Count; i++ ){
                if (TargetAl.Peptides[i].Peptide == Pept){
                    return TargetAl.Peptides[i];
                }
            }
            return null;
        }

        public override ProteinPair GetProtPair(QProtein Prot, int Left, int Right){
            if (Right != RefNumber || Left > FileCount - 1) return null;
            QAlignment TargetAl = AlLine[Left];
            for (int i = 0; i < TargetAl.Proteins.Count; i++ ){
                if (TargetAl.Proteins[i].Protein == Prot){
                    return TargetAl.Proteins[i];
                }
            }
            return null;
        }

        public override double[] GetPeptRatios(QPeptide Pept){
            double [] Res = new double[FileCount];
            for (int i = 0 ; i < FileCount ; i++){
                PeptidePair PPair = GetPeptPair(Pept, i, RefNumber);
                Res[i] = PPair.Ratio;
            }
            return Res;
        }
    }
 
}