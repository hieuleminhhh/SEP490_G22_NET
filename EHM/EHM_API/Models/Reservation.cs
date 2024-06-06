using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Reservation
    {
        public int ReservationId { get; set; }
        public DateTime? ReservationTime { get; set; }
        public int? Number { get; set; }
        public string? Note { get; set; }
        public string? Status { get; set; }
        public int? TableId { get; set; }
        public string? GuestPhone { get; set; }

        public virtual Guest? GuestPhoneNavigation { get; set; }
        public virtual Table? Table { get; set; }
    }
}
