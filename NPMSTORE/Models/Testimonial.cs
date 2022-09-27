using System;
using System.Collections.Generic;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class Testimonial
    {
        public decimal Id { get; set; }
        public string Details { get; set; }
        public decimal Approved { get; set; }
        public decimal Customerid { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
