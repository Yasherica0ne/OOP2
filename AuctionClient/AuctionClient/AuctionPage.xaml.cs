using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
    /// Логика взаимодействия для AuctionPage.xaml
    /// </summary>
    public partial class AuctionPage : Page
    {
        Account actualAccount;
        Trade actualTrade;
        Product actualProduct;

        private static int timeWithoutbets;

        public Account ActualAccount { get => actualAccount; set => actualAccount = value; }
        internal Trade ActualTrade { get => actualTrade; set => actualTrade = value; }
        internal Product ActualProduct { get => actualProduct; set => actualProduct = value; }
        public static string[] Response { get => response; set => response = value; }
        public static int TimeWithoutbets { get => timeWithoutbets; set => timeWithoutbets = value; }

        static string[] response = new string[6];

        public void TimerSetter()
        {
            if (response[3] != null)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    if (!Response[3].Equals("STOP"))
                        timeWithoutbets = int.Parse(Response[3]);
                    else
                    {
                        timer.Dispose();
                        timer = null;
                        IsWorkingTimer = false;
                        timeWithoutbets = cntFinish + 1;
                        Timer.Text = Response[3];
                    }
                    Response[3] = null;
                }));
            }
        }

        public void TradeUpdate()
        {
            actualTrade = ClientAuct.DeserializeFromString<Trade>(Response[4]);
            Dispatcher.Invoke((Action)(() =>
            {
                TopPrice.Text = ActualTrade.MaxBet + "$";
            }));
            Response[4] = null;
        }

        private static int cntFinish = 10;
        private static Timer timer;
        private static bool IsWorkingTimer = false;

        private void timerCall(object obj)
        {
            if (timeWithoutbets <= (int)obj)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    timeWithoutbets++;
                    Timer.Text = timeWithoutbets.ToString();
                }));
            }
        }

        private bool StartTrade(int countFinish)
        {
            TimerCallback tm = new TimerCallback(timerCall);
            timer = new Timer(tm, countFinish, 0, 3000);
            IsWorkingTimer = true;
            return true;
        }

        private Task<string> SendRequest(Request request, int index)
        {
            int counter = 0;
            string result = "";
            return Task.Run(() =>
            {
                ClientAuct.SendMessage(ClientAuct.SerializeToString(request));
                Dispatcher.Invoke((Action)(() =>
                {
                    while (Response[index] == null)
                    {
                        counter++;
                    }
                    result = Response[index];
                    counter = 0;
                }));
                return result;
            });
        }

        private async void SetActualTrade()
        {
            Request request = new Request("GetActualTrade");
            string stringResponse = await SendRequest(request, 0);
            actualTrade = ClientAuct.DeserializeFromString<Trade>(stringResponse);
            Response[0] = null;
            request = new Request("GetActualTimer");
            stringResponse = await SendRequest(request, 1);
            if (!stringResponse.Equals("STOP"))
            {
                timeWithoutbets = int.Parse(stringResponse);
                if (!IsWorkingTimer)
                {
                    StartTrade(cntFinish);
                }
            }
            else
            {
                Timer.Text = stringResponse;
            }
            Response[1] = null;
            request = new Request("GetActualProduct");
            stringResponse = await SendRequest(request, 2);
            actualProduct = ClientAuct.DeserializeFromString<Product>(stringResponse);
            Response[2] = null;
            ProductName.Text = ActualProduct.Name;
            TopPrice.Text = actualTrade.MaxBet.ToString() + "$";
            Description.Text = ActualProduct.Description;
        }


        public AuctionPage(Account account)
        {
            this.ActualAccount = account;
            InitializeComponent();
            if (timer != null)
            {
                timer.Dispose();
                IsWorkingTimer = false;
            }
            SetActualTrade();
            ClientAuct.onTimer += TimerSetter;
            ClientAuct.onTradeupdate += TradeUpdate;
            ClientAuct.IsOpenMain = true;
        }

        public delegate void MethodContainer();
        public static event MethodContainer onStatusBarLocalTradeUpdate;

        private async void RaiseMaxBet_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Content.ToString().Equals("X2"))
                actualTrade.MaxBet *= 2;
            else
                actualTrade.MaxBet += int.Parse(((Button)sender).Content.ToString());
            TopPrice.Text = actualTrade.MaxBet + "$";
            MainWindow.Messages[2] = actualTrade.MaxBet + "$";
            onStatusBarLocalTradeUpdate();
            timeWithoutbets = 0;
            Timer.Text = "0";
            Request request = new Request(((Button)sender).Content.ToString(), "RaiseMaxBet");
            string resptxt = await SendRequest(request, 5);
            if (!resptxt.Equals("true"))
                MessageBox.Show("Ставка не сделана", "Уведомление");
            response[5] = null;
        }
    }
}
