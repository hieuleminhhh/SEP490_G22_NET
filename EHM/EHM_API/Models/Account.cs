using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Account
    {
        public Account()
        {
            Invoices = new HashSet<Invoice>();
            News = new HashSet<News>();
            Notifications = new HashSet<Notification>();
            Orders = new HashSet<Order>();
            Reservations = new HashSet<Reservation>();
        }

        public int AccountId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<News> News { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
