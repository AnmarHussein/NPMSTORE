using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class Stor
    {
        public Stor()
        {
            StoreCategories = new HashSet<StoreCategory>();
        }

        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Detailes { get; set; }
        public decimal Customerid { get; set; }


        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<StoreCategory> StoreCategories { get; set; }
    }
}
