using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class Header
    {
        public decimal Id { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public decimal HomeId { get; set; }

        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }
        public virtual HomePage Home { get; set; }
    }
}
