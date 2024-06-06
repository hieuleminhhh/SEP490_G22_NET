﻿namespace EHM_API.DTOs.ComboDTO
{
    public class ComboDTO
    {
        public int ComboId { get; set; }
        public string? NameCombo { get; set; }
        public decimal? Price { get; set; }
        public string? Note { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsActive { get; set; }
    }
}
