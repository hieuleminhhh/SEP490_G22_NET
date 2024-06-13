namespace EHM_API.DTOs.OrderDTO.Guest
{
    public class OrderDTOAll
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public DateTime? RecevingOrder { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? TableId { get; set; }
        public int? InvoiceId { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? GuestPhone { get; set; }
        public decimal Deposits { get; set; }
        public string? GuestAddress { get; set; }
        public string? ConsigneeName { get; set; }
    }
}