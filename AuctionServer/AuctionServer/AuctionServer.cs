using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AuctionServer
{
    public class ServerObject
    {
        static TcpListener tcpListener; // сервер для прослушивания
        List<ClientObject> clients = new List<ClientObject>(); // все подключения
        static Trade actualTrade;

        internal static Trade ActualTrade { get => actualTrade; set => actualTrade = value; }
        public static int TimeWithoutBets { get => timeWithoutBets; set => timeWithoutBets = value; }

        public ServerObject()
        {
            using (AuctionContext db = new AuctionContext())
            {
                ActualTrade = db.Trades.FirstOrDefault();
            }
        }

        private static int timeWithoutBets = 0;

        public bool StartTrade()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                int countFinish = 10;
                actualTrade.TradeStartTime = DateTime.Now;
                Timer timer = new Timer((Action) =>
                {
                    if (timeWithoutBets <= countFinish)
                    {
                        timeWithoutBets++;
                        Console.WriteLine(timeWithoutBets);
                    }
                }
                , 0, 0, 3000);
                while (true)
                {
                    if (timeWithoutBets >= countFinish)
                    {
                        timer.Dispose();
                        Response response = new Response("Timer", "STOP", false);
                        Console.WriteLine("STOP");
                        FullBroadcastMessage(SerializeToString(response) + "%");
                        break;
                    }
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                actualTrade.TradeFinishTime = DateTime.Now;
            };
            worker.RunWorkerAsync();
            return true;
        }

        public static string SerializeToString<T>(T obj)
        {
            return new JavaScriptSerializer().Serialize(obj);
        }


        public static T DeserializeFromString<T>(string objectString)
        {
            return new JavaScriptSerializer().Deserialize<T>(objectString);
        }

        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            // и удаляем его из списка подключений
            if (client != null)
                clients.Remove(client);
        }
        // прослушивание входящих подключений
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        protected internal void SingleMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id == id) // если id клиента не равно id отправляющего
                {
                    clients[i].Stream.Write(data, 0, data.Length); //передача данных
                    return;
                }
            }
        }

        protected internal void FullBroadcastMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].IsOpen)
                    clients[i].Stream.Write(data, 0, data.Length); //передача данных
            }
        }

        // трансляция сообщения подключенным клиентам
        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id && clients[i].IsOpen) // если id клиента не равно id отправляющего
                {
                    clients[i].Stream.Write(data, 0, data.Length); //передача данных
                }
            }
        }
        // отключение всех клиентов
        protected internal void Disconnect()
        {
            tcpListener.Stop(); //остановка сервера

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //отключение клиента
            }
            Environment.Exit(0); //завершение процесса
        }
    }
}
