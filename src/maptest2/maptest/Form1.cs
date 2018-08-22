using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Drawing.Printing;
using System.Threading;
using System.Xml;

namespace maptest
{
    public partial class Form1 : Form
    {
        public static int px, py, Form_Width;
        public static int pixelX, pixelY, pixelx, pixely;
        public static int disx = 0, disy = 0,release=0;
        public static double lalitude = 0, longitude = 0;
        public static string UserAccount;
        public static bool showMRT = false;
        public static bool paintFlag = false;
        public static bool selectionFlag = false;
        private int returnx, returny,middlex,middley;
        private Rectangle mouseRect = Rectangle.Empty;  
        
        ToolTip toolTip1 = new ToolTip();
        PrintPreviewDialog PPD = new PrintPreviewDialog();                 
        PrintDocument PDT = new PrintDocument();
        LandmarkLayer test = new LandmarkLayer();
        int Xaxis, Yaxis;        
        bool isDragging = false;
		/// <summary>
        /// 建構子
        /// </summary>

        public Form1()
        {
            InitializeComponent();            
            Form_Width = this.Width;
            treeView1.ExpandAll();
            Xaxis=0;  
            Yaxis=0;
            PDT.PrintPage += new PrintPageEventHandler(this.PDT_PrintPage);
            PPD.FormClosing += new FormClosingEventHandler(this.PPD_FormClosing);
            textBox1.ForeColor = Color.LightGray;
            textBox1.Text = "請輸入地點";
            textBox1.SelectionStart = 0;
            textBox1.SelectionLength = 0;
            textBox1.Select();
        }        
        private void richTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            px = e.X; // 記住滑鼠點下時相對於元件的 (x,y) 坐標。
            py = e.Y;
            isDragging = true;
            Xaxis = e.X;
            Yaxis = e.Y;
            if (selectionFlag==true)
            DrawStart(e.Location);
        }
        private void richTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (isDragging)
            {          
                if (selectionFlag==true)
                {
                    ResizeToRectangle(e.Location);
                }
                if (MapView.level< 2 && selectionFlag == false)
                {
                    mapView1.Left += (e.X - px);
                    mapView1.Top += (e.Y - py);
                    disx += (e.X - px);
                    disy += (e.Y - py);                
                }                
                if (MapView.level>100)
                {
                    if ((e.X - Xaxis) <= -5)
                    {
                       // Xaxis = 1;
                        WebClient wc = new WebClient();
                        MapView.m_Images.Clear();
                        MapView.s.Clear();
                        int cnt = 0;
                        for (int i = MapView.mapY; i < MapView.range + MapView.mapY; i++)
                            for (int j = MapView.mapX; j < MapView.range + MapView.mapX; j++)
                            {
                                MapView.s.Add(BingMaps.TileXYToQuadKey(j-2 , i , MapView.level));
                                MapView.m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/a" + MapView.s[cnt] + ".jpeg?g=3649"))));
                                cnt++;
                            }
                        //MessageBox.Show((e.X - Xaxis)+"\n"+e.X+"\n"+Xaxis);                       
                    }                                                           
                }
                px = e.X;
                py = e.Y;                            
                release = 1;
                this.Refresh();
            }
            toolTip1.ToolTipIcon = ToolTipIcon.Info;
            toolTip1.ForeColor = Color.Blue;
            toolTip1.BackColor = Color.Gray;            
            for (int i = 0; i < MapView.ltox.Count;i++ )
            {
                if (e.X >= MapView.ltox[i] - 20 && e.X <= MapView.ltox[i] + 20 && e.Y <= MapView.ltoy[i] + 20 && e.Y >= MapView.ltoy[i] - 20 && MapView.iconFlag==true)
                {
                    conection con = new conection();
                    string s="",tile="";
                    con.returnInfo(MapView.ltox[i], MapView.ltoy[i], out s,out tile);
                    toolTip1.ToolTipTitle = tile;
                    //Thread.Sleep(500);
                    toolTip1.Show(s, this, new Point(MapView.ltox[i], MapView.ltoy[i]));
                    break;
                }
                else
                    toolTip1.RemoveAll();
            }

