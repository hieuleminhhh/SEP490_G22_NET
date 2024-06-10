﻿using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Dish
    {
        public Dish()
        {
            OrderDetails = new HashSet<OrderDetail>();
            Combos = new HashSet<Combo>();
        }

        public int DishId { get; set; }
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public decimal? Price { get; set; }
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public int? DiscountId { get; set; }
        public bool IsActive { get; set; }

        public virtual Category? Category { get; set; }
        public virtual Discount? Discount { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public virtual ICollection<Combo> Combos { get; set; }
    }
}
