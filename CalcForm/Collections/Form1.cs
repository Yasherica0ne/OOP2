using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Collections
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Collect collection;
        private void button6_Click(object sender, EventArgs e)
        {
            collection = new Collect(int.Parse(textBox1.Text));
            textBox2.Text = collection.Buffering(collection.list);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            IEnumerable<int> result = collection.list.Where(n => n % 2 != 0);
            textBox2.Text = collection.Buffering(result);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox2.Text = collection.list.Max().ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            double average = collection.list.Average();
            IEnumerable<int> result = collection.list.Where(n => n > average);
            textBox2.Text = collection.Buffering(result);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox2.Text = collection.Buffering(collection.list);
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
