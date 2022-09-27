using System.Collections.Generic;

namespace NPMSTORE.Models
{
    public class HomeVM
    {
        public HomePage home { get; set; }
        public Header header { get; set; }
        public Sidebar sidebar { get; set; }
        public Offer offer { get; set; }
        public List<Link> links { get; set; }
        public List<Caption> caption { get; set; }
        public AboutU aboutU { get; set; }
    }
}
