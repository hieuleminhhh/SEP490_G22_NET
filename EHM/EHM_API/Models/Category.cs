using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Category
    {
        public Category()
        {
            Dishes = new HashSet<Dish>();
        }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public virtual ICollection<Dish> Dishes { get; set; }
    }
}
