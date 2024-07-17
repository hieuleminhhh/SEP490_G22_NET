using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? Quantity { get; set; }
        public int? DishId { get; set; }
        public int? OrderId { get; set; }
        public int? DishesServed { get; set; }
        public int? ComboId { get; set; }
        public string? Note { get; set; }
        public DateTime? OrderTime { get; set; }

        public virtual Combo? Combo { get; set; }
        public virtual Dish? Dish { get; set; }
        public virtual Order? Order { get; set; }
    }
}
