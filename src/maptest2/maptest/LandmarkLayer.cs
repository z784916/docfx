using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace maptest
{
    class LandmarkLayer
    {
        private void readFile(string filePath, out List<string> txt)
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
        public void loadLand()
        {
            List<string> array = new List<string>();
            List<string> array2 = new List<string>();
            conection enter = new conection();
            readFile("C:/Users/user/Desktop/2018_工程師須看的書/2.C#BingMapViewer/題目/Khsc_landmark.csv", out array);
            readFile("C:/Users/user/Desktop/2018_工程師須看的書/2.C#BingMapViewer/題目/Khsc_landmark.geo", out array2);
            for (int i=0;i<4061;i++)
            {
                string[] words = array[i].Split(',');
                string[] words2 = array2[i].Split(',');
                string catagory="";
                if (words[3].IndexOf("國小") != -1 || words[3].IndexOf("國中") != -1 || words[3].IndexOf("高中") != -1) catagory = "school";
                if (words[3].IndexOf("加油站") != -1) catagory = "gas";
                if (words[3].IndexOf("捷運站") != -1) catagory = "mrt";
                if (words[3].IndexOf("停車場") != -1) catagory = "parking";
                if (catagory!="")
                enter.insertLand(words[0], words[3], words[5], words[6],words2[1],words2[2],catagory);
            }
        }
        public void  drawIcon(int topx,int btnx,int topy,int btny,out List<int> lati, out List<int> longi, out List<string> cate)
        {  
            double lat,lon,lat2,lon2;
            conection con = new conection();
            BingMaps.PixelXYToLatLong(topx, topy, MapView.level, out lat, out lon);
            BingMaps.PixelXYToLatLong(btnx,btny, MapView.level, out lat2, out lon2);
            con.isExist(lat,lon,lat2,lon2,out lati,out longi,out cate);
        }
    }
}
