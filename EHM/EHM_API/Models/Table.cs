using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Table
    {
        public Table()
        {
            Reservations = new HashSet<Reservation>();
        }

        public int TableId { get; set; }
        public int? Status { get; set; }
        public int? Capacity { get; set; }
        public int? Floor { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
