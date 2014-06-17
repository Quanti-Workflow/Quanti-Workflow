using System;

namespace PeakDetecting
{

    public struct peakinside {
       public int Extrem;  //� �������� �������� ����������� ������� � ������� Data
       public double Height;
       public int LeftBound;
       public int RightBound;
       public int IndLeftMin;
       public int IndRightMin;
       public double Center;
       public double Value;
       public double SKO;
       public double Skew;
       public double Kurt;
       //�������� ��������� ����, ��� ���������� ���������� ����� ����������� �����
       public double FWRL; //������ ���� �� ������ ����������
       public double ExactRightBound;  //����.�������� ������ ������� �� ������ ����������
       public double ExactLeftBound;   //����.�������� ����� ������� �� ������ ����������
       public double Resolution;
    } 

    public class PeakDetector{

        const double NoiseRate = 3;
        const double PeakBounds = 0.1; 

        public double NoiseLevel;

        int[] ExtremInd;

        int ExtremCount;



        //��������� ���� �� ������� ����������� �������� 
        void AddNulls(){
            Quanty.MZData[] Added = new Quanty.MZData[Data.Length*2];
            //���� ������� 
            int i, Count = 0;
            double Diff = 0.1;
            //������ �����
            if (Data[0].Intensity > 0){
                Added[0].Intensity = 0;
                Added[0].Mass = Data[0].Mass-(Data[1].Mass-Data[0].Mass);
                Count++;
            }
            //��� ������� ����� 
            for (i = 0; i < Data.Length-1 ; i++ ){
                Added[Count] = Data[i];
                Count++;
                //���� ������ 
                if (Data[i+1].Mass - Data[i].Mass > Diff*1.5){
                    //����������� ��� ����� - ����� � �� ������� 
                    if (Data[i].Intensity > 0 ){
                        Added[Count].Intensity = 0;
                        Added[Count].Mass = Data[i].Mass+Diff;
                        Count++;
                    }
                    if (Data[i+1].Intensity > 0 ){
                        Added[Count].Intensity = 0;
                        Added[Count].Mass = Data[i+1].Mass - (Data[i+2].Mass - Data[i+1].Mass);
                        Count++;
                    }
                }else{
                    Diff = Data[i+1].Mass - Data[i].Mass;
                }
            }
            //��������� ����� 
            if (Data[i].Intensity > 0){
                Added[Count].Intensity = 0;
                Added[Count].Mass = Data[i].Mass+Diff;
                Count++;
            }
            Data = new Quanty.MZData[Count+1];

            for ( i = 0 ; i<Count ; i++ ){
                Data[i] = Added[i];
            }
        }

