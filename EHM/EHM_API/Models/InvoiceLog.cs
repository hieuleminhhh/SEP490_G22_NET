using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class InvoiceLog
    {
        public int LogId { get; set; }
        public string? Description { get; set; }
        public int? InvoiceId { get; set; }

        public virtual Invoice? Invoice { get; set; }
    }
}
