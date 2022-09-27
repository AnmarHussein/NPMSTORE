using System;
using System.Collections.Generic;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class Payment
    {
        public Payment()
        {
            Orders = new HashSet<Order>();
        }

        public decimal Amount { get; set; }
        public decimal Id { get; set; }
        public DateTime CreateAt { get; set; }
        public string CardNumber { get; set; }

        public string CardName { get; set; }
        public string ExpiryDate { get; set; }
        public decimal SecurityCode { get; set; }
        public decimal? Customerid { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
