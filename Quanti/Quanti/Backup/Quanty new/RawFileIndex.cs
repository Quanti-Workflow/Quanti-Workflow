using System;
using System.Collections.Generic;
using System.Text;
using XRAWFILE2Lib;
using Quanty.Properties;

namespace Quanty {

    public struct MZData {
        public double Mass;
        public double Intensity;
    }

    public struct RawData{
        public MZData[] Data;
        public double RT;
    }
    
    public partial class RawFileBox {
        
        public RawData[] RawSpectra; 
        public XRawfile RawFile; 
        public int Spectra;
        public string RawFileName;

        public int[] ms2index; //��� ������� ������� ���� ����� ����� ���������� full-MS �������
        //��������� ������ ������ ��������������� full-MS ��������
        public int[] IndexDir; //��������� �� ����� ����� ���������� full-MS �������
        public int[] IndexRev; //��������� �� ����� ����� ����������� full-MS �������

        public double[] ESICurrents; //�������� ���� ������������� 
        public double[] TimeStamps; //���������� ����� ����������� MS-only ������� - � �������
        public double[] TimeCoefs;
        double AverageTimeStamp;

        public bool RTCorrection;
        public bool ESICorrection;

        MZData[] Buf;

        double LowRT;
        double HighRT;
        double TotalRT;

        //�������� �� ��������
        public static ILogAndProgress iLog = null;


        public RawFileBox(){
        }

        public int LoadIndex(string FileName){

            this.RawFileName = FileName;

            RawFile = new XRawfile();

            RawFile.Open(FileName);
            RawFile.SetCurrentController(0, 1);

            Spectra = 0;
            RawFile.GetNumSpectra(ref Spectra);

            if( Spectra <= 0) 
                return 0;

	        int i, lastfull = 0, total = 0;
            double TotalEsi = 0.0;

	        ms2index = new int[Spectra+1];
            IndexDir = new int[Spectra+1];
            IndexRev = new int[Spectra+1]; 
            RawSpectra = new RawData[Spectra+1];
            Buf = new MZData[1000000];

            ESICurrents = new double[Spectra+1];
            TimeStamps = new double[Spectra+1];
            TimeCoefs = new double[Spectra+1];

            string Filter = null;

            LowRT = 0.0;
            HighRT = 0.0;

            for(i = 1; i <= Spectra; i++){

                iLog.RepProgress((int)(100.0*((double)i/(double)Spectra)));

		        RawFile.GetFilterForScanNum(i, ref Filter);

		        //YL - ��� �������� ms-only
		        if((Filter.IndexOf(" ms ") != -1) && (Filter.IndexOf("FTMS") != -1)) { //is a FULL MS

			        TimeStamps[i] = RawSpectra[lastfull].RT;
                    
                    IndexDir[lastfull] = i;
			        IndexRev[i] = lastfull;

			        lastfull = i;
			        ms2index[i] = lastfull;

			        ++total;

				    RawFile.RTFromScanNum(i, ref RawSpectra[i].RT);
                    TotalRT = RawSpectra[i].RT;

                    TimeStamps[i] = RawSpectra[i].RT - TimeStamps[i];

                    object Labels = null;
                    object Values = null;
                    int ArraySize = 0;
                    double RT = 0.0; 

                    RawFile.GetStatusLogForScanNum(i, ref RT, ref Labels, ref Values , ref ArraySize);

                    for (int k = 0 ; k < ArraySize ; k++ ){
                        if ((Labels as Array).GetValue(k).ToString().Contains("Source Current")){
                            ESICurrents[i] = Convert.ToDouble((Values as Array).GetValue(k).ToString());
                            TotalEsi+=ESICurrents[i];
                        }
                    }


		        }
		        else {
			        ms2index[i] = lastfull;
		        }
		        Filter = null ;
	        }
            IndexDir[lastfull] = -1;
            TotalRT = RawSpectra[lastfull].RT;
            AverageTimeStamp = TotalRT/total;

            //����������� ��������� ������������ 
            for (i = IndexDir[0] ; IndexDir[i] != -1 ; i = IndexDir[i]) {

                TimeCoefs[i] = (TimeStamps[i]+TimeStamps[IndexDir[i]])/(2.0*AverageTimeStamp);

                ESICurrents[i] = ESICurrents[i]/(TotalEsi/(double)total);
            }
            TimeCoefs[i] = 1.0;
            
            return Spectra;
        }

        public void LoadInterval(double MinRT, double MaxRT){
            int Index = 0;
            //�� �������� ���� ���� ������ 
            while (RawSpectra[IndexDir[Index]].RT<MinRT){
                RawSpectra[Index].Data = null;
                Index = IndexDir[Index];
            }
            while (RawSpectra[IndexRev[Index]].RT<MaxRT){
                if (RawSpectra[Index].Data == null) {
                    ReadMS(Index);
                }
                iLog.RepProgress((int)((RawSpectra[IndexRev[Index]].RT/MaxRT)*100));
                if (IndexDir[Index] == -1) break;
                Index = IndexDir[Index];
            }
            while (RawSpectra[Index].RT<TotalRT){
                RawSpectra[Index].Data = null;
                if (IndexDir[Index] == -1) break;
                Index = IndexDir[Index];
            }
            LowRT = MinRT;
            HighRT = MaxRT;
        }


