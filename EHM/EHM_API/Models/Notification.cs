using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public string? Description { get; set; }
        public int? AccountId { get; set; }
        public int? OrderId { get; set; }
        public int? Type { get; set; }
        public DateTime? Time { get; set; }
        public bool? IsView { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Order? Order { get; set; }
    }
}
