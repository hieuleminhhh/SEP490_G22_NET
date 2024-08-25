namespace EHM_API.DTOs.IngredientDTO.Manager;
public class IngredientSearchNameDTO
    {
        public int? MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public decimal? Quantitative { get; set; }
        public string? Unit { get; set; }
}

    public class DishSearchDTO
    {
        public int? DishId { get; set; }
        public string? DishName { get; set; }
        public List<IngredientSearchNameDTO> Ingredients { get; set; }
    }

    public class ComboSearchDTO
    {
        public int? ComboId { get; set; }
        public string? ComboName { get; set; }
        public List<DishSearchDTO> Dishes { get; set; }
    }
