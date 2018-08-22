using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Drawing.Drawing2D;

namespace maptest
{
    public partial class MapView : UserControl
    {            
        public static int level, length, total, range, mapX, mapY;
        public static bool mouseEvent = false, testFlag = false,iconFlag=false,locateFlag = false;
        public static char layer='a';       
        public static int locateX = 0,locateY = 0;
        public static List<Image> m_Images = new List<Image>();
        public static List<string> s = new List<string>();
        public static Image[] icon = new Image[5];
        public static List<int> ltox = new List<int>();
        public static List<int> ltoy = new List<int>();
        public static List<string> cate = new List<string>();    
        public MapView()
        {
            InitializeComponent();
            level = 1;
            length = 2;
            total = 4;            
            mapX = 0;
            mapY = 0;
        }
        private void MapView_Load(object sender, EventArgs e)
        {           
            this.MouseWheel += new MouseEventHandler(this.textBox1_MouseWheel);
            WebClient wc = new WebClient();
            m_Images.Clear();
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a0.jpeg?g=3649"))));
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a1.jpeg?g=3649"))));
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a2.jpeg?g=3649"))));
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a3.jpeg?g=3649"))));
            icon[0] = Image.FromFile("C:\\Users\\ColifeTNNB01\\Desktop\\maptest2\\題目\\school.png");
            icon[1] = Image.FromFile("C:\\Users\\ColifeTNNB01\\Desktop\\maptest2\\題目\\mrt.png");
            icon[2] = Image.FromFile("C:\\Users\\ColifeTNNB01\\Desktop\\maptest2\\題目\\parking.png");
            icon[3] = Image.FromFile("C:\\Users\\ColifeTNNB01\\Desktop\\maptest2\\題目\\gas.png");
            icon[4] = Image.FromFile("C:\\Users\\ColifeTNNB01\\Desktop\\maptest2\\題目\\locate.png");
            s.Add("0");
            s.Add("1");
            s.Add("2");
            s.Add("3");
            range = (int)(Form1.Form_Width / 256) + 1;
        }
        private void textBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                mouseEvent = true;
                mapX += (e.X) / 256;
                mapY += (e.Y) / 256;
                mapX *= 2;
                mapY *= 2;
                enlarge2();
                if (Form1.showMRT == true) { MrtLayer.drawLine(); }
                this.Refresh();               
            }
            else
            {
                mouseEvent = true;
                reduce();
                if (Form1.showMRT == true) { MrtLayer.drawLine(); }
                this.Refresh();              
            }            
        }
        private void MapView_Paint(object sender, PaintEventArgs e) //依據陣列裡的圖片依序給位置
        {
            this.SetStyle(ControlStyles.UserPaint, true);//自繪
            this.SetStyle(ControlStyles.DoubleBuffer, true);// 雙緩衝
            this.SetStyle(ControlStyles.ResizeRedraw, true);//調整大小時重繪
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);// 雙緩衝
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);   //透明效果        
            Graphics g = e.Graphics;
            g.Clip = new Region(new Rectangle(0, 0, 1024, 1024));
            int addX = Form1.disx, addY = Form1.disy;   //地圖繪製
            int x = 0 + addX, y = 0 + addY;
            if (Form1.release == 1)
            {
                g.Clear(this.BackColor);
                Form1.release = 0;
            }
            for (int i = 0; i < total; i++)
            {
                if (length < range) range = length;                                       
                if (x < 256 * range+addX)
                {
                    g.DrawImage(m_Images[i], new Point(x , y));
                    x += 256;
                }
                else
                {
                    x = addX;
                    y += 256;
                    i--;
                }
            }  
            range = (int)(Form1.Form_Width / 256) + 1;

            if (Form1.showMRT==true)    //捷運路線
            {
                Pen pen = new Pen(Color.Red, 4); ;
                for (int i = 0; i < MrtLayer.cnt; i++)
                {
                    if (MrtLayer.red[i] == 0) { pen = new Pen(Color.Orange, 4); }
                    else { pen = new Pen(Color.Red, 4); }     
                    g.DrawLine(pen, MrtLayer.drawX[i] - mapX * 256, MrtLayer.drawY[i] - mapY * 256, MrtLayer.drawx[i] - mapX * 256, MrtLayer.drawy[i] - mapY * 256);
                }
            }
            SolidBrush blueBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 255));   //行政區
            if (TownLayer.forFlag==true)
            {
                for (int i = 0; i < TownLayer.poi.Length; i++)
                {
                    TownLayer.poi[i].X = TownLayer.poi[i].X - mapX * 256;
                    TownLayer.poi[i].Y = TownLayer.poi[i].Y - mapY * 256;                               
                }
                TownLayer.forFlag = false;
            }
            if (testFlag==true)
            {
                Point[] newPoint={};                
                int pointCnt = 0,cnt,maxX,maxY,minX,minY;
                
                for (int i = 0; i < TownLayer.arrayNum.Count;i++ )
                {
                    cnt = 0;
                    maxX = 0; maxY = 0; minX = 65535; minY = 65535;
                    Array.Resize(ref newPoint, 0);
                    for (int j = pointCnt; j < TownLayer.arrayNum[i]+pointCnt; j++)
                    {
                        if (cnt >= newPoint.Length) Array.Resize(ref newPoint, cnt + 1);
                        newPoint[cnt].X = TownLayer.poi[j].X  ;
                        newPoint[cnt].Y = TownLayer.poi[j].Y  ;
                        if (newPoint[cnt].X > maxX) maxX = newPoint[cnt].X;
                        if (newPoint[cnt].Y > maxY) maxY = newPoint[cnt].Y;
                        if (newPoint[cnt].X < minX) minX = newPoint[cnt].X;
                        if (newPoint[cnt].Y < minY) minY = newPoint[cnt].Y;
                        cnt++;
                    }                   
                    string town = TownLayer.TownName[i];
                    using (Font font1 = new Font("Arial", level, FontStyle.Bold, GraphicsUnit.Point))
                    {
                        Rectangle rectF1 = new Rectangle((minX+maxX)/2-2*level, (minY+maxY)/2,level*8,level*2);
                        g.DrawString(town, font1, Brushes.White, rectF1);
                    }
                    pointCnt += TownLayer.arrayNum[i];
                    if (newPoint.Length>0)
                    g.FillPolygon(blueBrush,newPoint);
                    g.DrawPolygon(new Pen(Color.Yellow),newPoint);                   
                }                          
            }
            if (iconFlag == true)  //地標
            for (int i=0;i<ltox.Count;i++)
            {               
                switch(cate[i])
                {
                    case "school":
                        g.DrawImage(icon[0], new Point(ltox[i], ltoy[i]));
                        break;
                    case "mrt":
                        g.DrawImage(icon[1], new Point(ltox[i], ltoy[i]));
                        break;
                    case "parking":
                        g.DrawImage(icon[2], new Point(ltox[i], ltoy[i]));
                        break;
                    case "gas":
                        g.DrawImage(icon[3], new Point(ltox[i], ltoy[i]));
                        break;
                }                               
            }
            if (locateFlag == true)
            {
                g.DrawImage(icon[4],new Point(locateX,locateY));
            }
        }
        public static void taiwan()
        {
            total = 16;
            range = 4;
            level = 9;
            length = 512;
            mapX = 425;
            mapY = 221;
            WebClient wc = new WebClient();
            s.Clear();
            m_Images.Clear();
            for (int i = mapY; i < range + mapY; i++)
                for (int j = mapX; j < range + mapX; j++)
                {
                    s.Add(BingMaps.TileXYToQuadKey(j, i, level));
                }
            for (int i = 0; i < range * range; i++)
            {
                m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/"+layer + s[i] + ".jpeg?g=3649"))));
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (iconFlag == false) iconFlag = true;
            else iconFlag = false;
            LandmarkLayer test = new LandmarkLayer();
            test.drawIcon(MapView.mapX * 256 + 1365, MapView.mapX * 256, MapView.mapY * 256 + 1365, MapView.mapY * 256,out ltox,out ltoy ,out cate);
            this.Refresh();
        }
        public static void MRTFocus()
        {
            level = 13;
            length = 8192;
            mapX = 6832;
            mapY = 3565;
            total = 16;
            range = 4;
            WebClient wc = new WebClient();
            s.Clear();
            m_Images.Clear();
            for (int i = mapY; i < range + mapY; i++)
                for (int j = mapX; j < range + mapX; j++)
                {
                    s.Add(BingMaps.TileXYToQuadKey(j, i, level));
                }
            for (int i = 0; i < range * range; i++)
            {
                m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/"+layer + s[i] + ".jpeg?g=3649"))));
            }
        }
        public static void townFocus()
        {
            total = 16;
            range = 4;
            level = 9;
            length = 512;
            mapX = 425;
            mapY = 221;
            WebClient wc = new WebClient();
            s.Clear();
            m_Images.Clear();
            for (int i = mapY; i < range + mapY; i++)
                for (int j = mapX; j < range + mapX; j++)
                {
                    s.Add(BingMaps.TileXYToQuadKey(j, i, level));
                }
            for (int i = 0; i < range * range; i++)
            {
                m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/" + layer + s[i] + ".jpeg?g=3649"))));
            }
        }
        public static void reduce()
        {
           if (level>1)
           {
               level--;
               if (level==1)
               {
                   total = 4;
                   range = 2;
               }
               WebClient wc = new WebClient();
               s.Clear();
               m_Images.Clear();
               mapX = (mapX - 1) / 2;
               mapY = (mapY - 1) / 2;
               for (int i = mapY; i < range + mapY; i++)
                   for (int j = mapX; j < range + mapX; j++)
                   {
                       s.Add(BingMaps.TileXYToQuadKey(j, i, level));
                   }
               for (int i = 0; i < range * range; i++)
               {
                   m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/"+layer + s[i] + ".jpeg?g=3649"))));
               }
               total = range * range;
               length = (1 << level);  
           }
        }
        public static void enlarge2()
        {   level++;
            WebClient wc = new WebClient();
            s.Clear();
            m_Images.Clear();
            if (mouseEvent==false)
            {
                mapX = mapX * 2 + 1;
                mapY = mapY * 2 + 1;
            }
            else
            {
                mouseEvent = false;
            }
            for (int i=mapY;i<range+mapY;i++)
                for (int j=mapX;j<range+mapX;j++)
                {
                    s.Add(BingMaps.TileXYToQuadKey(j, i, level));
                }           
            for (int i=0;i<range*range;i++)
            {
                m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/"+layer + s[i] + ".jpeg?g=3649"))));
            }
            total = range * range;
            length = (1<<level);              
        }
        public static void enlarge()   //放大地圖並把圖片依序放進陣列
        {
            WebClient wc = new WebClient();
            List<Image> temp_Images = new List<Image>();
            List<string>temp_str=new List<string>();
            bool isOddLine = true;
            s.Clear();
            s.Add("0");
            s.Add("1");
            s.Add("2");
            s.Add("3");            
            for (int i = 0; i < total;i++ )
            {
               if (isOddLine==true)
               {
                   temp_str.Add(s[i] + "0");
                   temp_str.Add(s[i] + "1");
                   if ((i + 1) % length == 0)
                   {
                       isOddLine = false;
                       i = i  - length;
                   }
               }
               else
               {
                   temp_str.Add(s[i] + "2");
                   temp_str.Add(s[i] + "3");
                   if ((i + 1) % length == 0)
                   {
                       isOddLine = true;
                   }
               }
            }
            level++;
            length = length * 2;
            total = length * length;
            for (int i = 0; i < total;i++ )
            temp_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a" + temp_str[i] + ".jpeg?g=3649"))));            
            m_Images = temp_Images;
            s = temp_str;
        }       
        public static void loginMap(int Level, int Range, int Length, int MapX, int MapY, int Total,bool mrt,bool district,bool icon,char layer)
        {
            total = Total;
            range = Range;
            level = Level;
            length = Length;
            mapX = MapX;
            mapY = MapY;
            Form1.showMRT = mrt;
            testFlag = district;
            iconFlag = icon;
            MapView.layer = layer;
            WebClient wc = new WebClient();
            s.Clear();
            m_Images.Clear();
            int cnt = 0;
            for (int i = mapY; i < range + mapY; i++)
                for (int j = mapX; j < range + mapX; j++)
                {
                    s.Add(BingMaps.TileXYToQuadKey(j, i, level));
                    MapView.m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/" + MapView.layer + MapView.s[cnt] + ".jpeg?g=3649"))));
                    cnt++;
                }            
        }       
        public static void landmarkFocus(int returnx,int returny)
        {   
            WebClient wc = new WebClient();
            s.Clear();
            m_Images.Clear();
            mapY = returny/256-1;
            mapX = returnx/256-2;
            level=17;
            range = 4;
            length = 4;
            for (int i = mapY; i < range + mapY; i++)
                for (int j = mapX; j < range + mapX; j++)
                {
                    s.Add(BingMaps.TileXYToQuadKey(j, i, level));
                }
            for (int i = 0; i < range * range; i++)
            {
                m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/" + layer + s[i] + ".jpeg?g=3649"))));
            }
            total = range * range;
            locateFlag = true;
            locateX = returnx-MapView.mapX * 256-7;
            locateY = returny-MapView.mapY * 256-63;
        }
        public static void partial(int x,int y)
        {
            level++;
            WebClient wc = new WebClient();
            s.Clear();
            m_Images.Clear();
            if (mouseEvent == false)
            {
                mapX = (x +mapX*256)/256*2-1;
                mapY = (y +mapY*256)/256*2;
            }
            else
            {
                mouseEvent = false;
            }
            for (int i = mapY; i < range + mapY; i++)
                for (int j = mapX; j < range + mapX; j++)
                {
                    s.Add(BingMaps.TileXYToQuadKey(j, i, level));
                }
            for (int i = 0; i < range * range; i++)
            {
                m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/" + layer + s[i] + ".jpeg?g=3649"))));
            }
            total = range * range;
            length = (1 << level);     
        }
        public static void allView()
        {
            total = 16;
            range = 4;
            level = 2;
            length = 4;
            mapX = 0;
            mapY = 0;
            WebClient wc = new WebClient();
            s.Clear();
            m_Images.Clear();
            for (int i = mapY; i < range + mapY; i++)
                for (int j = mapX; j < range + mapX; j++)
                {
                    s.Add(BingMaps.TileXYToQuadKey(j, i, level));
                }
            for (int i = 0; i < range * range; i++)
            {
                m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/" + layer + s[i] + ".jpeg?g=3649"))));
            }
        }
        public static void testlayer()
        {
            WebClient wc = new WebClient();
            MapView.m_Images.Clear();
            if (MapView.level == 1) MapView.range = 2;
            for (int i = 0; i < MapView.range * MapView.range; i++)
            {
                MapView.m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/" + MapView.layer + MapView.s[i] + ".jpeg?g=3649"))));
            }
        }        
        private void test()
        {
            WebClient wc = new WebClient();
            m_Images.Clear();
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a00.jpeg?g=3649"))));
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a01.jpeg?g=3649"))));
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a10.jpeg?g=3649"))));
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a11.jpeg?g=3649"))));
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a02.jpeg?g=3649"))));
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a03.jpeg?g=3649"))));
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a12.jpeg?g=3649"))));
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a13.jpeg?g=3649"))));
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a20.jpeg?g=3649"))));
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a21.jpeg?g=3649"))));
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a30.jpeg?g=3649"))));
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a31.jpeg?g=3649"))));
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a22.jpeg?g=3649"))));
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a23.jpeg?g=3649"))));
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a32.jpeg?g=3649"))));
            m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a33.jpeg?g=3649"))));
           
        }
    }
}
