using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Ingredient
    {
        public int? DishId { get; set; }
        public int? MaterialId { get; set; }
        public decimal? Quantitative { get; set; }

        public virtual Dish? Dish { get; set; }
        public virtual Material? Material { get; set; }
    }
}
