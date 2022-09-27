using System;
using System.Collections.Generic;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class Category
    {
        public Category()
        {
            StoreCategories = new HashSet<StoreCategory>();
        }

        public decimal Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<StoreCategory> StoreCategories { get; set; }
    }
}
