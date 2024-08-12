using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Discount
    {
        public Discount()
        {
            Dishes = new HashSet<Dish>();
            Orders = new HashSet<Order>();
        }

        public int DiscountId { get; set; }
        public int? DiscountPercent { get; set; }
        public bool? DiscountStatus { get; set; }
        public string? DiscountName { get; set; }
        public int? Type { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Note { get; set; }
        public decimal? TotalMoney { get; set; }
        public int? QuantityLimit { get; set; }

        public virtual ICollection<Dish> Dishes { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
