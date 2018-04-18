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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AuctionClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(Account account)
        {
            InitializeComponent();
            userAcc = account;
            if (account.IsAdmin) AdminP.Visibility = Visibility.Visible;
            MainFrame.Content = new AuctionPage(account);
            ClientAuct.onTradeupdate += SetStatusBarTrade;
            ClientAuct.onStatusBarProductUpdate += SetStatusBarProduct;
            ClientAuct.onStatusBarTradeUpdate += SetStatusBarTrade;
            AuctionPage.onStatusBarLocalTradeUpdate += SetLocalStatusBarTrade;
        }

        private static string[] messages = new string[3];

        Account userAcc;

        private Trade actualTrade;
        private Product actualProduct;

        public static string[] Messages { get => messages; set => messages = value; }

        private void SetStatusBarTrade()
        {
            actualTrade = ClientAuct.DeserializeFromString<Trade>(Messages[0]);
            Dispatcher.Invoke((Action)(() =>
            {
                ActualPrice.Text = actualTrade.MaxBet.ToString();
                if (StartTime.Text.Equals("-"))
                {
                    StartTime.Text = actualTrade.TradeStartTime.ToShortTimeString();
                }
            }));
        }

        private void SetLocalStatusBarTrade()
        {
            Dispatcher.Invoke((Action)(() =>
            {
                ActualPrice.Text = messages[2];
            }));
        }

        private void SetStatusBarProduct()
        {
            actualProduct = ClientAuct.DeserializeFromString<Product>(Messages[1]);
            Dispatcher.Invoke((Action)(() =>
            {
                ActualLot.Text = actualProduct.Name;
            }));
        }

        private void Auction_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new AuctionPage(userAcc);
            Status.Text = "Auction";
        }

        private void MyCab_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new MyCabPage(userAcc);
            Status.Text = "Cabinet";
        }

        private void AdminPanel_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new AdminPanel();
            Status.Text = "Admin";
        }
    }
}
