using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace maptest
{
    public partial class login : UserControl
    {
        public static bool[] flag = new bool[3];
        public static char LAY;
        Form2 form = new Form2();
        public login()
        {
            InitializeComponent();
            textBox1.Text = "aaa";
            textBox2.Text = "bbb";
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            if (conection.identify(textBox1.Text, textBox2.Text))
            {
                MessageBox.Show("登入成功","登入系統");
                Form1.UserAccount = textBox1.Text;
                int[] query = new int [6];
                char layer = ' ';
                conection.userLogin(out query, out flag,out layer,Form1.UserAccount);
                if (query[0]!=1000)
                {
                    MapView.loginMap(query[0], query[1], query[2], query[3], query[4], query[5], flag[0], flag[1], flag[2],layer);
                }
                if (flag[0] == true) { MrtLayer.drawLine(); Form1.showMRT = true; }
                if (flag[1] == true) { TownLayer.drawPolygon(); MapView.testFlag = true; }
                if (flag[2] == true) { LandmarkLayer test = new LandmarkLayer(); test.drawIcon(MapView.mapX * 256 + 1024, MapView.mapX * 256, MapView.mapY * 256 + 1024, MapView.mapY * 256, out MapView.ltox, out MapView.ltoy, out MapView.cate); MapView.iconFlag = true; }
                LAY = layer;
                textBox1.Text = "";
                textBox2.Text = "";
                this.Visible = false;
                Form1.paintFlag = true;
                this.Parent.Invalidate();
            }
            else
            {
                MessageBox.Show("帳號或密碼有誤!!","輸入錯誤");
            }              
        }

        private void button2_Click(object sender, EventArgs e)
        {           
            form.Location = new Point(100,100);
            form.Visible = true;
        }
    }
}
