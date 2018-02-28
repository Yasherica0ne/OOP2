using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab2
{
    public partial class Form2 : Form
    {
        public Form2(Form1 fm)
        {
            InitializeComponent();
            this.Owner = fm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 fm1 = (Form1)this.Owner;
            fm1.Save(textBox1.Text);
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
