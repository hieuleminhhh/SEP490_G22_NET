using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Reservation
    {
        public int ReservationId { get; set; }
        public DateTime? ReservationTime { get; set; }
        public int? GuestNumber { get; set; }
        public string? Note { get; set; }
        public int? Status { get; set; }
        public int? TableId { get; set; }
        public string GuestPhone { get; set; } = null!;
        public int? OrderId { get; set; }

        public virtual Guest GuestPhoneNavigation { get; set; } = null!;
        public virtual Order? Order { get; set; }
        public virtual Table? Table { get; set; }
    }
}
