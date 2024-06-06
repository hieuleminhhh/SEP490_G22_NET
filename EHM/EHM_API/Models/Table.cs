using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Table
    {
        public Table()
        {
            Orders = new HashSet<Order>();
            Reservations = new HashSet<Reservation>();
        }

        public int TableId { get; set; }
        public string? Status { get; set; }
        public int? Capacity { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