        void ReadMS(int Scan){
	        int ArraySize = 0;
            Object MassList = null, EmptyRef=null;
            double temp=0.0;

            try {
	            RawFile.GetMassListFromScanNum(ref Scan, null, 
		            0, //type
                    0, //value
                    0, //peaks
                    0, //centeroid
                    ref temp,
		            ref MassList, 
                    ref EmptyRef, 
                    ref ArraySize);
            }
            catch{
                Exception e = new Exception(string.Format("Scan #{0} cannot be loaded, probably RAW file is corrupted!",Scan));
                throw e;
            }
			
            //RawSpectra[Scan].Data = new MZData[ArraySize];

            for ( int j = 0 ; j<ArraySize ; j++){
                Buf[j].Mass = (double)(MassList as Array).GetValue(0,j);
                Buf[j].Intensity =  (double)(MassList as Array).GetValue(1,j);
            }

            MassList = null;
            GC.Collect(2);

            //if (Settings.Default.Centroids){
            //    RawSpectra[Scan].Data = PeakDetect(Buf);
            //}else{
            //    RawSpectra[Scan].Data = Centroid(Buf, ArraySize);
            //}
            int isCentroided = 0;

            RawFile.IsCentroidScanForScanNum(Scan,ref isCentroided);

            RawSpectra[Scan].Data = Centroid(Buf, ArraySize, isCentroided != 0);
        }

        public MZData[] PeakDetect(MZData[] Data ){

            PeakDetecting.PeakDetector pd = new PeakDetecting.PeakDetector();
            PeakDetecting.peakinside[] Peaks = new PeakDetecting.peakinside[1];
            pd.PeaksDetecting(ref Data, ref Peaks);
	        MZData[] OutData = new MZData[Peaks.GetLength(0)];
            for (int i = 0 ; i < Peaks.GetLength(0) ; i++){
                OutData[i].Intensity = Peaks[i].Value;
                OutData[i].Mass = Peaks[i].Center;
            }
            return OutData;
        }

        public MZData[] Centroid(MZData[] Data,int Len, bool StickMode /* former "in" */)
        {
	        int total = 0, u;
	        int o = 0, i = 0, count = Len;
	        double sumIi, sumI, last = 0.0;
            double du = 0.0;
	        bool goingdown = false;
            MZData[] OutData;

            if (StickMode) {
                //������� ���� �� �������� ���� ��� ���� ��������� �� ������ ������ ������� 
                for ( i = 1 ; i<count ; i++){
                    if (Data[i].Mass < Data[i-1].Mass || Data[i].Mass == 0){
                        break;
                    }
                }
                OutData = new MZData[i];
                count = i;
                for (i=0; i<count ; i++){
                    OutData[i].Intensity = Data[i].Intensity;
                    OutData[i].Mass = Data[i].Mass;
                }
                return OutData;
            }


            //������� ��������� �����
	        while(i < count && Data[i].Intensity == 0.0) ++i;

	        //������� ������� ������ ���� 
	        while(i < count)
	        {
		        while(i < count && Data[i].Intensity != 0.0)
		        {
			        if(last > Data[i].Intensity) {
                        goingdown = true;
                    }else{
                        if(goingdown) {
				            ++total;
				            goingdown = false;
    			        }
                    }

			        last = Data[i].Intensity;
			        ++i;
		        }

		        last = 0.0;
		        goingdown = false;

		        while(i < count && Data[i].Intensity == 0.0) 
                    i++;

		        total++;
	        }

	        //�������� ������ �� ������������ ������� 
	        OutData = new MZData[total];
	        i = 0; o = 0; total = 0; last = 0.0; goingdown = false;

	        while(i < count && Data[i].Intensity == 0.0) i++;

	        while(i < count)
	        {
		        sumIi = sumI = 0.0;
		        o = i -1;
		        while(i < count && Data[i].Intensity != 0.0){

			        //���� ����� �� ����
			        if(last > Data[i].Intensity) {
                        goingdown = true;
                    }else{
                        if(goingdown) {
				            u = Convert.ToInt32((sumIi / sumI)/* + 0.5*/);
				            OutData[total].Intensity = sumI;
				            OutData[total].Mass = Data[o+u].Mass;
				            ++total;

				            sumIi = sumI = 0.0;
				            o = i -1;
				            goingdown = false;
    			        }
                    }

			        sumIi += Data[i].Intensity*(i-o);
			        sumI += Data[i].Intensity;

			        last = Data[i].Intensity;
			        i++;
		        }

		        u = Convert.ToInt32((sumIi / sumI) /*+0.5*/ );
                du = sumIi / sumI - (double)u;
		        //������������� �� ��������� 
		        OutData[total].Intensity = sumI;
		        //�������� - �� ������ 
		        //OutData[total].Mass = Data[o+u].Mass;
                //�������� �� ������
                OutData[total].Mass = Data[o+u].Mass*(1-du) + Data[o+u+1].Mass*du;

		        last = 0.0;
		        goingdown = false;

		        while(i < count && Data[i].Intensity == 0.0) 
                    i++;
		        total++;
	        }
            return OutData;
        }

    }
}
