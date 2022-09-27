using System;
using System.Collections.Generic;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class ContactU
    {
        public decimal Id { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public decimal Customerid { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
