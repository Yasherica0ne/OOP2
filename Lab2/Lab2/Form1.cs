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
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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
                tb = new ToolBar(this);
                Point point = new Point(this.Location.X + this.Width - 15, this.Location.Y);
                tb.Location = point;
                tb.LocationChanged += new System.EventHandler(Locate);
                toolStripStatusLabel4.Text = Account.count.ToString();
                //toolStripStatusLabel6.Text = DateTime.Now.Date.ToShortDateString();
                timer = new Timer() { Interval = 1000 };
                timer.Tick += Timer1_Tick;
                timer.Start();
            }
        }

        List<Account> accounts = new List<Account>();
        Account account;
        public int[] count;
        public List<string> search;
        public List<Account> buffer = new List<Account>();
        public string fileName;
        Timer timer;

        private void button1_Click(object sender, EventArgs e)
        {
            int dpType = 0;
            if (radioButton1.Checked) dpType = 0;
            else if (radioButton2.Checked) dpType = 1;
            else if (radioButton3.Checked) dpType = 2;
            else throw new Exception("Не выбран тип вклада");
            if (textBox5.Text == "") throw new Exception("Поле номера паспорта пусто");
            else if (monthCalendar1.SelectionRange.Start.Date == monthCalendar1.TodayDate)
                throw new Exception("Дата рождения не выбрана");
            else if ((new DateTime().AddTicks(DateTime.Now.Ticks - monthCalendar1.SelectionRange.Start.Date.Ticks)).Year < 19)
                throw new Exception("Вам должно быть больше восемнадцати лет");
            else if (comboBox1.Text == "") throw new Exception("Поле с суммой вклада пусто");
            long passNum = -1;
            long.TryParse(textBox5.Text, out passNum);
            float balance = -1;
            float.TryParse(comboBox1.Text, out balance);
            account = new Account(textBox2.Text, textBox3.Text, textBox1.Text, passNum, monthCalendar1.SelectionRange.Start.Date, dpType, balance, checkBox1.Checked, checkBox2.Checked, Account.OperationType.deposit);
            //account = new Account();
            ValidationContext valCont = new ValidationContext(account);
            List<ValidationResult> results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(account, valCont, results, true))
            {
                foreach (var error in results)
                {
                    throw new Exception(error.ErrorMessage);
                }
            }
            valCont = new ValidationContext(account.owner);
            results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(account.owner, valCont, results, true))
            {
                foreach (var error in results)
                {
                    throw new Exception(error.ErrorMessage);
                }
            }
            Account.count++;
            stripStatus(sender);
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
            CheckList();
            if (textBox4.Text == "") throw new Exception("Поле номера пасспорта пусто");
            textBox6.Text = Account.AccountHistory(long.Parse(textBox4.Text));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            stripStatus(sender);
            CheckList();
            var controls = groupBox2.Controls;
            int sortType = 0;
            foreach (RadioButton rb in controls)
            {
                sortType++;
                if (rb.Checked) break;
            }
            buffer.Clear();
            if (sortType == 0) throw new Exception("Не выбран тип сортировки");
            else buffer.AddRange(Account.Sort(sortType));
            textBox6.Text = Account.AccountsToString(buffer);

        }

        public void Save(string name)
        {
            if (buffer.Count == 0) throw new Exception("Буфер пуст");
            else
            { 
                List<Account> buf = new List<Account>();
                buf.AddRange(buffer);
                XmlSerializeWrapper.Serialize<List<Account>>(buf, name + ".xml");
                MessageBox.Show("Результат сохранён", "window");
            }
        }
        public void button6_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2(this);
            f2.Show();
        }

        public void CheckList()
        {
            if (Account.count == 0) throw new Exception("Список пуст");
        }

        private void stripStatus(object sender)
        {
            if (sender is ToolStripMenuItem)
            {
                toolStripStatusLabel1.Text = "Поиск";
                return;
            }

            switch (((Button)sender).Name)
            {
                case "button1":
                    {
                        toolStripStatusLabel1.Text = "Создание вклада";
                        toolStripStatusLabel4.Text = Account.count.ToString();
                        break;
                    }
                case "button5": toolStripStatusLabel1.Text = "Сортировка"; break;
            }
        }
        public void changeResult(string result)
        {
            textBox6.Text = result;
        }
        
        public void ClearFields()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            monthCalendar1.SelectionRange.Start = DateTime.Now;
            comboBox1.Text = "";
            checkBox1.Checked = false;
            checkBox2.Checked = false;
        }

        public void поискToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            stripStatus(sender);
            Search search = new Search(this);
            search.Show();
        }

        private void оПрограммеToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Версия: 0.3\nАвтор: Макаров Виктор Алексеевич", "Lab3");
        }

        public void Locate(object sender, EventArgs e)
        {
            Point point = new Point(this.Location.X + this.Width - 15, this.Location.Y);
            tb.Location = point;
        }
        ToolBar tb = new ToolBar();

        public void ClearList()
        {
            accounts.Clear();
            listBox1.Items.Clear();
            buffer.Clear();
            Account.count = 0;
            toolStripStatusLabel4.Text = "0";
            toolStripStatusLabel2.Text = "Очистка";
            return;
        }

        private void Timer1_Tick(Object sender, EventArgs e)
        {
            toolStripStatusLabel6.Text = DateTime.Now.ToString();
        }

        private void открытьПанельИнструментовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tb.Show();
        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tb.Visible = false;
        }
    }
}