        void NoiseLevelDetect(bool bDetect)
        {

	        //���� ��� ���������

	        int i,j,Count=0,DCount,DefeatedCount;
	        double Sum = 0,Avg;
	        double nMax=0;

            Quanty.MZData[] Extrems = new Quanty.MZData[Data.Length]; 
            ExtremInd = new int[Data.Length]; 
	        //���� ����� ������ ����� - ����������� ����� ������

           //��� ������� ������ ���������� ������ ����� ����-�������, ���������� �� ���������
           for(j=1; j<=Data.Length-2 ; j++)
           {
              if (Data[j].Intensity > Data[j-1].Intensity && 
                  Data[j].Intensity >=Data[j+1].Intensity && 
                  (bDetect || Data[j].Intensity > NoiseLevel))
	          {
                 Extrems[Count]=Data[j];
		         if(Data[j].Intensity > nMax) nMax=Data[j].Intensity;
		         ExtremInd[Count]=j;
                 Sum+=Data[j].Intensity;
                 Count++;
              }
           }
           ExtremCount=Count;
           DCount = Count;

           if(bDetect)
           {
		        //������� ������� ����������
		        //������� ����������� ���������� ��������� ��� ������� �� ���������
		        //������� ������� �������� ���������� ������ ��� � ��� ����
		        //������� ���������� ������� ������� ���� �����������
		        //����� ������� �� ������� ��� ���������
		        //����� ��������� ������� ��� ���� ����������
		        //������ ������� ���� �� ������ ��������� ��������� ���������
		        do {
			        //���������� ������� �������
                    //!!��� ���!!
			        Avg = (Sum/DCount)*NoiseRate;
			        //���������� ���� �� ����������� ���� �������
			        DefeatedCount = 0;
			        for ( j=0 ; j<Count ; j++ ) {
				        if (Extrems[j].Intensity > Avg){
					        Extrems[j].Intensity = 0.0;
					        DefeatedCount++;
				        }
			        }
			        DCount=0;
			        //���������� ����� ������� �������� ����������
			        for ( j=0,Sum=0 ; j<Count ; j++ ) {
				        if (Extrems[j].Intensity != 0){
					        Sum+=Extrems[j].Intensity;
					        DCount++;
				        }
			        }
		        }
		        while (DefeatedCount>0);
		        NoiseLevel = Avg;
           }

           //�������� � ������ ������� ����������, ������� ���� ������ � ������ ��������� 

           int[] ExtremBuf = new int[Count - DCount];
           Count=0;
           for(i=0;i<ExtremCount;i++){
	           if(Data[ExtremInd[i]].Intensity > NoiseLevel)  {
		           ExtremInd[Count]=ExtremInd[i]; Count++;
	           }
           }
           ExtremCount=Count;

           //�� ��������� � ������� ������ ExtremInd[i], ������ ExtremCount ��������� ��������
           //��������� �� ���������� �����, ������� ���� ������ � ����� ������ ������������ ���������
        }

//-------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------

        private Quanty.MZData[] Data;

