namespace EHM_API.DTOs.DishDTO.Manager
{
	public class SearchDishAndComboDTO
	{
		public List<SearchDishDTO> Dishes { get; set; } = new();
		public List<SearchComboDTO> Combos { get; set; } = new();
	}

	public class SearchDishDTO
	{
		public int DishId { get; set; }
		public string? ItemName { get; set; }
		public string? ItemDescription { get; set; }
		public decimal? Price { get; set; }
		public decimal? DiscountedPrice { get; set; }
		public string? ImageUrl { get; set; }
		public bool IsActive { get; set; }
	}

	public class SearchComboDTO
	{
		public int ComboId { get; set; }
		public string? NameCombo { get; set; }
		public decimal? Price { get; set; }
		public string? ImageUrl { get; set; }
		public bool? IsActive { get; set; }
	}
}
