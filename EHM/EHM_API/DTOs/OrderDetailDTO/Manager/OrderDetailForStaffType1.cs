namespace EHM_API.DTOs.OrderDetailDTO.Manager
{
    public class OrderDetailForStaffType1
    {
        public int OrderId { get; set; }
        public int OrderDetailId { get; set; }
        public string? ItemName { get; set; }
        public string? ComboName { get; set; }
        public int? OrderType { get; set; }
        public DateTime? OrderTime { get; set; }
    }
}

