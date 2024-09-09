using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Combo
    {
        public Combo()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ComboId { get; set; }
        public string? NameCombo { get; set; }
        public decimal? Price { get; set; }
        public string? Note { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsActive { get; set; }
        public int? QuantityCombo { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ComboDetail> ComboDetails { get; set; }
    }
}
