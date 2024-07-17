using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class ComboDetail
    {
        public int DishId { get; set; }
        public int ComboId { get; set; }
        public int? QuantityDish { get; set; }

        public virtual Combo Combo { get; set; } = null!;
        public virtual Dish Dish { get; set; } = null!;
    }
}
