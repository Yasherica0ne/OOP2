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
    public partial class ToolBar : Form
    {
        public ToolBar()
        {
            InitializeComponent();
        }
        public ToolBar(Form1 fm)
        {
            InitializeComponent();
            this.Owner = fm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ((Form1)this.Owner).поискToolStripMenuItem1_Click(sender, e);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ((Form1)this.Owner).ClearList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ((Form1)this.Owner).button6_Click(sender, e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ((Form1)this.Owner).ClearFields();
        }
    }
}
