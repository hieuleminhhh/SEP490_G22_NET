using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Address
    {
        public Address()
        {
            Orders = new HashSet<Order>();
            Reservations = new HashSet<Reservation>();
        }

        public int AddressId { get; set; }
        public string? GuestAddress { get; set; }
        public string? ConsigneeName { get; set; }
        public string? GuestPhone { get; set; }

        public virtual Guest? GuestPhoneNavigation { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