                BingMaps.PixelXYToLatLong(e.X + MapView.mapX * 256, e.Y + MapView.mapY * 256, MapView.level, out lalitude, out longitude);
                this.Text = "lalitude: " + lalitude + "     longitude: " + longitude + "  X: " + e.X  + " Y: " + e.Y +" MapX: "
                +MapView.mapX+" MapY: "+MapView.mapY;
        }
        private void richTextBox_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;           
            Xaxis = (Xaxis - e.X) / 256;
            Yaxis = (Yaxis - e.Y) / 256;
            if (MapView.level >= 2 && selectionFlag==false)
            {
                WebClient wc = new WebClient();
                MapView.m_Images.Clear();
                MapView.s.Clear();
                int cnt = 0;
                for (int i = MapView.mapY; i < MapView.range + MapView.mapY; i++)
                    for (int j = MapView.mapX; j < MapView.range + MapView.mapX; j++)
                    {
                        MapView.s.Add(BingMaps.TileXYToQuadKey(j + Xaxis, i + Yaxis, MapView.level));
                        MapView.m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/" + MapView.layer + MapView.s[cnt] + ".jpeg?g=3649"))));
                        cnt++;
                    }
                MapView.total = MapView.range * MapView.range;
                MapView.mapX += Xaxis;
                MapView.mapY += Yaxis;   
            }
            if (selectionFlag == true)
            {
                middlex = e.X - mouseRect.Width / 2;
                middley = e.Y - mouseRect.Height / 2;
                //MessageBox.Show(middlex + " " + middley);
                
                MapView.partial(middlex,middley);
                Cursor.Clip = Rectangle.Empty;
                DrawRectangle();
                mouseRect = Rectangle.Empty;
                selectionFlag = false;              
                mapView1.Cursor = Cursors.Default;
            }
            if (MapView.testFlag == true) { TownLayer.drawPolygon(); }
            if (MapView.iconFlag == true) { test.drawIcon(MapView.mapX * 256 + 1024, MapView.mapX * 256, MapView.mapY * 256 + 1024, MapView.mapY * 256, out MapView.ltox, out MapView.ltoy, out MapView.cate); }
            if (MapView.locateFlag == true) { locateXY();}
            mapView1.Invalidate();
        }
        private void button1_Click(object sender, EventArgs e) //放大
        { 
            MapView.enlarge2();
            if (showMRT == true) { MrtLayer.drawLine();}
            if (MapView.testFlag == true) { TownLayer.drawPolygon(); }
            if (MapView.iconFlag == true) { LandmarkLayer land = new LandmarkLayer(); land.drawIcon(MapView.mapX * 256 + 1365, MapView.mapX * 256, MapView.mapY * 256 + 1365, MapView.mapY * 256,out MapView.ltox,out MapView.ltoy ,out MapView.cate);}
            this.Text = "X: " + MapView.mapX + " " + " Y: "+ MapView.mapY+ " lavel: " + MapView.level;
            if (MapView.iconFlag == true) { test.drawIcon(MapView.mapX * 256 + 1365, MapView.mapX * 256, MapView.mapY * 256 + 1365, MapView.mapY * 256, out MapView.ltox, out MapView.ltoy, out MapView.cate); }
            if (MapView.locateFlag == true) { locateXY(); }
            mapView1.Invalidate();
        }
        private void button2_Click(object sender, EventArgs e) // 縮小
        {            
            MapView.reduce();
            if (showMRT == true) { MrtLayer.drawLine();}
            if (MapView.testFlag == true) { TownLayer.drawPolygon(); }
            this.Text = "X: " + MapView.mapX + " " + " Y: " + MapView.mapY + " leval: " + MapView.level;
            if (MapView.iconFlag == true) { test.drawIcon(MapView.mapX * 256 + 1365, MapView.mapX * 256, MapView.mapY * 256 + 1365, MapView.mapY * 256, out MapView.ltox, out MapView.ltoy, out MapView.cate);}
            if (MapView.locateFlag == true) { locateXY(); }
            mapView1.Invalidate();            
        }
        private void button4_Click(object sender, EventArgs e)//登出
        {
            conection.insertMap(UserAccount,MapView.level,MapView.range,MapView.length,MapView.total,MapView.mapX,MapView.mapY,showMRT,MapView.testFlag,MapView.iconFlag,MapView.layer);
            UserAccount = "";
            MessageBox.Show("登出成功!");
            login1.Show();
        }
        private void button7_Click(object sender, EventArgs e) //框選
        {
            if (selectionFlag == false)
            {
                mapView1.Cursor = Cursors.Cross;
                selectionFlag = true;
            }
            else
            {
                selectionFlag = false;
                mapView1.Cursor = Cursors.Arrow;
            }
        }
        private void button8_Click(object sender, EventArgs e) //全圖
        {
            MapView.allView();
            if (MapView.testFlag == true) { TownLayer.drawPolygon(); }
            if (MapView.iconFlag == true) { LandmarkLayer land = new LandmarkLayer(); land.drawIcon(MapView.mapX * 256 + 1365, MapView.mapX * 256, MapView.mapY * 256 + 1365, MapView.mapY * 256, out MapView.ltox, out MapView.ltoy, out MapView.cate); }
            this.Text = "X: " + MapView.mapX + " " + " Y: " + MapView.mapY + " lavel: " + MapView.level;
            if (MapView.iconFlag == true) { test.drawIcon(MapView.mapX * 256 + 1365, MapView.mapX * 256, MapView.mapY * 256 + 1365, MapView.mapY * 256, out MapView.ltox, out MapView.ltoy, out MapView.cate); }
            this.Refresh();
        }
        private void button3_Click(object sender, EventArgs e) // 列印
        {
            PrintDialog PD = new PrintDialog();
            PD.UseEXDialog = true;
            PD.Document = PDT;
            DialogResult result = PD.ShowDialog();
            if (result == DialogResult.OK)
            {
                PDT.Print();
            }
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (paintFlag==true)
            {
                mapView1.Invalidate();
                if (login.flag[0] == true) treeView1.Nodes[0].Nodes[0].Checked = true;
                if (login.flag[1] == true) treeView1.Nodes[0].Nodes[1].Checked = true;
                if (login.flag[2] == true) treeView1.Nodes[0].Nodes[2].Checked = true;
                if (login.LAY == 'a') treeView1.Nodes[1].Nodes[0].Checked = true;
                if (login.LAY == 'r') treeView1.Nodes[1].Nodes[1].Checked = true;
                if (login.LAY == 'h') treeView1.Nodes[1].Nodes[2].Checked = true;
                paintFlag = false;
            }
        }
        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (treeView1.Nodes[0].Checked == true)
            {
                foreach (TreeNode ChildeNode in e.Node.Nodes)
                {
                    ChildeNode.Checked = true;
                }
            }
            else
            {
                foreach (TreeNode ChildeNode in e.Node.Nodes)
                {
                    ChildeNode.Checked = false;
                }
            }
            if (treeView1.Nodes[0].Nodes[0].Checked==true)
            {
                MapView.MRTFocus();
                if (showMRT == false) { MrtLayer.drawLine(); showMRT = true; }                              
            }
            else { showMRT = false; }
            if (treeView1.Nodes[0].Nodes[1].Checked==true)
            {
                if (MapView.testFlag == false) { TownLayer.drawPolygon(); MapView.testFlag = true; }                             
            }
            else { MapView.testFlag = false; }
            if (treeView1.Nodes[0].Nodes[2].Checked == true)
            {
                if (MapView.iconFlag == false) MapView.iconFlag = true;
                LandmarkLayer test = new LandmarkLayer();
                test.drawIcon(MapView.mapX * 256 + 1024, MapView.mapX * 256, MapView.mapY * 256 + 1024, MapView.mapY * 256, out MapView.ltox, out MapView.ltoy, out MapView.cate);
            }
            else { MapView.iconFlag = false; }

           
            if (e.Node  == treeView1.Nodes[1].Nodes[0])
            {
                if (treeView1.Nodes[1].Nodes[0].Checked == true)
                {
                    MapView.layer = 'a';
                    treeView1.Nodes[1].Nodes[1].Checked = false;
                    treeView1.Nodes[1].Nodes[2].Checked = false;
                }
            }
           if (e.Node == treeView1.Nodes[1].Nodes[1])
            {
                if (treeView1.Nodes[1].Nodes[1].Checked == true)
                {
                    MapView.layer = 'r';
                    treeView1.Nodes[1].Nodes[0].Checked = false;
                    treeView1.Nodes[1].Nodes[2].Checked = false;
                } 
            }
           if (e.Node == treeView1.Nodes[1].Nodes[2])
           {
               if (treeView1.Nodes[1].Nodes[2].Checked == true)
               {
                   MapView.layer = 'h';
                   treeView1.Nodes[1].Nodes[0].Checked = false;
                   treeView1.Nodes[1].Nodes[1].Checked = false;
               }
           }
           WebClient wc = new WebClient();
           MapView.m_Images.Clear();
           if (MapView.level == 1) MapView.range = 2;
           for (int i = 0; i < MapView.range * MapView.range; i++)
           {
               MapView.m_Images.Add(Image.FromStream(new MemoryStream(wc.DownloadData("https://ecn.t1.tiles.virtualearth.net/tiles/" + MapView.layer + MapView.s[i] + ".jpeg?g=3649"))));
           }
           this.Refresh(); 
        }
        private void PDT_PrintPage(object sender, PrintPageEventArgs e)
        {
            this.SetStyle(ControlStyles.UserPaint, true);//自繪
            this.SetStyle(ControlStyles.DoubleBuffer, true);// 雙緩衝
            this.SetStyle(ControlStyles.ResizeRedraw, true);//調整大小時重繪
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);// 雙緩衝
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);   //透明效果        
            //Graphics g = CreateGraphics();
            Graphics g = e.Graphics;
            g.Clip = new Region(new Rectangle(0, 0, 1024, 1024));
            int addX = Form1.disx, addY = Form1.disy;   //地圖繪製
            int x = 0 + addX, y = 0 + addY;
            if (Form1.release == 1)
            {
                g.Clear(this.BackColor);
                Form1.release = 0;
            }
            for (int i = 0; i < MapView.total; i++)
            {
                if (MapView.length < MapView.range) MapView.range = MapView.length;
                if (x < 256 * MapView.range + addX)
                {
                    g.DrawImage(MapView.m_Images[i], new Point(x, y));
                    x += 256;
                }
                else
                {
                    x = addX;
                    y += 256;
                    i--;
                }
            }
            MapView.range = (int)(Form1.Form_Width / 256) + 1;

            if (Form1.showMRT == true)    //捷運路線
            {
                Pen pen = new Pen(Color.Red, 4); ;
                for (int i = 0; i < MrtLayer.cnt; i++)
                {
                    if (MrtLayer.red[i] == 0) { pen = new Pen(Color.Orange, 4); }
                    else { pen = new Pen(Color.Red, 4); }
                    g.DrawLine(pen, MrtLayer.drawX[i] - MapView.mapX * 256, MrtLayer.drawY[i] - MapView.mapY * 256, MrtLayer.drawx[i] - MapView.mapX * 256, MrtLayer.drawy[i] - MapView.mapY * 256);
                }
            }
            SolidBrush blueBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 255));   //行政區
            if (TownLayer.forFlag == true)
            {
                for (int i = 0; i < TownLayer.poi.Length; i++)
                {
                    TownLayer.poi[i].X = TownLayer.poi[i].X - MapView.mapX * 256;
                    TownLayer.poi[i].Y = TownLayer.poi[i].Y - MapView.mapY * 256;
                }
                TownLayer.forFlag = false;
            }
            if (MapView.testFlag == true)
            {
                Point[] newPoint = { };
                int pointCnt = 0, cnt, maxX, maxY, minX, minY;

                for (int i = 0; i < TownLayer.arrayNum.Count; i++)
                {
                    cnt = 0;
                    maxX = 0; maxY = 0; minX = 65535; minY = 65535;
                    Array.Resize(ref newPoint, 0);
                    for (int j = pointCnt; j < TownLayer.arrayNum[i] + pointCnt; j++)
                    {
                        if (cnt >= newPoint.Length) Array.Resize(ref newPoint, cnt + 1);
                        newPoint[cnt].X = TownLayer.poi[j].X;
                        newPoint[cnt].Y = TownLayer.poi[j].Y;
                        if (newPoint[cnt].X > maxX) maxX = newPoint[cnt].X;
                        if (newPoint[cnt].Y > maxY) maxY = newPoint[cnt].Y;
                        if (newPoint[cnt].X < minX) minX = newPoint[cnt].X;
                        if (newPoint[cnt].Y < minY) minY = newPoint[cnt].Y;
                        cnt++;
                    }
                    string town = TownLayer.TownName[i];
                    using (Font font1 = new Font("Arial", MapView.level, FontStyle.Bold, GraphicsUnit.Point))
                    {
                        Rectangle rectF1 = new Rectangle((minX + maxX) / 2 - 2 * MapView.level, (minY + maxY) / 2, MapView.level * 8, MapView.level * 2);
                        g.DrawString(town, font1, Brushes.White, rectF1);
                    }
                    pointCnt += TownLayer.arrayNum[i];
                    if (newPoint.Length > 0)
                        g.FillPolygon(blueBrush, newPoint);
                    g.DrawPolygon(new Pen(Color.Yellow), newPoint);
                }
            }
            if (MapView.iconFlag == true)  //地標
                for (int i = 0; i < MapView.ltox.Count; i++)
                {
                    switch (MapView.cate[i])
                    {
                        case "school":
                            g.DrawImage(MapView.icon[0], new Point(MapView.ltox[i], MapView.ltoy[i]));
                            break;
                        case "mrt":
                            g.DrawImage(MapView.icon[1], new Point(MapView.ltox[i], MapView.ltoy[i]));
                            break;
                        case "parking":
                            g.DrawImage(MapView.icon[2], new Point(MapView.ltox[i], MapView.ltoy[i]));
                            break;
                        case "gas":
                            g.DrawImage(MapView.icon[3], new Point(MapView.ltox[i], MapView.ltoy[i]));
                            break;
                    }
                }
        }        
        private void PPD_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            PPD.Hide();
        }
        private void button5_Click(object sender, EventArgs e) //預覽列印
        {            
            PPD.Document = PDT;
            PPD.Show();
        }
        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textBox1.Text = "";
            textBox1.ForeColor = Color.Black;
        }
        private void login1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            login1.Hide();
        }
        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text=="")
            {
                textBox1.ForeColor = Color.LightGray;
                textBox1.Text = "請輸入地點";
            }
        }
        private void locateXY()
        {
            conection con = new conection();
            con.findLocation(textBox1.Text, out returnx, out returny,false);
            MapView.locateX = returnx - MapView.mapX * 256 - 7;
            MapView.locateY = returny - MapView.mapY * 256 - 63;
        }
        private void search(object sender, EventArgs e)
        {
            string address = textBox1.Text;
            int x,y;
            string requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address));
            XmlDocument doc = new XmlDocument();
            doc.Load(requestUri);
            if (doc.SelectSingleNode("//status").InnerText=="OK")
            {
                XmlNodeList nodes = doc.SelectNodes("//location");
                double lng = Convert.ToDouble(nodes[0].SelectSingleNode("lng").InnerText);
                double lat = Convert.ToDouble(nodes[0].SelectSingleNode("lat").InnerText);
                //string s = string.Format("logitude: {0} latitude: {1}", lng,lat);
                BingMaps.LatLongToPixelXY(lat, lng, 17, out x, out y);
                MapView.landmarkFocus(x, y);                
                MessageBox.Show(Convert.ToString(doc.SelectSingleNode("//formatted_address").InnerText), "地理資訊");
                this.Refresh();
            }
            else
            {
                MessageBox.Show("找不到目標","ERROR");
            }            
        }
        private void DrawStart(Point StartPoint)
        {
            Cursor.Clip = RectangleToScreen(new Rectangle(0, 0, ClientSize.Width, ClientSize.Height));
            mouseRect = new Rectangle(StartPoint.X, StartPoint.Y, 0, 0);
        }
        private void ResizeToRectangle(Point p)
        {
            DrawRectangle();
            mouseRect.Width = p.X - mouseRect.Left;
            mouseRect.Height = p.Y - mouseRect.Top;
            //this.Text = "Width: " + mouseRect.Width + " Heitht: " + mouseRect.Height;
            DrawRectangle();
        }
        private void DrawRectangle()
        {
            Rectangle rect = RectangleToScreen(mouseRect);
            ControlPaint.DrawReversibleFrame(rect, Color.White, FrameStyle.Dashed);
        }  
       

         
    }
}
