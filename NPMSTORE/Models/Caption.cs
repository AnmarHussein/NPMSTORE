using System;
using System.Collections.Generic;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class Caption
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public decimal HomeId { get; set; }

        public virtual HomePage Home { get; set; }
    }
}
