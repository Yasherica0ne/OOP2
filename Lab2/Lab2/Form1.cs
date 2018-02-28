using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            if (!XmlSerializeWrapper.isEmpty())
            {
                accounts.AddRange(XmlSerializeWrapper.Deserialize<List<Account>>(Account.mainFilePath));
                foreach (Account acc in accounts)
                {
                    listBox1.Items.Add(acc.number + " " + acc.owner.surname + " " + acc.owner.name + " " + acc.owner.midname + " " + acc.owner.passNumber + " " + acc.balance);
                    Account.count++;
                }
            }
        }

        List<Account> accounts = new List<Account>();
        Account account;
        public int[] count;
        public List<string> search;
        public IEnumerable<Account> buffer;
        public string fileName;

        private void button1_Click(object sender, EventArgs e)
        {
            int dpType = 0;
            if (radioButton1.Checked) dpType = 0;
            else if (radioButton2.Checked) dpType = 1;
            else if (radioButton3.Checked) dpType = 2;
            else throw new Exception("Не выбран тип вклада");
            if (textBox1.Text == "") throw new Exception("Поле фамилии пусто");
            else if (textBox2.Text == "") throw new Exception("Поле имени пусто");
            else if (textBox3.Text == "") throw new Exception("Поле отчества пусто");
            else if (textBox5.Text == "") throw new Exception("Поле номера паспорта пусто");
            else if (monthCalendar1.SelectionRange.Start.Date == monthCalendar1.TodayDate)
                throw new Exception("Дата рождения не выбрана");
            else if ((new DateTime().AddTicks(DateTime.Now.Ticks - monthCalendar1.SelectionRange.Start.Date.Ticks)).Year < 19)
                throw new Exception("Вам должно быть больше восемнадцати лет");
            else if (comboBox1.Text == "") throw new Exception("Поле с суммой вклада пусто");
            account = new Account(textBox2.Text, textBox3.Text, textBox1.Text, long.Parse(textBox5.Text), monthCalendar1.SelectionRange.Start.Date, dpType, float.Parse(comboBox1.Text), checkBox1.Checked, checkBox2.Checked, float.Parse(comboBox1.Text), Account.OperationType.deposit);
            //account = new Account();
            Account.count++;
            accounts.Add(account);
            //listBox1.Items.Add("0 Makarov Victor Alekseevich");
            listBox1.Items.Add(account.number + " " + textBox1.Text + " " + textBox2.Text + " " + textBox3.Text + " " + account.owner.passNumber + " " + account.balance);
            //var convertedjson = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            XmlSerializeWrapper.Serialize(accounts, Account.mainFilePath);
            //accounts = XmlSerializeWrapper.Deserialize<List<Account>>("users.xml");
        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            count = new int[4];
            search = new List<string>();
            count[0] = 0;
            search.Add(listBox1.Text.Split(' ').First());
            textBox6.Text = Account.Search(search, count).First().ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == "") throw new Exception("Поле номера пасспорта пусто");
            textBox6.Text = Account.AccountHistory(long.Parse(textBox4.Text));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var controls = groupBox2.Controls;
            int sortType = 0;
            foreach (RadioButton rb in controls)
            {
                sortType++;
                if (rb.Checked) break;
            }
            if (sortType == 0) throw new Exception("Не выбран тип сортировки");
            else buffer = Account.Sort(sortType);
            textBox6.Text = Account.AccountsToString(buffer);
        }

        public void Save(string name)
        {
            if (buffer == null) throw new Exception("Буфер пуст");
            if (buffer.Count() > 0)
            {
                List<Account> buf = new List<Account>();
                buf.AddRange(buffer);
                XmlSerializeWrapper.Serialize<List<Account>>(buf, name + ".xml");
                MessageBox.Show("Результат сохранён", "window");
            }
            else throw new Exception("Буфер пуст");

        }
        private void button6_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2(this);
            f2.Show();
        }
        public void changeResult(string result)
        {
            textBox6.Text = result;
        }

        private void поискToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Search search = new Search(this);
            search.Show();
        }

        private void оПрограммеToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Версия: 0.2\nАвтор: Макаров Виктор Алексеевич", "Lab3");
        }

        public void Locate(object sender, EventArgs e)
        {
            Point point = new Point(this.Location.X + this.Width - 15, this.Location.Y);
            tb.Location = point;
        }
        ToolBar tb = new ToolBar();
        private void инструментыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tb = new ToolBar(this);
            Point point = new Point(this.Location.X + this.Width - 15, this.Location.Y);
            tb.Location = point;
            tb.LocationChanged += new System.EventHandler(Locate);
            tb.Show();
        }

    }
}