        public int PeaksDetecting(  ref Quanty.MZData[] inData, 
                                    ref peakinside[] Outs)
        {
	        double ResLevel;
	        int i, j, WorkCount, Count, cnt, nResult;

            int LastMinRight;

            Data = inData;

	        ExtremInd = new int[Data.Length];
       		
	        //���� ������ ����������� ������ ���� - ���������� ���
	        ExtremCount=0;
            AddNulls();
	        NoiseLevelDetect(true);


	        peakinside[] Peaks = new peakinside[ExtremCount];

	        cnt=0; WorkCount = 0; LastMinRight=1;

	        while(cnt<ExtremCount)
	        {
		        nResult=0;
    						
		        Peaks[WorkCount].Extrem =ExtremInd[cnt]; 
    						
		        //�������������, �� ������� ����������� ���������� ����
		        ResLevel=PeakBounds*Data[ExtremInd[cnt]].Intensity; 
		        //���� ����� - ���� ������ �������, ����������� ���
		        i=ExtremInd[cnt]-1;
		        while(true)
		        {
                    //�������� � ������� ������������ ����� �������
    				if(i<=LastMinRight){
                        nResult=1; 
                        break;
                    } 

                    //������ ����������� �������
			        if(Data[i].Intensity<=ResLevel){
				        nResult=0; break;
			        }
			        i--;
		        }
    			
		        if (nResult == 0) //���� ���������� ����������� ������������ �������� �����
		        {
			        Peaks[WorkCount].LeftBound=i+1;
    				
			        //���� ������ - ���� ������ �������, ����������� ���
                    //�� ��������� ���� �� ����� �������� �������� ��� ��� �� ������� �� �� ������
			        i=ExtremInd[cnt]+1;
			        while(true)
			        {
                        //�� ���� ����� ������ �������� ��������� ����� ����������� ��������
				        if(Data[i].Intensity >Data[Peaks[WorkCount].Extrem].Intensity){
                            nResult=1; 
                            break;
                        } 

                        //������ ����������� �������
				        if(Data[i].Intensity<=ResLevel){
					        nResult=0; 
                            break;
				        }
				        i++;
			        }

			        if(nResult==0) //���� ���������� ����������� ������������ �������� ������
			        {
				        Peaks[WorkCount].RightBound=i-1;

				        Peaks[WorkCount].Height =Data[Peaks[WorkCount].Extrem].Intensity;
    									
				        //�������� ��������� ����� ������� ����
				        Peaks[WorkCount].ExactLeftBound = Peaks[WorkCount].LeftBound;

				        if((Data[Peaks[WorkCount].LeftBound].Intensity-Data[Peaks[WorkCount].LeftBound-1].Intensity)!=0)
    				        Peaks[WorkCount].ExactLeftBound-=
                                (Data[Peaks[WorkCount].LeftBound].Intensity-ResLevel)/
                                (Data[Peaks[WorkCount].LeftBound].Intensity-(double)Data[Peaks[WorkCount].LeftBound-1].Intensity);
    					
				        //�������� ��������� ������ ������� ����
				        Peaks[WorkCount].ExactRightBound = Peaks[WorkCount].RightBound;
				        if((Data[Peaks[WorkCount].RightBound+1].Intensity-Data[Peaks[WorkCount].RightBound].Intensity)!=0)
				            Peaks[WorkCount].ExactRightBound+=
                                (Data[Peaks[WorkCount].RightBound].Intensity-ResLevel)/
                                (Data[Peaks[WorkCount].RightBound].Intensity-(double)Data[Peaks[WorkCount].RightBound+1].Intensity);
    					
				        //����������� ������������ ������������ ����� �������
				        //�� ������ ������� ���������� ���������� ����
				        LastMinRight=Peaks[WorkCount].RightBound; 

				        WorkCount++; //����������� ������ �� ��������� ��� ������ ����
							        //��������� ��������� ��� ��� ����������
			        }
		        }
		        switch(nResult)
		        {
		            case(2):  cnt=ExtremCount; break; //����� �� ����� ��������� - ���������� ���������� �����
			        default:  cnt++;    break;
		        }
	        }
            Count = WorkCount;
       
	        //��������������, ����� ����������
	        //� ������ ������ ������� ����� ��������� �� ����������� ����� ������ ������� �����
	        //�������� ����������� �� ������ ���� ������ ���� ������ ������� �����
	        //������ ���� �������� � ������ ������������� (�� ������ �������) ����
    		
	        double TruncLevel; //������� ������� ����
	        double fPeakSum=0.0;
	        double fMoment=0.0;
	        double fTemp, fDelta, fMassDelta;
	        for (i = 0 ; i < Count; i++ )
	        {
                //��� �������� ������� �������� 
                double[]  XJ = new double[(Peaks[i].RightBound - Peaks[i].LeftBound)*2 +8];
                double[]  SJ = new double[(Peaks[i].RightBound - Peaks[i].LeftBound)*2 +8];
                int Index = 0;

		        TruncLevel = (Data[Peaks[i].Extrem].Intensity)*PeakBounds;
		        fPeakSum=0.0;
		        fMoment=0.0;
                double fMassDeltaSumm = 0.0;
    			
		        for(j=Peaks[i].LeftBound; j<Peaks[i].RightBound; j++)
		        {
			        fMassDelta = Data[j+1].Mass - Data[j].Mass;
                    fMassDeltaSumm+=fMassDelta;

                    fTemp=(Data[j+1].Intensity - Data[j].Intensity)*0.5*fMassDelta;
			        fPeakSum+=fTemp;
			        fMoment+=fTemp*(Data[j].Mass+fMassDelta*(2.0/3.0));

                    SJ[Index] = fTemp; 
                    XJ[Index] = fTemp*(Data[j].Mass+fMassDelta*(2.0/3.0)); 
                    Index++;


                    fTemp=Data[j].Intensity*fMassDelta;
    			    fPeakSum+=fTemp;
                    fMoment+=fTemp*(Data[j].Mass+fMassDelta*0.5);

                    SJ[Index] = fTemp; 
                    XJ[Index] = fTemp*(Data[j].Mass+fMassDelta*0.5);
                    Index++;

		        }

		        fDelta=(double)Peaks[i].LeftBound-Peaks[i].ExactLeftBound;
                fMassDelta = (Data[Peaks[i].LeftBound].Mass - Data[Peaks[i].LeftBound-1].Mass)*fDelta;
                fMassDeltaSumm+=fMassDelta;

                fTemp=(Data[Peaks[i].LeftBound].Intensity-TruncLevel)*fMassDelta*0.5;
                fPeakSum+=fTemp;

                fMoment+=fTemp* (Data[Peaks[i].LeftBound].Mass-fMassDelta/3.0);

                SJ[Index] = fTemp; 
                XJ[Index] = fTemp* (Data[Peaks[i].LeftBound].Mass-fMassDelta/3.0);
                Index++;


                fTemp=TruncLevel*fMassDelta;
                fPeakSum+=fTemp;

                fMoment+=fTemp*(Data[Peaks[i].LeftBound].Mass-fMassDelta*0.5);

                SJ[Index] = fTemp; 
                XJ[Index] = fTemp*(Data[Peaks[i].LeftBound].Mass-fMassDelta*0.5);
                Index++;


                fDelta=Peaks[i].ExactRightBound-(double)Peaks[i].RightBound;
                fMassDelta = (Data[Peaks[i].RightBound+1].Mass - Data[Peaks[i].RightBound].Mass)*fDelta;
                fMassDeltaSumm+=fMassDelta;

		        fTemp=(TruncLevel-Data[Peaks[i].RightBound].Intensity)*fMassDelta*0.5;
		        fPeakSum+=fTemp;

		        fMoment+=fTemp* (Data[Peaks[i].RightBound].Mass+fMassDelta*(2.0/3.0));

                SJ[Index] = fTemp; 
                XJ[Index] = fTemp* (Data[Peaks[i].RightBound].Mass+fMassDelta*(2.0/3.0));
                Index++;


                fTemp=Data[Peaks[i].RightBound].Intensity*fMassDelta;
                fPeakSum+=fTemp;
                fMoment+=fTemp*(Data[Peaks[i].RightBound].Mass+fMassDelta*0.5);

                SJ[Index] = fTemp; 
                XJ[Index] = fTemp*(Data[Peaks[i].RightBound].Mass+fMassDelta*0.5);
                Index++;

		        Peaks[i].Value = fPeakSum;
		        Peaks[i].Center = fMoment/fPeakSum;
		        Peaks[i].FWRL=Peaks[i].ExactRightBound-Peaks[i].ExactLeftBound;
		        Peaks[i].Resolution=Peaks[i].Center/fMassDeltaSumm;

                //������� ������� �������� 
                for ( j = 0 ; j < Index ; j++){
                    Peaks[i].SKO += (XJ[j]/SJ[j]-Peaks[i].Center)*(XJ[j]/SJ[j]-Peaks[i].Center)*SJ[j];
//                    Peaks[i].Skew += (XJ[j]/SJ[j]-Peaks[i].Center)*(XJ[j]/SJ[j]-Peaks[i].Center)*(XJ[j]/SJ[j]-Peaks[i].Center)*SJ[j];
//                    Peaks[i].Kurt += (XJ[j]/SJ[j]-Peaks[i].Center)*(XJ[j]/SJ[j]-Peaks[i].Center)*(XJ[j]/SJ[j]-Peaks[i].Center)*(XJ[j]/SJ[j]-Peaks[i].Center)*SJ[j];
                }
                Peaks[i].SKO = Math.Sqrt(Peaks[i].SKO/fPeakSum);

//                Peaks[i].Skew = Peaks[i].Skew / fPeakSum; 

//                Peaks[i].Kurt = Peaks[i].Kurt / fPeakSum; 


	        }

	        //��������� ���� � �������� ����������� ��� ��������������
	        WorkCount=0;
	        for (i = 0; i<Count ; i++){
		        if (Peaks[i].Extrem!=0 && Peaks[i].Resolution>0 && Peaks[i].FWRL>0 && Peaks[i].Center>0 && Peaks[i].Value>0){
                    Peaks[WorkCount] = Peaks[i];
			        WorkCount++;
		        }
	        }
	        Count = WorkCount;
            Outs = Peaks;

            return Count;

         }
    }
}

