namespace EHM_API.DTOs.OrderDTO.Guest
{
    public class OrderDTOAll
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public DateTime? RecevingOrder { get; set; }
        public int? TableId { get; set; }
        public int? InvoiceId { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? GuestPhone { get; set; }
        public decimal Deposits { get; set; }
        public string? GuestAddress { get; set; }
        public string? ConsigneeName { get; set; }
        public IEnumerable<OrderDetailDTO> OrderDetails { get; set; }

    }
}