namespace EHM_API.DTOs.IngredientDTO.Manager
{
    public class CreateIngredientDTO
    {
        public int DishId { get; set; }
        public int MaterialId { get; set; }
        public int? Quantitative { get; set; }
    }
}
