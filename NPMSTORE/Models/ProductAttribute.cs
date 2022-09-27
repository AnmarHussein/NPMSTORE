using System;
using System.Collections.Generic;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class ProductAttribute
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Describtion { get; set; }
        public decimal Productid { get; set; }

        public virtual Product Product { get; set; }
    }
}
