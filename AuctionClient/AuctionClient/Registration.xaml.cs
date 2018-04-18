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

namespace AuctionClient
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void SecondPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (SecondPass.Password.Equals("")) Registry.IsEnabled = false;
            else if (FirstPass.Password.Length != SecondPass.Password.Length)
            {
                if (!FirstPass.Password.Contains(SecondPass.Password)) Warning.Text = "Пароли не совпадают";
                else Warning.Text = "";
                Registry.IsEnabled = false;
            }
            else if (!FirstPass.Password.Equals(SecondPass.Password))
            {
                Warning.Text = "Пароли не совпадают";
                Registry.IsEnabled = false;
            }
            else
            {
                Warning.Text = "";
                Registry.IsEnabled = true;
            }
        }

        private bool IsCheckFields()
        {
            if (!RegLogin.Text.Equals("") && !FirstPass.Password.Equals("") && !SecondPass.Password.Equals("")) return true;
            else return false;
        }

        private void Regisration_Click(object sender, RoutedEventArgs e)
        {
            //TODO: add account to database;
            using (AuctionContext db = new AuctionContext())
            {
                IEnumerable<Account> accounts = from acc in db.Accounts where acc.Login.Equals(RegLogin.Text) select acc;
                if (accounts.Count() != 0)
                    MessageBox.Show("Такой аккаунт уже существует", "Ошибка", MessageBoxButton.OK);
                else
                {
                    ((Entering)this.Owner).EnterData(RegLogin.Text, FirstPass.Password);
                    db.Accounts.Add(new Account(RegLogin.Text, Account.GetHashCode(FirstPass.Password), Email.Text));
                    db.SaveChanges();
                    this.Close();
                }
            }
        }

        private void RegLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RegLogin.Text.Equals("")) Registry.IsEnabled = false;
            else if (IsCheckFields()) Registry.IsEnabled = true;
        }

        private void FirstPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(!SecondPass.Password.Equals("")) SecondPass_PasswordChanged(sender, e);
            if (FirstPass.Password.Equals("")) Registry.IsEnabled = false;
        }
    }
}
