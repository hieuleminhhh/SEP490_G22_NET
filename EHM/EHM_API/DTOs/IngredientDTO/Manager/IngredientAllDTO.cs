namespace EHM_API.DTOs.IngredientDTO.Manager
{
    public class IngredientAllDTO
    {
        public int DishId { get; set; }
        public int MaterialId { get; set; }
        public int? Quantitative { get; set; }
        public string DishItemName { get; set; }
        public string MaterialName { get; set; }
        public string MaterialUnit { get; set; }
    }
}
