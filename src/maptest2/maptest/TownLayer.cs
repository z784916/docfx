using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace maptest
{
    class TownLayer
    {
        public static Point[] poi = {};
        public static bool forFlag = false;
        public static List<string>TownName=new List<string>();
        public static List<int>arrayNum=new List<int>();
        private static void readFile(string filePath, out List<string> txt)
        {
            StreamReader sr = new StreamReader(filePath, Encoding.Default);
            string line = sr.ReadLine();
            List<string> str = new List<string>();
            str.Add(line);
            while ((line = sr.ReadLine()) != null)
            {
                str.Add(line);
            }
            txt = str;
        }
        private static int TownPixelX, TownPixelY;
        public static void drawPolygon()
        {
            int cnt = -1,arrayCnt = 0;
            string tmp = "";  
            Array.Resize(ref poi, 0);           
            List<string> array = new List<string>();
            List<string> array2 = new List<string>();           
            readFile("C:\\Users\\ColifeTNNB01\\Desktop\\maptest2\\題目\\Khsc_town.geo", out array);
            readFile("C:\\Users\\ColifeTNNB01\\Desktop\\maptest2\\題目\\Khsc_town.csv", out array2);
            arrayNum.Clear();
            for (int i=0;i<42;i++)
            {
                string[] words = array2[i].Split(',');
                TownName.Add(words[4]);
            }
            for (int i =0 ; i <66; i++)
            {
                string[] words = array[i].Split(',');                             
                double[] intWords = new double[words.Length];
                for (int k = 0; k < words.Length; k++)
                {
                    intWords[k] = Convert.ToDouble(words[k]);
                }
                for (int j = 3; j + 1< Convert.ToInt32(words[2]) * 2 + 3; j += 2)
                {
                    BingMaps.LatLongToPixelXY(intWords[j + 1], intWords[j], MapView.level, out TownPixelX, out TownPixelY);
                    cnt++;
                    Array.Resize(ref poi, poi.Length + 1);
                    poi[cnt] = new Point(TownPixelX , TownPixelY);                                                            
                }                
                if (words[1] != tmp)
                {
                    tmp = words[1];
                    if (i != 0) arrayNum.Add(arrayCnt);                                            
                    arrayCnt = 0;
                }
                arrayCnt += Convert.ToInt32(intWords[2]);
                if (i == 65) arrayNum.Add(arrayCnt);                                                                            
            }
            forFlag = true;
        }
    }
}
