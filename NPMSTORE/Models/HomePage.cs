using System;
using System.Collections.Generic;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class HomePage
    {
        public HomePage()
        {
            AboutUs = new HashSet<AboutU>();
            Captions = new HashSet<Caption>();
            Headers = new HashSet<Header>();
            Links = new HashSet<Link>();
            Offers = new HashSet<Offer>();
            Sidebars = new HashSet<Sidebar>();
        }

        public decimal Id { get; set; }
        public string Name { get; set; }
        public decimal? Customerid { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<AboutU> AboutUs { get; set; }
        public virtual ICollection<Caption> Captions { get; set; }
        public virtual ICollection<Header> Headers { get; set; }
        public virtual ICollection<Link> Links { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
        public virtual ICollection<Sidebar> Sidebars { get; set; }
    }
}
