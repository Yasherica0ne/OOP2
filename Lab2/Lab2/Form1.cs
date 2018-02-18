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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<Account> accounts = new List<Account>();
        Account account;
        DateTime date = DateTime.Now;
        bool isInitDate = false;
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            date = e.Start.Date;
            isInitDate = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int dpType = 0;
                if (radioButton1.AutoCheck) dpType = 1;
                else if (radioButton2.AutoCheck) dpType = 2;
                else if (radioButton3.AutoCheck) dpType = 3;
                else throw new Exception("Не выбран тип вклада");
                if (textBox1.Text == "") throw new Exception("Поле фамилии пусто");
                else if (textBox2.Text == "") throw new Exception("Поле имени пусто");
                else if (textBox3.Text == "") throw new Exception("Поле отчества пусто");
                else if (textBox5.Text == "") throw new Exception("Поле номера паспорта пусто");
                else if (!isInitDate) throw new Exception("Дата рождения не выбрана");
                else if(comboBox1.Text == "") throw new Exception("Поле с суммой вклада пусто");
                account = new Account(textBox2.Text, textBox3.Text, textBox1.Text, long.Parse(textBox5.Text), date, dpType, float.Parse(comboBox1.Text), checkBox1.AutoCheck, checkBox2.AutoCheck, float.Parse(comboBox1.Text), OperationType.deposit);
                //account = new Account();
                Account.count++;
                accounts.Add(account);
                //listBox1.Items.Add("0 Makarov Victor Alekseevich");
                listBox1.Items.Add(account.number + " " + textBox1.Text + " " + textBox2.Text + " " + textBox3.Text);
                XmlSerializeWrapper.Serialize(accounts, "accounts.xml");
                isInitDate = false;
                //accounts = XmlSerializeWrapper.Deserialize<List<Account>>("users.xml");
            }
            catch(Exception excp)
            {
                MessageBox.Show(excp.Message);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            label10.Text = XmlSerializeWrapper.ShowInfo(listBox1.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox4.Text == "") throw new Exception("Поле номера пасспорта пусто");
                textBox6.Text = Account.AccountHistory(long.Parse(textBox4.Text));
            }
            catch(Exception excp)
            {
                MessageBox.Show(excp.Message);
            }
        }
    }
}
