using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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

namespace Lab7._1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=BankDB;Integrated Security=True";
        public MainWindow()
        {
            InitializeComponent();
            //connectionString = ConfigurationManager.ConnectionStrings["BankBD"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlQuery = "SELECT * FROM Account"; //INNER JOIN AccountOwner On Account.AccountId = AccountOwner.AccountOwnerId";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    //Object[] result = null;
                    BDData.Items.Add(reader.GetValue(0));
                }
                reader.Close();
            }
        }

        private void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            new AddAccount().ShowDialog();
        }
    }
}
