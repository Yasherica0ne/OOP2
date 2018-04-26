using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab7._1
{
    class Account
    {
        private int accountId;
        private string depositType;
        private float balance;
        private DateTime createDate;
        private bool smsNotify;
        private bool internetBanking;

        public Account(int accountId, string depositType, float balance, DateTime createDate, bool smsNotify, bool internetBanking)
        {
            this.AccountId = accountId;
            this.DepositType = depositType;
            this.Balance = balance;
            this.CreateDate = createDate;
            this.SmsNotify = smsNotify;
            this.InternetBanking = internetBanking;
        }

        public int AccountId { get => accountId; set => accountId = value; }
        public string DepositType { get => depositType; set => depositType = value; }
        public float Balance { get => balance; set => balance = value; }
        public DateTime CreateDate { get => createDate; set => createDate = value; }
        public bool SmsNotify { get => smsNotify; set => smsNotify = value; }
        public bool InternetBanking { get => internetBanking; set => internetBanking = value; }
    }
}
