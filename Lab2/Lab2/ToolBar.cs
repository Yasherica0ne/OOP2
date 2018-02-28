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
    }
}
