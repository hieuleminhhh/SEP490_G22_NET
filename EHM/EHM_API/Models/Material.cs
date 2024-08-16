using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Material
    {
        public int MaterialId { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Unit { get; set; }
        public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    }
}
