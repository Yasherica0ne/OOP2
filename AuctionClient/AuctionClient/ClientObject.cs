using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AuctionClient
{
    public class ClientAuct
    {
        static string userName;
        private const string host = "127.0.0.1";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;
        static List<string> sender = new List<string>();
        static bool isOpenMain = false;

        public static string UserName { get => userName; set => userName = value; }

        public static string Host => host;

        public static int Port => port;

        public static TcpClient Client { get => client; set => client = value; }
        public static NetworkStream Stream { get => stream; set => stream = value; }
        public static bool IsOpenMain { get => isOpenMain; set => isOpenMain = value; }

        // отправка сообщений
        public static void SendMessage(string message)
        {
            //string message = Console.ReadLine();
            byte[] data = Encoding.Unicode.GetBytes(message);
            Stream.Write(data, 0, data.Length);
        }

        public delegate void MethodContainer();

        public static event MethodContainer onTimer;
        public static event MethodContainer onTradeupdate;
        public static event MethodContainer onStatusBarTradeUpdate;
        public static event MethodContainer onStatusBarProductUpdate;
        private static List<Response> responseList = new List<Response>();
        private static string notFullMessage = null;

        // получение сообщений
        public static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    if (notFullMessage != null)
                        builder.Append(notFullMessage);
                    int bytes = 0;
                    do
                    {
                        bytes = Stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (Stream.DataAvailable);
                    string[] bufMesaage = builder.ToString().Split('%');
                    for (int i = 0; i < bufMesaage.Length; i++)
                    {
                        if (i + 1 == bufMesaage.Length)
                        {
                            if (bufMesaage[i] != "")
                                notFullMessage = bufMesaage[i];
                            else
                                notFullMessage = null;
                            break;
                        }
                        responseList.Add(ClientAuct.DeserializeFromString<Response>(bufMesaage[i]));
                    }
                    while (responseList.Count != 0)
                    {
                        if (responseList.FirstOrDefault().IsWithSender)
                        {
                            switch (responseList.FirstOrDefault().Reciever)
                            {
                                case "GetSalesList":
                                case "GetPurchaseList":
                                    {
                                        MyCabPage.Response = responseList.FirstOrDefault().Message;
                                        break;
                                    }
                                case "FindUser": Entering.Response = responseList.FirstOrDefault().Message; break;
                                case "ApproveNewProduct":
                                case "GetNewProductList":
                                    {
                                        AdminPanel.Response = responseList.FirstOrDefault().Message;
                                        break;
                                    }
                                case "GetActualTrade":
                                    {
                                        AuctionPage.Response[0] = responseList.FirstOrDefault().Message;
                                        MainWindow.Messages[0] = responseList.FirstOrDefault().Message;
                                        onStatusBarTradeUpdate();
                                        break;
                                    }
                                case "GetActualTimer":
                                    AuctionPage.Response[1] = responseList.FirstOrDefault().Message; break;
                                case "GetActualProduct":
                                    {
                                        AuctionPage.Response[2] = responseList.FirstOrDefault().Message;
                                        MainWindow.Messages[1] = responseList.FirstOrDefault().Message; ;
                                        onStatusBarProductUpdate();
                                        break;
                                    }
                                case "RaiseMaxBet":
                                    {
                                        AuctionPage.Response[5] = responseList.FirstOrDefault().Message; break;
                                    }
                            }
                            responseList.Remove(responseList.FirstOrDefault());
                        }
                        else
                        {
                            if (IsOpenMain)
                            {

                                switch (responseList.FirstOrDefault().Reciever)
                                {
                                    case "Timer":
                                        {
                                            AuctionPage.Response[3] = responseList.FirstOrDefault().Message;
                                            onTimer();
                                            break;
                                        }
                                    case "UpdateTrade":
                                        {
                                            AuctionPage.Response[4] = responseList.FirstOrDefault().Message;
                                            MainWindow.Messages[0] = responseList.FirstOrDefault().Message;
                                            onTradeupdate();
                                            break;
                                        }
                                    //case "UpdateProduct":
                                    //    {
                                    //        MainWindow.Messages[1] = MainWindow.Messages[0] = responseList.FirstOrDefault().Message;
                                    //        onStatusBarProductUpdate();
                                    //        break;
                                    //    }
                                }
                                responseList.Remove(responseList.FirstOrDefault());
                            }
                        }
                    }
                    //Console.WriteLine(message);//вывод сообщения
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!"); //соединение было прервано
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        public static string SerializeToString<T>(T obj)
        {
            return new JavaScriptSerializer().Serialize(obj);
        }


        public static T DeserializeFromString<T>(string objectString)
        {
            return new JavaScriptSerializer().Deserialize<T>(objectString);
        }

        public static void Disconnect()
        {
            if (Stream != null)
                Stream.Close();//отключение потока
            if (Client != null)
                Client.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }
    }
}