using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class Offer
    {
        public string Heading2 { get; set; }
        public decimal Id { get; set; }
        public string Pargraf { get; set; }
        public string Image { get; set; }
        public string Heading1 { get; set; }
        public decimal HomeId { get; set; }

        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }
        public virtual HomePage Home { get; set; }
    }
}
