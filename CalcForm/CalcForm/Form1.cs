using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalcForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (Calc.EqualsH(textBox1.Text, textBox2.Text)) textBox3.Text = "true";
                else textBox3.Text = "false";
                label2.Text = ">";
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (Calc.EqualsH(textBox1.Text, textBox2.Text)) textBox3.Text = "false";
                else textBox3.Text = "true";
                label2.Text = "<";
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (Calc.isEquals(textBox1.Text, textBox2.Text)) textBox3.Text = "true";
                else textBox3.Text = "false";
                label2.Text = "==";
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (Calc.isEquals(textBox1.Text, textBox2.Text)) textBox3.Text = "false";
                else textBox3.Text = "true";
                label2.Text = "!=";
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                textBox3.Text = Calc.rightSh(textBox1.Text, textBox2.Text);
                label2.Text = ">>";
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                textBox3.Text = Calc.leftSh(textBox1.Text, textBox2.Text); ;
                label2.Text = "<<";
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
    }
}
