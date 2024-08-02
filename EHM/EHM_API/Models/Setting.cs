using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class Setting
    {
        public string? EateryName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public string? Qrcode { get; set; }
        public string? Logo { get; set; }
        public string? LinkContact { get; set; }
        public int Id { get; set; }
    }
}
