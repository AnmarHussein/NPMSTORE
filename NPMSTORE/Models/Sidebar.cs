using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class Sidebar
    {
        public decimal Id { get; set; }
        public string Title { get; set; }
        public string Heading { get; set; }
        public string Describtion { get; set; }
        public string Image { get; set; }
        public decimal HomeId { get; set; }

        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }

        public virtual HomePage Home { get; set; }
    }
}
