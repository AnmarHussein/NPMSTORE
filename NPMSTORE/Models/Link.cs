using System;
using System.Collections.Generic;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class Link
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Link1 { get; set; }
        public decimal HomeId { get; set; }

        public virtual HomePage Home { get; set; }
    }
}
