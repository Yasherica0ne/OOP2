using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionClient
{
    class Response
    {
        string reciever;
        string message;
        bool isWithSender;

        public Response() { }

        public Response(string reciever, string message, bool isWithSender)
        {
            this.reciever = reciever;
            this.message = message;
            this.isWithSender = isWithSender;
        }

        public string Reciever { get => reciever; set => reciever = value; }
        public string Message { get => message; set => message = value; }
        public bool IsWithSender { get => isWithSender; set => isWithSender = value; }
    }
}
