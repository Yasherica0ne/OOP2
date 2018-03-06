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
    public partial class Search : Form
    {
        public Search(Form1 fm)
        {
            InitializeComponent();
            this.Owner = fm;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form1 fm1 = (Form1)this.Owner;
            fm1.CheckList();
            var controls = groupBox3.Controls;
            int sortType = 0;
            int index = 0;
            TextBox tb;
            fm1.count = new int[4];
            fm1.search = new List<string>();
            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i] is TextBox)
                {
                    tb = (TextBox)controls[i];
                    if (tb.Text != "")
                    {
                        fm1.count[index++] = sortType;
                        fm1.search.Add(tb.Text);
                    }
                    sortType++;
                }
            }
            if (listBox2.Text != "")
            {
                fm1.count[index] = 3;
                switch (listBox2.Text.Split(' ').First())
                {
                    case "1": fm1.search.Add("0"); break;
                    case "2": fm1.search.Add("1"); break;
                    case "3": fm1.search.Add("2"); break;

                }

            }
            fm1.buffer.AddRange(Account.Search(fm1.search, fm1.count));
            fm1.changeResult(Account.AccountsToString(fm1.buffer));
            listBox2.ClearSelected();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
