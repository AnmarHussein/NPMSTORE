using System;
using System.Collections.Generic;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderProducts = new HashSet<OrderProduct>();
        }

        public decimal Id { get; set; }
        public decimal TotalCost { get; set; }
        public decimal OrderStat { get; set; }
        public DateTime CreateAt { get; set; }
        public decimal Paymentid { get; set; }
        public decimal Customerid { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
