using System;
using System.Collections.Generic;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class AboutU
    {
        public decimal Id { get; set; }
        public string AboutProject { get; set; }
        public string OurMission { get; set; }
        public string OurVision { get; set; }
        public string WhyChoess { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public decimal HomeId { get; set; }

        public virtual HomePage Home { get; set; }
    }
}
