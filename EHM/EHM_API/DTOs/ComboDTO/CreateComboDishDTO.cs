using System.ComponentModel.DataAnnotations;

namespace EHM_API.DTOs.ComboDTO
{
	namespace EHM_API.DTOs.ComboDTO
	{
		public class CreateComboDishDTO
		{
			[Required(ErrorMessage = "NameCombo is required")]
			public string? NameCombo { get; set; }

			[Required(ErrorMessage = "Price is required")]
			[Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative number")]
			public decimal? Price { get; set; }
			public string? Note { get; set; }
			public string? ImageUrl { get; set; }
			public List<DishDTO>? Dishes { get; set; }
		}
	}

}
