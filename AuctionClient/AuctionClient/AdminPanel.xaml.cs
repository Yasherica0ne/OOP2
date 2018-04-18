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
    /// Логика взаимодействия для AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Page
    {
        private async void SetProductList()
        {
            Request request = new Request("GetNewProductList");
            string stringResp = await SendRequest(request);
            ProductList.ItemsSource = ClientAuct.DeserializeFromString<List<Product>>(stringResp);
            response = null;
            ProductList.DisplayMemberPath = "Name";
        }

        public AdminPanel()
        {
            InitializeComponent();
            SetProductList();
        }

        static string response;

        public static string Response { get => response; set => response = value; }

        private Task<string> SendRequest(Request request)
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
                return result;
            });
        }

        private void ProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Product product = (Product)ProductList.SelectedItem;
            ProductName.Text = product.Name + " - " + product.Price + "$";
            Description.Text = product.Description;
        }

        private async void Approve_Click(object sender, RoutedEventArgs e)
        {
            Request request = new Request(((Product)ProductList.SelectedItem).ProductID.ToString(), "ApproveProduct");
            string isTrueResponse = await SendRequest(request);
            if (isTrueResponse.Equals("true")) MessageBox.Show("Товар одобрен", "Уведомление");
            else MessageBox.Show("Ошибка выполнения", "Уведомление");
            response = null;
        }

        private void Cancell_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Description_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}
