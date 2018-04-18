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
    /// Логика взаимодействия для MyCabPage.xaml
    /// </summary>
    public partial class MyCabPage : Page
    {
        Account account;
        public MyCabPage(Account account)
        {
            this.account = account;
            InitializeComponent();
            Nickname.Text = account.Login;

        }

        private void AddProdWindow_Click(object sender, RoutedEventArgs e)
        {
            AddProductWindow addProductWindow = new AddProductWindow(account);
            addProductWindow.Show();
        }

        static string response;

        public static string Response { get => response; set => response = value; }

        private Task<List<Product>> SendRequest(Request request)
        {
            int counter = 0;
            string result = "";
            return Task.Run(() =>
            {
                ClientAuct.SendMessage(ClientAuct.SerializeToString(request));
                Dispatcher.Invoke((Action)(() =>
                {
                    while (response == null)
                {
                    counter++;
                }
                    result = response;
                    counter = 0;
                }));
                return ClientAuct.DeserializeFromString<List<Product>>(result);
            });
        }

        private async void ExpandPurchase_Expanded(object sender, RoutedEventArgs e)
        {
            Request request = new Request(account.AccountId.ToString(), "GetPurchaseList");
            List<Product> prodList = await SendRequest(request);
            PurchaseList.ItemsSource = prodList;
            PurchaseList.DisplayMemberPath = "Name";
            response = null;
        }

        private async void ExpandSales_Expanded(object sender, RoutedEventArgs e)
        {
            Request request = new Request(account.AccountId.ToString(), "GetSalesList");
            SaleList.ItemsSource = await SendRequest(request);
            SaleList.DisplayMemberPath = "Name";
            response = null;
        }
    }
}
