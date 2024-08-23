namespace EHM_API.DTOs.OrderDetailDTO.Manager
{
    public class ItemInOrderDetail
    {
        public int OrderDetailId { get; set; }
        public string? ItemName { get; set; }
        public decimal? ItemPrice { get; set; }
        public string? ComboName { get; set; }
        public decimal? ComboPrice { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? DishesServed { get; set; }
        public DateTime? OrderTime { get; set; }
    }
}
