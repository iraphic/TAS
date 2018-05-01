using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TAS
{
    public partial class viewCode : Form
    {
        public viewCode()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(@"D:\Docs\SKRIPSIku\TAS\TAS\Resources\FullCode.txt"))
            {
                String line = sr.ReadToEnd();
                textBox2.Text = line;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(@"D:\Docs\SKRIPSIku\TAS\TAS\Resources\HalfCode.txt"))
            {
                String line = sr.ReadToEnd();
                textBox1.Text = line;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.WordWrap = true;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.WordWrap = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            viewFlow tampil4 = new viewFlow();
            tampil4.Show();
        }
    }
}
