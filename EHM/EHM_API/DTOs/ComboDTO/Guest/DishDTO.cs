namespace EHM_API.DTOs.ComboDTO.Guest
{
    public class DishDTO
    {
        public int DishId { get; set; }
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public decimal? Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? CategoryName { get; set; }
        public int? QuantityDish { get; set; }
    }
}
