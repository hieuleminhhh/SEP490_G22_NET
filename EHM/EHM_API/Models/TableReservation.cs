using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class TableReservation
    {
        public int TableId { get; set; }
        public int ReservationId { get; set; }

        public virtual Reservation Reservation { get; set; } = null!;
        public virtual Table Table { get; set; } = null!;
    }
}
