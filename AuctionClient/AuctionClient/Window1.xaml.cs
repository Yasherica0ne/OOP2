using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Threading;
using System.ComponentModel;
using System.Net.Sockets;

namespace AuctionClient
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Entering : Window
    {
        string fileName = @"Account.json";

        private string GetCryptPass(string pass, string login)
        {
            long code = 0;
            StringBuilder buffer = new StringBuilder();
            char[] charsLogin = login.ToCharArray();
            for (int i = 0; i < login.Length; i++)
                code += charsLogin[i];
            char[] charsPass = pass.ToCharArray();
            char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();
            //long intPart;
            for (int i = 0; i < pass.Length; i++)
            {
                double buf = (double)(code + charsPass[i]);
                buf /= (double)chars.Length;
                buffer.Append(chars[(code + (int)charsPass[i]) % chars.Length]);
                buffer.Append(Math.Round(buf, 4));
                buffer.Append('>');
            }
            return buffer.ToString();
        }

        private string GetEncryptPass(string cryptPass, string login)
        {
            long code = 0;
            StringBuilder buffer = new StringBuilder();
            char[] charsLogin = login.ToCharArray();
            for (int i = 0; i < login.Length; i++)
                code += charsLogin[i];
            string[] charsPass = cryptPass.Split('>');
            string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            char[] chars = Chars.ToCharArray();
            for (int i = 0; i < charsPass.Length - 1; i++)
            {
                double buf;
                //buf = long.Parse(charsPass[i].Substring(1));
                //long modulo = (Chars.LastIndexOf(charsPass[i].First())*100)/chars.Length;
                //buf += double.Parse("0," + modulo);
                buf = double.Parse(charsPass[i].Substring(1));
                buf *= chars.Length;
                //buf += 0.49;
                buf = Math.Round(buf);
                buf -= code;
                buffer.Append((char)buf);
            }
            return buffer.ToString();
        }

        public Entering()
        {
            InitializeComponent();
            Enter.Focus();
            ConnectToServer();
            if (System.IO.File.Exists(fileName))
            {
                Account account = Account.Deserialize<Account>(fileName);
                Login.Text = account.Login;
                Password.Password = GetEncryptPass(account.Password, account.Login);
            }
        }

        public void EnterData(string login, string password)
        {
            this.Visibility = Visibility.Visible;
            Login.Text = login;
            Password.Password = password;
        }

        private void NewAccount_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration();
            //this.Visibility = Visibility.Collapsed;
            registration.Owner = this;
            registration.Show();
        }

        private void ConnectToServer()
        {
            ClientAuct.UserName = Login.Text;
            ClientAuct.Client = new TcpClient();
            try
            {
                ClientAuct.Client.Connect(ClientAuct.Host, ClientAuct.Port); //подключение клиента
                ClientAuct.Stream = ClientAuct.Client.GetStream(); // получаем поток

                string message = ClientAuct.UserName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                ClientAuct.Stream.Write(data, 0, data.Length);

                // запускаем новый поток для получения данных
                Thread receiveThread = new Thread(new ThreadStart(ClientAuct.ReceiveMessage));
                receiveThread.Start(); //старт потока
                ClientAuct.SendMessage(Login.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            //finally
            //{
            //    ClientAuct.Disconnect();
            //}
        }


        private bool IsCheckFields()
        {
            if (!Login.Text.Equals("") && !Password.Password.Equals("")) return true;
            else return false;
        }

        static string response = null;

        public static string Response { get => response; set => response = value; }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            if (!IsCheckFields()) throw new Exception("Поле логина или пароля пусто");
            //Dispatcher.BeginInvoke(new ThreadStart(delegate { Indicator.IsBusy = true; }));
            Account account = null;
            List<Account> accounts = null;
            bool IsOpenMain = false;
            worker.DoWork += (o, ea) =>
            {
                int counter = 0;
                Request request = null;
                Dispatcher.Invoke((Action)(() =>
                {
                    request = new Request(Login.Text, "FindUser");
                }));
                ClientAuct.SendMessage(ClientAuct.SerializeToString(request));
                while (response == null)
                {
                    counter++;
                }
                IEnumerable<Account> accs = ClientAuct.DeserializeFromString<List<Account>>(response);
                counter = 0;
                Dispatcher.Invoke((Action)(() =>
                {
                    accounts = accs.ToList();
                }));
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                if (accounts.Count() != 0)
                {
                    account = accounts.First<Account>();
                    if (!account.Password.Equals(Account.GetHashCode(Password.Password)))
                    {
                        Indicator.IsBusy = false;
                        MessageBox.Show("Неверный пароль", "Ошибка", MessageBoxButton.OK);
                    }
                    else IsOpenMain = true;
                }
                else
                {
                    Indicator.IsBusy = false;
                    MessageBox.Show("Неверное имя аккаунта", "Ошибка", MessageBoxButton.OK);
                }
                if (IsOpenMain)
                {
                    if (IsSavePassword.IsChecked.Value)
                        Account.Serialize<Account>(new Account(Login.Text, GetCryptPass(Password.Password, Login.Text)), fileName);
                    MainWindow mainWindow = new MainWindow(account);
                    mainWindow.Show();
                    Indicator.IsBusy = false;
                    this.Close();
                }
            };
            Indicator.IsBusy = true;
            worker.RunWorkerAsync();
        }

        //private void Enter_Click(object sender, RoutedEventArgs e)
        //{
        //    BackgroundWorker worker = new BackgroundWorker();
        //    if (!IsCheckFields()) throw new Exception("Поле логина или пароля пусто");
        //    //Dispatcher.BeginInvoke(new ThreadStart(delegate { Indicator.IsBusy = true; }));
        //    bool IsOpenMain = false;
        //    worker.DoWork += (o, ea) =>
        //    {
        //        Account account = null;
        //        using (AuctionContext bd = new AuctionContext())
        //        {
        //            List<Account> accounts = bd.Accounts.Where(n => n.Login.Equals(Login.Text)).ToList();
        //            Dispatcher.Invoke((Action)(() =>
        //            {
        //                if (accounts.Count() != 0)
        //                {
        //                    account = accounts.First<Account>();
        //                    if (!account.Password.Equals(Account.GetHashCode(Password.Password)))
        //                    {
        //                        Indicator.IsBusy = false;
        //                        MessageBox.Show("Неверный пароль", "Ошибка", MessageBoxButton.OK);
        //                    }
        //                    else IsOpenMain = true;
        //                }
        //                else
        //                {
        //                    Indicator.IsBusy = false;
        //                    MessageBox.Show("Неверное имя аккаунта", "Ошибка", MessageBoxButton.OK);
        //                }
        //                if (IsOpenMain)
        //                {
        //                    if (IsSavePassword.IsChecked.Value)
        //                        Account.Serialize<Account>(new Account(Login.Text, GetCryptPass(Password.Password, Login.Text)), fileName);
        //                    MainWindow mainWindow = new MainWindow(account);
        //                    mainWindow.Show();
        //                }
        //            }));
        //        };
        //    };
        //    worker.RunWorkerCompleted += (o, ea) =>
        //    {
        //        if (IsOpenMain)
        //        {
        //            Indicator.IsBusy = false;
        //            this.Close();
        //        }
        //    };
        //    Indicator.IsBusy = true;
        //    worker.RunWorkerAsync();
        //}

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Login_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Login.Text.Equals("")) Enter.IsEnabled = false;
            else if (IsCheckFields()) Enter.IsEnabled = true;
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Password.Password.Equals("")) Enter.IsEnabled = false;
            else if (IsCheckFields()) Enter.IsEnabled = true;
        }
    }
}
