﻿namespace EHM_API.DTOs.ComboDTO
{
	public class ViewComboDTO
	{
		public int ComboId { get; set; }
		public string? NameCombo { get; set; }
		public decimal? Price { get; set; }
		public string? Note { get; set; }
		public string? ImageUrl { get; set; }
		public List<DishDTO>? Dishes { get; set; }
	}
}
