namespace EHM_API.DTOs.DiscountDTO.Manager
{
    public class CreateDiscount
    {
        public int? DiscountPercent { get; set; }
        public bool? DiscountStatus { get; set; }
        public string? DiscountName { get; set; }
        public int? Type { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Note { get; set; }
        public decimal? TotalMoney { get; set; }
        public int? QuantityLimit { get; set; }
    }
}
