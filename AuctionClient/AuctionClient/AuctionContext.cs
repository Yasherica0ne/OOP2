using AuctionClient.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionClient
{
    class AuctionContext : DbContext
    {
        public AuctionContext() : base("DbConnection") { }
        private DbSet<Account> accounts;
        private DbSet<Product> products;
        private DbSet<Trade> trades;
        private DbSet<ProductBufferItem> productBuffer;

        public DbSet<Account> Accounts { get => accounts; set => accounts = value; }
        public DbSet<Product> Products { get => products; set => products = value; }
        public DbSet<Trade> Trades { get => trades; set => trades = value; }
        public DbSet<ProductBufferItem> ProductBuffer
        {
            get => productBuffer; set => productBuffer = value;
        }
    }
}
