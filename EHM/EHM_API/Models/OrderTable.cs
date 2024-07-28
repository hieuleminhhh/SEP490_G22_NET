using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class OrderTable
    {
        public int OrderId { get; set; }
        public int TableId { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Table Table { get; set; } = null!;
    }
}
