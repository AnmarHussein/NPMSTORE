using System;
using System.Collections.Generic;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class StoreCategory
    {
        public decimal Id { get; set; }
        public decimal? Storeid { get; set; }
        public decimal? Categoryid { get; set; }
        public decimal? Productsid { get; set; }

        public virtual Category Category { get; set; }
        public virtual Product Products { get; set; }
        public virtual Stor Store { get; set; }
    }
}
