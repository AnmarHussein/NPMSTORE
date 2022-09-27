using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderProducts = new HashSet<OrderProduct>();
            ProductAttributes = new HashSet<ProductAttribute>();
            StoreCategories = new HashSet<StoreCategory>();
        }

        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public decimal? Seales { get; set; }
        public string Details { get; set; }
        public DateTime CreateAt { get; set; }
        public bool? Stat { get; set; }


        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
        public virtual ICollection<ProductAttribute> ProductAttributes { get; set; }
        public virtual ICollection<StoreCategory> StoreCategories { get; set; }
    }
}
