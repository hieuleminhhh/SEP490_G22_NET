using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Table
    {
        public int TableId { get; set; }
        public int? Status { get; set; }
        public int? Capacity { get; set; }
        public string? Floor { get; set; }
        public string? Lable { get; set; }
        public virtual ICollection<TableReservation> TableReservations { get; set; }
        public virtual ICollection<OrderTable> OrderTables { get; set; }
    }
}
