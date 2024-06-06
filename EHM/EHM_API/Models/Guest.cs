using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Guest
    {
        public Guest()
        {
            Addresses = new HashSet<Address>();
            Orders = new HashSet<Order>();
            Reservations = new HashSet<Reservation>();
        }

        public string GuestPhone { get; set; } = null!;
        public string? Email { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
