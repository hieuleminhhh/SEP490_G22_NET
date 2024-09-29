namespace EHM_API.DTOs.OrderDetailDTO.Manager
{
    public class OrderDetailForStaff
    {
        public int OrderId { get; set; }
        public int OrderDetailId { get; set; }
        public string? ItemName { get; set; }
        public string? ComboName { get; set; }
        public int? OrderType { get; set; }
        public DateTime? OrderTime { get; set; }
        public int? TableId { get; set; }
        public string? TableLabel { get; set; }  // New field
        public string? Floor { get; set; }       // New field
        public int QuantityRequired { get; set; }
    }
}
