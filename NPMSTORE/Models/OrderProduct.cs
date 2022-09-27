using System;
using System.Collections.Generic;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class OrderProduct
    {
        public decimal Quantity { get; set; }
        public decimal Orderid { get; set; }
        public decimal Productid { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
