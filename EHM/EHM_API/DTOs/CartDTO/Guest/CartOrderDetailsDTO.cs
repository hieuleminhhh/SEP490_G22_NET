namespace EHM_API.DTOs.CartDTO.Guest
{
    public class CartOrderDetailsDTO
    {
        public decimal? UnitPrice { get; set; }
        public int? Quantity { get; set; }
        public int? DishId { get; set; }
        public int? ComboId { get; set; }
        public string? Note { get; set; }
        public DateTime? OrderTime { get; set; }

    }
}
