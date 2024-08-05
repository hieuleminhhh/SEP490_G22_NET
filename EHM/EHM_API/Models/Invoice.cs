using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Invoice
    {
        public Invoice()
        {
            InvoiceLogs = new HashSet<InvoiceLog>();
            Orders = new HashSet<Order>();
        }

        public int InvoiceId { get; set; }
        public DateTime? PaymentTime { get; set; }
        public decimal? PaymentAmount { get; set; }
        public string? Taxcode { get; set; }
        public int PaymentStatus { get; set; }
        public string? CustomerName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? AccountId { get; set; }
        public decimal? AmountReceived { get; set; }
        public decimal? ReturnAmount { get; set; }
        public int? PaymentMethods { get; set; }

        public virtual Account? Account { get; set; }
        public virtual ICollection<InvoiceLog> InvoiceLogs { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
