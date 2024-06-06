using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? Status { get; set; }
        public DateTime? RecevingOrder { get; set; }
        public int? AccountId { get; set; }
        public int? TableId { get; set; }
        public int? InvoiceId { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? GuestPhone { get; set; }
        public decimal Deposits { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Guest? GuestPhoneNavigation { get; set; }
        public virtual Invoice? Invoice { get; set; }
        public virtual Table? Table { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
