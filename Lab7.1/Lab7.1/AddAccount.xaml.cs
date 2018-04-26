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
using System.Data.SqlClient;
using System.Data;

namespace Lab7._1
{
    /// <summary>
    /// Логика взаимодействия для AddAccount.xaml
    /// </summary>
    public partial class AddAccount : Window
    {
        public AddAccount()
        {
            InitializeComponent();
        }



        private void CreateDeposit_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=BankDB;Integrated Security=True";
            bool IsSmsNotify = SmsNotify.IsChecked.Value ? true : false;
            bool IsIntrnetBanking = InternetBanking.IsChecked.Value ? true : false;
            string lol = typeDeposit.SelectedItem.ToString();
            string kek = sumDeposit.Text;
            //string sqlExpression = String.Format($"INSERT INTO Account {Surname.Text},{Name.Text},{Midname.Text},{PassData.Text},{DateTime.Parse(date.Text)},{typeDeposit.SelectedItem.ToString()},{float.Parse(sumDeposit.SelectedItem.ToString())}, {IsSmsNotify}, {IsIntrnetBanking}");
            string sqlExpression = String.Format($"INSERT INTO Account (DepositType, Balance, CreateDate, SmsNotify, InternetBanking) VALUES ('{typeDeposit.Text}',{float.Parse(sumDeposit.Text)}, '{DateTime.Now}','{IsSmsNotify}', '{IsIntrnetBanking}');SET @AccountId=SCOPE_IDENTITY()");
            string sqlExpressionp2 = String.Format($"INSERT INTO AccountOwner (PassportData, AccountOwnerId, Surname, UName, Midname, BirthDate) VALUES ('{PassData.Text}', @AccId, '{Surname.Text}','{UName.Text}', '{Midname.Text}', '{date.Text}');");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlParameter idParam = new SqlParameter
                {
                    ParameterName = "@AccountId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output // параметр выходной
                };
                command.Parameters.Add(idParam);
                int number = command.ExecuteNonQuery();
                SqlCommand command2 = new SqlCommand(sqlExpressionp2, connection);
                // создаем параметр для имени
                SqlParameter nameParam = new SqlParameter("@AccId", idParam.Value);
                // добавляем параметр к команде
                command2.Parameters.Add(nameParam);
                command2.ExecuteNonQuery();
            }
            this.Close();
        }

    }
}
