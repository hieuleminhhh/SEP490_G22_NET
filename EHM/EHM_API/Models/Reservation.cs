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
        public int AddressId { get; set; }
        public int? OrderId { get; set; }
        public string? ReasonCancel { get; set; }
        public int? AccountId { get; set; }
        public string? CancelBy { get; set; }
        public int? AcceptBy { get; set; }
        public DateTime? TimeIn { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Address Address { get; set; } = null!;
        public virtual Order? Order { get; set; }
        public virtual ICollection<TableReservation> TableReservations { get; set; }
    }
}
