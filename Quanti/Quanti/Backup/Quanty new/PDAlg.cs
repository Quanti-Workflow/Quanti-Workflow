using System;

namespace PeakDetecting
{

    public struct peakinside {
       public int Extrem;  //В качестве значений фактические индескы в массиве Data
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
       //Ширинные параметры пика, при включенной калибровке имеют размерность массы
       public double FWRL; //Ширина пика на уровне разрешения
       public double ExactRightBound;  //Точн.значение правой границы по уровню разрешения
       public double ExactLeftBound;   //Точн.значение левой границы по уровню разрешения
       public double Resolution;
    } 

    public class PeakDetector{

        const double NoiseRate = 3;
        const double PeakBounds = 0.1; 

        public double NoiseLevel;

        int[] ExtremInd;

        int ExtremCount;



        //добавляет нули на границы заполненных областей 
        void AddNulls(){
            Quanty.MZData[] Added = new Quanty.MZData[Data.Length*2];
            //ищем разрывы 
            int i, Count = 0;
            double Diff = 0.1;
            //первая точка
            if (Data[0].Intensity > 0){
                Added[0].Intensity = 0;
                Added[0].Mass = Data[0].Mass-(Data[1].Mass-Data[0].Mass);
                Count++;
            }
            //все средние точки 
            for (i = 0; i < Data.Length-1 ; i++ ){
                Added[Count] = Data[i];
                Count++;
                //если разрыв 
                if (Data[i+1].Mass - Data[i].Mass > Diff*1.5){
                    //добавляенми две точки - после и до разрыва 
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
            //последняя точка 
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

	        //Ищем все эктремумы

	        int i,j,Count=0,DCount,DefeatedCount;
	        double Sum = 0,Avg;
	        double nMax=0;

            Quanty.MZData[] Extrems = new Quanty.MZData[Data.Length]; 
            ExtremInd = new int[Data.Length]; 
	        //если объем данных вырос - увеличиваем объем памяти

           //При расчете порога используем данные всего масс-спектра, независимо от диапазона
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
		        //Удаляем шумовые экстремумы
		        //шумовым экстремумом считаетсям экстремум чей уровень не превышает
		        //средний уровень шумового экстремума болеее чем в три раза
		        //сначала определяем средний уровень всех экстремумов
		        //потом удаляем из выборки все нешумовые
		        //потом повторяем процесс для всех оставшихся
		        //делаем проходы пока не удалим последний нештатный экстремум
		        do {
			        //Определяем средний уровень
                    //!!Что это!!
			        Avg = (Sum/DCount)*NoiseRate;
			        //выкидываем пики не превышающие этот уровень
			        DefeatedCount = 0;
			        for ( j=0 ; j<Count ; j++ ) {
				        if (Extrems[j].Intensity > Avg){
					        Extrems[j].Intensity = 0.0;
					        DefeatedCount++;
				        }
			        }
			        DCount=0;
			        //Определяем новую среднюю величину экстремума
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

           //сдвигаем в начало массива экстремумы, которые выше порога и внутри диапазона 

           int[] ExtremBuf = new int[Count - DCount];
           Count=0;
           for(i=0;i<ExtremCount;i++){
	           if(Data[ExtremInd[i]].Intensity > NoiseLevel)  {
		           ExtremInd[Count]=ExtremInd[i]; Count++;
	           }
           }
           ExtremCount=Count;

           //По окончании у имеется массив ExtremInd[i], первые ExtremCount элементов которого
           //указывают на экстремумы пиков, которые выше порога и лежат внутри исследуемого диапазона
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
       		
	        //Если задано определение уровня шума - определяем его
	        ExtremCount=0;
            AddNulls();
	        NoiseLevelDetect(true);


	        peakinside[] Peaks = new peakinside[ExtremCount];

	        cnt=0; WorkCount = 0; LastMinRight=1;

	        while(cnt<ExtremCount)
	        {
		        nResult=0;
    						
		        Peaks[WorkCount].Extrem =ExtremInd[cnt]; 
    						
		        //Интенсивность, на которой проверяется разрешение пика
		        ResLevel=PeakBounds*Data[ExtremInd[cnt]].Intensity; 
		        //Идем влево - ищем первый минимум, разрешающий пик
		        i=ExtremInd[cnt]-1;
		        while(true)
		        {
                    //уперлись в границу обработанной части спектра
    				if(i<=LastMinRight){
                        nResult=1; 
                        break;
                    } 

                    //Найден разрешающий минимум
			        if(Data[i].Intensity<=ResLevel){
				        nResult=0; break;
			        }
			        i--;
		        }
    			
		        if (nResult == 0) //Скан завершился нахождением разрешающего минимума слева
		        {
			        Peaks[WorkCount].LeftBound=i+1;
    				
			        //Идем вправо - ищем первый минимум, разрешающий пик
                    //мы добавляем нули по краям значащих областей так что за границы мы не выйдем
			        i=ExtremInd[cnt]+1;
			        while(true)
			        {
                        //По ходу скана вместо минимума обнаружен более интенсивный максимум
				        if(Data[i].Intensity >Data[Peaks[WorkCount].Extrem].Intensity){
                            nResult=1; 
                            break;
                        } 

                        //Найден разрешающий минимум
				        if(Data[i].Intensity<=ResLevel){
					        nResult=0; 
                            break;
				        }
				        i++;
			        }

			        if(nResult==0) //Скан завершился нахождением разрешающего минимума справа
			        {
				        Peaks[WorkCount].RightBound=i-1;

				        Peaks[WorkCount].Height =Data[Peaks[WorkCount].Extrem].Intensity;
    									
				        //Уточняем положение левой границы пика
				        Peaks[WorkCount].ExactLeftBound = Peaks[WorkCount].LeftBound;

				        if((Data[Peaks[WorkCount].LeftBound].Intensity-Data[Peaks[WorkCount].LeftBound-1].Intensity)!=0)
    				        Peaks[WorkCount].ExactLeftBound-=
                                (Data[Peaks[WorkCount].LeftBound].Intensity-ResLevel)/
                                (Data[Peaks[WorkCount].LeftBound].Intensity-(double)Data[Peaks[WorkCount].LeftBound-1].Intensity);
    					
				        //Уточняем положение правой границы пика
				        Peaks[WorkCount].ExactRightBound = Peaks[WorkCount].RightBound;
				        if((Data[Peaks[WorkCount].RightBound+1].Intensity-Data[Peaks[WorkCount].RightBound].Intensity)!=0)
				            Peaks[WorkCount].ExactRightBound+=
                                (Data[Peaks[WorkCount].RightBound].Intensity-ResLevel)/
                                (Data[Peaks[WorkCount].RightBound].Intensity-(double)Data[Peaks[WorkCount].RightBound+1].Intensity);
    					
				        //Передвигаем ограничитель обработанной части спектра
				        //на правую границу последнего найденного пика
				        LastMinRight=Peaks[WorkCount].RightBound; 

				        WorkCount++; //Передвигаем курсор на следующий пик только если
							        //последний найденный пик нас устраивает
			        }
		        }
		        switch(nResult)
		        {
		            case(2):  cnt=ExtremCount; break; //дошли до конца диапазона - инициируем завершение цикла
			        default:  cnt++;    break;
		        }
	        }
            Count = WorkCount;
       
	        //Интегрирование, поиск центроидов
	        //в данный момент границы пиков проложены по фактической точке уровня обрезки пиков
	        //центроид оценивается по центру масс фигуры выше уровня обрезки пиков
	        //оценка пика включает и нижний прямоугольник (до уровня обрезки) тоже
    		
	        double TruncLevel; //уровень обрезки пика
	        double fPeakSum=0.0;
	        double fMoment=0.0;
	        double fTemp, fDelta, fMassDelta;
	        for (i = 0 ; i < Count; i++ )
	        {
                //для рассчета старших моментов 
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

                //рассчет старших моментов 
                for ( j = 0 ; j < Index ; j++){
                    Peaks[i].SKO += (XJ[j]/SJ[j]-Peaks[i].Center)*(XJ[j]/SJ[j]-Peaks[i].Center)*SJ[j];
//                    Peaks[i].Skew += (XJ[j]/SJ[j]-Peaks[i].Center)*(XJ[j]/SJ[j]-Peaks[i].Center)*(XJ[j]/SJ[j]-Peaks[i].Center)*SJ[j];
//                    Peaks[i].Kurt += (XJ[j]/SJ[j]-Peaks[i].Center)*(XJ[j]/SJ[j]-Peaks[i].Center)*(XJ[j]/SJ[j]-Peaks[i].Center)*(XJ[j]/SJ[j]-Peaks[i].Center)*SJ[j];
                }
                Peaks[i].SKO = Math.Sqrt(Peaks[i].SKO/fPeakSum);

//                Peaks[i].Skew = Peaks[i].Skew / fPeakSum; 

//                Peaks[i].Kurt = Peaks[i].Kurt / fPeakSum; 


	        }

	        //отсеиваем пики с нулевыми центроидами или интенсивностью
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

