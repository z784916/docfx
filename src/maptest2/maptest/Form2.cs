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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            //textBox2.PasswordChar='*';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "" || textBox2.Text == "" || conection.isDuplicate(textBox1.Text)==true)
                {
                    if (conection.isDuplicate(textBox1.Text)) MessageBox.Show("此帳號已被使用");
                    else MessageBox.Show("帳號或密碼不能為空"); 
                }
                else
                {
                    conection.insert(textBox1.Text, textBox2.Text);
                    MessageBox.Show("註冊成功!!");
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(conection.isDuplicate(textBox1.Text)+" ");
        }
    }
}
