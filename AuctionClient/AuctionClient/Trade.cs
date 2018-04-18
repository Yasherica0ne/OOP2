using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionClient
{
    class Trade
    {
        private int tradeID;
        private int productID;
        private int maxBetAccountID;
        private float maxBet;
        private DateTime tradeStartTime;
        private DateTime tradeFinishTime;

        public int TradeId { get => tradeID; set => tradeID = value; }
        public int ProductId { get => productID; set => productID = value; }
        public int MaxBetAccountId { get => maxBetAccountID; set => maxBetAccountID = value; }
        public float MaxBet { get => maxBet; set => maxBet = value; }
        public DateTime TradeStartTime { get => tradeStartTime; set => tradeStartTime = value; }
        public DateTime TradeFinishTime { get => tradeFinishTime; set => tradeFinishTime = value; }
        

        public Trade()
        {
            this.tradeStartTime = new DateTime();
            this.tradeFinishTime = new DateTime();
        }

        public Trade(int _productID)
        {
            this.productID = _productID;
            this.tradeStartTime = new DateTime();
            this.tradeFinishTime = new DateTime();
        }
    }
}
