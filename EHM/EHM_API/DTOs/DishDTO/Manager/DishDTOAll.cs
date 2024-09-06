namespace EHM_API.DTOs.DishDTO.Manager
{
    public class DishDTOAll
    {
        public int DishId { get; set; }
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public decimal? Price { get; set; }
        public int? DiscountId { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public int? DiscountPercentage { get; set; }
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool IsActive { get; set; }
        public int? QuantityDish { get; set; }
    }
}
