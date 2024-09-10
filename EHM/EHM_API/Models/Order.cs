using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Order
    {
        public Order()
        {
            Notifications = new HashSet<Notification>();
            OrderDetails = new HashSet<OrderDetail>();
            Reservations = new HashSet<Reservation>();
        }

        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? Status { get; set; }
        public DateTime? RecevingOrder { get; set; }
        public int? AccountId { get; set; }
        public int? InvoiceId { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? GuestPhone { get; set; }
        public decimal? Deposits { get; set; }
        public int? AddressId { get; set; }
        public string? Note { get; set; }
        public int? Type { get; set; }
        public int? DiscountId { get; set; }
        public string? CancelationReason { get; set; }
        public int? ShipId { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Address? Address { get; set; }
        public virtual Discount? Discount { get; set; }
        public virtual Guest? GuestPhoneNavigation { get; set; }
        public virtual Invoice? Invoice { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<OrderTable> OrderTables { get; set; }
    }
}
