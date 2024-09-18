namespace EHM_API.DTOs.ComboDTO.Manager
{
    public class UpdateComboDishDTO
    {
        public string NameCombo { get; set; }
        public decimal? Price { get; set; }
        public string? Note { get; set; }
        public string? ImageUrl { get; set; }
        public List<DishComboDTO> Dishes { get; set; } 
    }

    public class DishComboDTO
    {
        public int DishId { get; set; }
        public int? QuantityDish { get; set; }
    }
}