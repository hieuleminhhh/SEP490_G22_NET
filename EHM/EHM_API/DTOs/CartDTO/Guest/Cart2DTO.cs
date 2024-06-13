namespace EHM_API.DTOs.CartDTO.Guest
{
    public class Cart2DTO
    {
        public int DishId { get; set; }
        public int ComboId { get; set; }
        public string? NameCombo { get; set; }
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public decimal? Price { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? Note { get; set; }
        public int? Quantity { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public int? DiscountId { get; set; }
    }
}
