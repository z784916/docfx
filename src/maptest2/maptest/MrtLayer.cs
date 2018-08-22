using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace maptest
{
    public class MrtLayer
    {
        public static int cnt=0;
        public static List<int> red = new List<int>();
        public static List<int> drawX = new List<int>();
        public static List<int> drawY = new List<int>();
        public static List<int> drawx = new List<int>();
        public static List<int> drawy = new List<int>();
     
        private static void readFile(string filePath,out List<string>txt)
        {
            StreamReader sr = new StreamReader(filePath, Encoding.Default);
            string line = sr.ReadLine();
            List<string>str=new List<string>();
            str.Add (line);
            while((line=sr.ReadLine())!=null)
            {
                str.Add(line);
            }
            txt = str;
        }
        public static void drawLine()
        {   
            List<string>array=new List<string>();
            List<string> array2 = new List<string>();
            readFile("C:\\Users\\ColifeTNNB01\\Desktop\\maptest2\\題目\\Khsc_mrt.geo", out array);
            readFile("C:\\Users\\ColifeTNNB01\\Desktop\\maptest2\\題目\\Khsc_mrt.csv", out array2);            
            for (int i = 0; i < 107; i++)
            {
                string[] words = array[i].Split(',');
                string[] color = array2[i].Split(',');
                double[] intWords = new double[words.Length];                
                for (int k = 0; k < words.Length; k++)
                {
                    intWords[k] = double.Parse(words[k]);
                }
                for (int j = 2; j+3 < Convert.ToInt32(words[1])*2 + 2; j += 2)
                {
                    BingMaps.LatLongToPixelXY(intWords[j + 1], intWords[j], MapView.level, out Form1.pixelX, out Form1.pixelY);                    
                    BingMaps.LatLongToPixelXY(intWords[j + 3], intWords[j + 2], MapView.level, out Form1.pixelx, out Form1.pixely);                    
                    drawX.Add(Form1.pixelX);
                    drawY.Add(Form1.pixelY);
                    drawx.Add(Form1.pixelx);
                    drawy.Add(Form1.pixely);
                    if (color[3] == "紅線") { red.Add(1); }
                    else { red.Add(0); }
                    cnt++;
                }
            }
        }
    }
}
