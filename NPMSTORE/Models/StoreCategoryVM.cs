using System.Collections.Generic;

namespace NPMSTORE.Models
{
    public class StoreCategoryVM
    {
        public Product product { get; set; }

        public StoreCategory storeCategory { get; set; }

        public Category  category { get; set; }

        public Stor store  { get; set; }
    }

    public class CategoryVM
    {
        public string cateName { get; set; }

        public decimal cateId { get; set; }
        public Stor stor { get; set; }

        public List<Category> category { get; set; }
    }

    public class ProductsVM
    {
        public List<Product> productlist { get; set; }
        public string NameStore { get; set; }

        public string DetiStore { get; set; }
        public string ImgStore { get; set; }
        public decimal? IdStoe { get; set; }
    }

    public class AllProductInCategoryVM
    {
        public decimal storID { get; set; }
        public string storName { get; set; }

        public decimal cateId { get; set; }
        public string cateName  { get; set; }

        public List<Product> prodList { get; set; }
    }
}
