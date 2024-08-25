using System;
using System.Collections.Generic;

namespace EHM_API.Models
{
    public partial class News
    {
        public int NewsId { get; set; }
        public string? NewsTitle { get; set; }
        public string? NewsImage { get; set; }
        public DateTime? NewsDate { get; set; }
        public string? NewsContent { get; set; }
        public int? AccountId { get; set; }

        public virtual Account? Account { get; set; }
    }
}
