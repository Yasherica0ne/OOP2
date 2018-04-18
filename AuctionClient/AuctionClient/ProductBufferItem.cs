using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations; //.Schema;

namespace AuctionClient.Resources
{
    class ProductBufferItem
    {
        private int productID;
        private bool isChecked;
        private string status;

        public bool GetStatus()
        {
            if (this.status.Equals("Approved"))
                return true;
            else
                return false;
        }

        public void SetStatus(bool IsApproved)
        {
            if (IsApproved)
                this.status = "Approved";
            else
                this.status = "Denied";
        }

        public ProductBufferItem(int _productID)
        {
            this.productID = _productID;
            this.IsChecked = false;
            //this.status = "";
        }
        [Key]
        public int ProductId { get => productID; set => productID = value; }
        public bool IsChecked { get => isChecked; set => isChecked = value; }
        public string Status { get => status; set => status = value; }
    }
}
