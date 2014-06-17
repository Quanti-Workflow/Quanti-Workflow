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
namespace Quanty {

    class Utils{

        static public double GetStringValue(string Str){
            try{
	            return Convert.ToDouble(Str);
            }catch{
                return 0.0;
            }
        }

        static public string IPIListtoString(List<string> L){
            string Res = "";
            if (L.Count == 0) return Res;
            for (int i = 0 ; i < L.Count ; i++){
                Res = Res + L[i] + ";";
            }
            return Res.Substring(0,Res.Length-1);
        }

        static public List<string> IPIStringtoList(String str){
            str.Split(new char[] { ';' });
            List<String> Res = new List<string>();
            Res.AddRange(str.Split(new char[] { ';' }));
            return Res;
        }

    }
}