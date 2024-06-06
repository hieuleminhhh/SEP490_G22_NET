using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Discount
    {
        public Discount()
        {
            Dishes = new HashSet<Dish>();
            Invoices = new HashSet<Invoice>();
        }

        public int DiscountId { get; set; }
        public int? DiscountAmount { get; set; }
        public bool? DiscountStatus { get; set; }

        public virtual ICollection<Dish> Dishes { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}
