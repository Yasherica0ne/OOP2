using AuctionClient.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddProductWindow.xaml
    /// </summary>
    public partial class AddProductWindow : Window
    {
        public AddProductWindow(Account account)
        {
            this.account = account;
            InitializeComponent();
        }

        Account account;

        private void ImgSource_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddProd_Click(object sender, RoutedEventArgs e)
        {
            using (AuctionContext db = new AuctionContext())
            {
                if (!System.IO.File.Exists(ImagePath.Text))
                    MessageBox.Show("Файл не найден", "Ошибка", MessageBoxButton.OK);
                else if (Regex.IsMatch(Price.Text, @"[^0-9.,]+") || Regex.IsMatch(Price.Text, @""))
                    MessageBox.Show("Цена задана не верно", "Ошибка", MessageBoxButton.OK);
                else
                {
                    Product product = new Product(ImagePath.Text, float.Parse(Price.Text, System.Globalization.CultureInfo.InvariantCulture), "Почка Виталия", "Почка здоровая не очень большая, справка прилагается", account.AccountId);
                    //db.Products.Add(product);
                    //db.SaveChanges();
                    //ProductBufferItem pbItem = new ProductBufferItem(((Product)db.Products.Where(n => n.Name.Equals(product.Name)).FirstOrDefault()).ProductID);
                    //db.ProductBuffer.Add(pbItem);
                    //db.SaveChangesAsync();
                }
            }
        }

        private void Cancell_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
