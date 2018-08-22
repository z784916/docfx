using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace maptest
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (conection.identify(textBox1.Text,textBox2.Text))
            {
                MessageBox.Show("登入成功");
                textBox1.Enabled = false;
                textBox2.Enabled = false;
            }
            else
            {
                MessageBox.Show("帳號或密碼有誤!!");
            }
        }
    }
}
