using System;
using System.Collections.Generic;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class Role
    {
        public decimal Id { get; set; }
        public string Name { get; set; }

        public virtual CustomersLogin CustomersLogin { get; set; }
    }
}
