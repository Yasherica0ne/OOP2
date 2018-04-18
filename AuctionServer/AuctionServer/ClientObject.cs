using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AuctionServer
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        public bool IsOpen { get => isOpen; set => isOpen = value; }

        string userName;
        TcpClient client;
        ServerObject server; // объект сервера
        bool isOpen = false;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public async void Process()
        {
            try
            {
                Stream = client.GetStream();
                // получаем имя пользователя
                string message = null; // = GetMessage();
                //userName = message;

                //message = userName + " вошел в чат";
                //// посылаем сообщение о входе в чат всем подключенным пользователям
                //server.BroadcastMessage(message, this.Id);
                //Console.WriteLine(message);
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        Console.WriteLine(message);
                        Request request = ServerObject.DeserializeFromString<Request>(message);
                        switch (request.MethodName)
                        {
                            case "GetPurchaseList":
                                {
                                    server.SingleMessage(await GetPurchaseList(int.Parse(request.Parametr)), this.Id);
                                    break;
                                }
                            case "GetSalesList":
                                {
                                    server.SingleMessage(await GetSalesList(int.Parse(request.Parametr)), this.Id);
                                    break;
                                }
                            case "FindUser":
                                {
                                    server.SingleMessage(await FindUser(request.Parametr), this.Id);
                                    break;
                                }
                            //case "RaiseMaxBet":
                            //    {
                            //        server.BroadcastMessage(message, this.Id);
                            //        break;
                            //    }
                            case "GetNewProductList":
                                {
                                    server.SingleMessage(await GetNewProductList(), this.Id);
                                    break;
                                }
                            case "ApproveNewProduct":
                                {
                                    server.SingleMessage(await ApproveNewProduct(int.Parse(request.Parametr)), this.Id);
                                    break;
                                }
                            case "GetActualTrade":
                                {
                                    server.SingleMessage(await GetActualTrade(), this.Id);
                                    this.isOpen = true;
                                    break;
                                }
                            case "GetActualTimer":
                                {
                                    server.SingleMessage(CreateResponse("GetActualTimer", ServerObject.TimeWithoutBets.ToString()), this.Id);
                                    break;
                                }
                            case "GetActualProduct":
                                {
                                    server.SingleMessage(await GetActualProduct(), this.Id);
                                    break;
                                }
                            case "RaiseMaxBet":
                                {
                                    if (request.Parametr.Equals("X2"))
                                        ServerObject.ActualTrade.MaxBet *= 2;
                                    else
                                        ServerObject.ActualTrade.MaxBet += float.Parse(request.Parametr);
                                    ServerObject.TimeWithoutBets = 0;
                                    server.BroadcastMessage(CreateResponse("Timer", ServerObject.TimeWithoutBets.ToString(), false), this.Id);
                                    server.BroadcastMessage(CreateResponse("UpdateTrade", ServerObject.SerializeToString(ServerObject.ActualTrade), false), this.Id);
                                    server.SingleMessage(CreateResponse("RaiseMaxBet", "true"), this.Id);
                                    break;
                                }
                        }
                        //else Console.WriteLine("Error: " + message);
                        //message = String.Format("{0}: {1}", userName, message);
                        //Console.WriteLine(message);
                        //server.BroadcastMessage(message, this.Id);
                    }
                    catch
                    {
                        message = String.Format("{0}: покинул чат", userName);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        private string CreateResponse(string reciever, string message, bool isSender = true)
        {
            return ServerObject.SerializeToString(new Response(reciever, message, isSender)) + "%";
        }

        private Task<string> GetSalesList(int id)
        {
            return Task.Run(() =>
            {
                using (AuctionContext db = new AuctionContext())
                {
                    return CreateResponse("GetSalesList", ServerObject.SerializeToString(db.Products.Where(n => n.OwnerId == id).ToList<Product>()));
                }
            });
        }

        private Task<string> FindUser(string login)
        {
            return Task.Run(() =>
            {
                using (AuctionContext bd = new AuctionContext())
                {
                    List<Account> accounts = bd.Accounts.Where(n => n.Login.Equals(login)).ToList();
                    return CreateResponse("FindUser", ServerObject.SerializeToString(accounts));
                }
            });
        }

        private Task<string> GetNewProductList()
        {
            return Task.Run(() =>
            {
                using (AuctionContext db = new AuctionContext())
                {
                    List<Product> list = db.Products.Where(n => !n.IsChecked).ToList<Product>();
                    return CreateResponse("GetNewProductList", ServerObject.SerializeToString(list));
                }
            });
        }

        private Task<string> GetActualTrade()
        {
            return Task.Run(() =>
            {
                return CreateResponse("GetActualTrade", ServerObject.SerializeToString(ServerObject.ActualTrade));
            });
        }

        private Task<string> GetActualProduct()
        {
            return Task.Run(() =>
            {
                using (AuctionContext db = new AuctionContext())
                {
                    Product product = db.Products.Where(n => n.ProductID.Equals(ServerObject.ActualTrade.ProductId)).FirstOrDefault();
                    return CreateResponse("GetActualProduct", ServerObject.SerializeToString(product));
                }
            });
        }

        private Task<string> ApproveNewProduct(int id)
        {
            return Task.Run(() =>
            {
                using (AuctionContext db = new AuctionContext())
                {
                    var product = db.Products.Find(id);
                    product.IsChecked = true;
                    Trade trade = new Trade(id);
                    db.Trades.Add(trade);
                    db.SaveChangesAsync();
                    return CreateResponse("ApproveNewProduct", "true");
                }
            });
        }

        private Task<string> GetPurchaseList(int id)
        {
            return Task.Run(() =>
            {
                using (AuctionContext db = new AuctionContext())
                {
                    List<Trade> buffer = db.Trades.Where(n => n.MaxBetAccountId == id).ToList();
                    List<Product> prodList = new List<Product>();
                    foreach (Trade trade in buffer)
                    {
                        prodList.Add(db.Products.Where(n => n.ProductID == trade.ProductId).FirstOrDefault<Product>());
                    }
                    return CreateResponse("GetPurchaseList", ServerObject.SerializeToString(prodList));
                }
            });
        }

        // чтение входящего сообщения и преобразование в строку
        private string GetMessage()
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
