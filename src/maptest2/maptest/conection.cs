using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms; 

namespace maptest
{
    class conection
    {
        private static SqlConnection conn = new SqlConnection("Persist Security Info=False;Integrated Security=true;Initial Catalog=TutorialDB;Server=DESKTOP-O8FR33P");
        private void DBconect()
        {
            string Constr = @"Persist Security Info=False;Integrated Security=true;
                     Initial Catalog=TutorialDB;Server=USER-PC";

            // step 3 . 建立SqlConnection
            SqlConnection conn = new SqlConnection(Constr);

            // step 4 . 宣告查詢字串
            string Sqlstr = @"select * from Customers";

            // step 5. 建立SqlDataAdapter
            SqlDataAdapter da = new SqlDataAdapter(Sqlstr, conn);

            // step 6. 建立DataSet來儲存Table
            DataSet ds = new DataSet();

            // step 7. 將DataAdapter查詢之後的結果，填充至DataSet
            da.Fill(ds);

            // step 8 . 用DataGridView1 顯示出來
            //this.dataGridView1.DataSource = ds.Tables[0].DefaultView; 
        }
        private bool clip(double max, double min, double target)
        {
            if (target <= max && target >= min)
            {
                return true;
            }
            return false;
        }
        public void returnInfo(int x, int y, out string content, out string name)
        {
            content = "";
            name = "";
            string sql = "SELECT * from landmark";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            SqlDataReader dd = cmd.ExecuteReader();
            while (dd.Read())
            {
                double lat = Convert.ToDouble(dd["latitude"]);
                double lon = Convert.ToDouble(dd["longitude"]);
                int pixelX, pixelY;
                BingMaps.LatLongToPixelXY(lat, lon, MapView.level, out pixelX, out pixelY);
                pixelX -= MapView.mapX * 256;
                pixelY -= MapView.mapY * 256;
                if (x == pixelX && y == pixelY)
                {
                    name = Convert.ToString(dd["name"]);
                    content = Convert.ToString(dd["location"]) + "\r\n" + Convert.ToString(dd["telephone"]);
                }
            }
            conn.Close();
            cmd.Dispose();
        }
        public static void insert(string account, string password)
        {
            //SQL INSERT 語法
            String strSQL = " INSERT INTO user_info (account,password) VALUES ('" + account + "','" + password + "') ";
            conn.Open();
            SqlCommand sqlcommand = new SqlCommand(strSQL, conn);
            sqlcommand.ExecuteNonQuery();
            conn.Close();
        }
        public static void insertMap(string account, int level, int range, int length, int total, int mapx, int mapy, bool mrt, bool district, bool icon, char layer)
        {
            string sql = "UPDATE user_info SET maplevel=" + level + ",range=" + range + ",length=" + length + ",total=" + total + ",mapX=" + mapx + ",mapY=" + mapy + ",mrt='" + mrt + "',district='" + district + "',icon='" + icon + "',layer='" + layer + "' WHERE account='" + account + "'";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        public static bool isDuplicate(string account)
        {
            string strSQL = "SELECT account from user_info ";
            conn.Open();
            SqlCommand sqlcommand = new SqlCommand(strSQL, conn);
            sqlcommand.ExecuteNonQuery();
            SqlDataReader dd = sqlcommand.ExecuteReader(); //讀取
            string s = "";
            while (dd.Read())
            {
                s = Convert.ToString(dd["account"]).TrimEnd();
                if (account == s)
                {
                    conn.Close();
                    return true;
                }
            }
            conn.Close();
            sqlcommand.Dispose();
            return false;
        }
        public static bool identify(string account, string password)
        {
            string sql = "SELECT * from user_info";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            SqlDataReader dd = cmd.ExecuteReader();
            string s = "";
            while (dd.Read())
            {
                s = Convert.ToString(dd["account"]).TrimEnd();
                if (account == s)
                {
                    s = Convert.ToString(dd["password"]).TrimEnd();
                    if (password == s)
                    {
                        conn.Close();
                        cmd.Dispose();
                        return true;
                    }
                }
            }
            conn.Close();
            cmd.Dispose();
            return false;
        }
        public static void userLogin(out int[] s, out bool[] flag, out char layer, string account)
        {
            string sql = "SELECT * from user_info WHERE account='" + account + "'";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            SqlDataReader dd = cmd.ExecuteReader();
            dd.Read();
            int[] str = new int[9];
            bool[] f = new bool[3];
            if (Convert.ToString(dd["maplevel"]) == "")
            {
                str[0] = 1000; s = str; flag = f; layer = ' ';
                conn.Close();
                cmd.Dispose();
                return;
            }
            str[0] = Convert.ToInt32(dd["maplevel"]);
            str[1] = Convert.ToInt32(dd["range"]);
            str[2] = Convert.ToInt32(dd["length"]);
            str[3] = Convert.ToInt32(dd["mapX"]);
            str[4] = Convert.ToInt32(dd["mapY"]);
            str[5] = Convert.ToInt32(dd["total"]);
            f[0] = Convert.ToBoolean(dd["mrt"]);
            f[1] = Convert.ToBoolean(dd["district"]);
            f[2] = Convert.ToBoolean(dd["icon"]);
            char c = Convert.ToChar(Convert.ToString(dd["layer"]).TrimEnd());
            layer = c;
            conn.Close();
            cmd.Dispose();
            s = str;
            flag = f;
        }
        public void insertLand(string id, string name, string location, string telephone, string longitude, string latitude, string catagory)
        {
            String strSQL = " INSERT INTO landmark (id,name,location,telephone,longitude,latitude,category) VALUES ('" + id + "','" + name + "','" + location + "','" + telephone + "','" + longitude + "','" + latitude + "','" + catagory + "') ";
            conn.Open();
            SqlCommand sqlcommand = new SqlCommand(strSQL, conn);
            sqlcommand.ExecuteNonQuery();
            conn.Close();
        }
        public void isExist(double lat, double lon, double lat2, double lon2, out List<int> lati, out List<int> longi, out List<string> cate)
        {
            string sql = "SELECT latitude,longitude,category from landmark";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            SqlDataReader dd = cmd.ExecuteReader();
            List<int> temp = new List<int>(), temp2 = new List<int>();
            List<string> temp3 = new List<string>();
            while (dd.Read())
            {
                double d = Convert.ToDouble(dd["latitude"]), d2 = Convert.ToDouble(dd["longitude"]);
                if (clip(lat2, lat, d) && clip(lon, lon2, d2))
                {
                    int x, y;
                    BingMaps.LatLongToPixelXY(d, d2, MapView.level, out x, out y);
                    x -= MapView.mapX * 256;
                    y -= MapView.mapY * 256;
                    temp.Add(x);
                    temp2.Add(y);
                    temp3.Add(Convert.ToString(dd["category"]));
                }
            }
            lati = temp;
            longi = temp2;
            cate = temp3;
            conn.Close();
            cmd.Dispose();
        }
        public bool findLocation(string locationName,out int returnx,out int returny,bool isFirst)
        {
            string sql = "SELECT * from landmark";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql,conn);
            cmd.ExecuteNonQuery();
            SqlDataReader dd = cmd.ExecuteReader();
            string s = "";
            returnx=0;
            returny=0;
            while(dd.Read())
            {
                s = Convert.ToString(dd["name"]).TrimEnd();
                if (s.IndexOf(locationName)!=-1)
                {
                    if (isFirst == true) MapView.level = 19;
                    BingMaps.LatLongToPixelXY(Convert.ToDouble(dd["latitude"]), Convert.ToDouble(dd["longitude"]), MapView.level, out returnx, out returny);
                    conn.Close();
                    cmd.Dispose();
                    return true;                    
                }                
            }            
            conn.Close();
            cmd.Dispose();
            return false;
         }
    }
}
