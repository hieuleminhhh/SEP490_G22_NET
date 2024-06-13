using System.ComponentModel.DataAnnotations;

namespace EHM_API.DTOs.ComboDTO
{
	namespace EHM_API.DTOs.ComboDTO
	{
		public class CreateComboDishDTO
		{
            public string? NameCombo { get; set; }
			public decimal? Price { get; set; }
			public string? Note { get; set; }
			public string? ImageUrl { get; set; }
			public bool? IsActive { get; set; }
            public List<int> DishIds { get; set; }
        }
	}

}
