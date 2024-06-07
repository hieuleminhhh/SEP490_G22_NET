namespace EHM_API.DTOs.ComboDTO
{
	public class CreateComboDishDTO
	{
		public string? NameCombo { get; set; }
		public decimal? Price { get; set; }
		public string? Note { get; set; }
		public string? ImageUrl { get; set; }
		public List<DishDTO>? Dishes { get; set; }
	}
}
