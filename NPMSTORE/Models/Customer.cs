using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class Customer
    {
        public Customer()
        {
            ContactUs = new HashSet<ContactU>();
            HomePages = new HashSet<HomePage>();
            Orders = new HashSet<Order>();
            Payments = new HashSet<Payment>();
            Stors = new HashSet<Stor>();
            Testimonials = new HashSet<Testimonial>();
        }

        public decimal Id { get; set; }
        public string FullName { get; set; }
        public decimal Gender { get; set; }
        public string Image { get; set; }
        public DateTime Bdate { get; set; }
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PhoneNumber { get; set; }

        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }
        public virtual CustomersLogin CustomersLogin { get; set; }
        public virtual ICollection<ContactU> ContactUs { get; set; }
        public virtual ICollection<HomePage> HomePages { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Stor> Stors { get; set; }
        public virtual ICollection<Testimonial> Testimonials { get; set; }
    }
}
