using System.Collections.Generic;

namespace NPMSTORE.Models
{
    public class OrderProductsVM
    {
        public Order order { get; set; }

        public decimal quantity { get; set; }

        public List<Product> prodList { get; set; }
    }
}
