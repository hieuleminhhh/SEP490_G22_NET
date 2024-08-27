using EHM_API.DTOs.TableDTO;

namespace EHM_API.DTOs.OrderDTO.Manager
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public DateTime? RecevingOrder { get; set; }
        public int? AccountId { get; set; }
        public List<TableAllDTO> TableIds { get; set; }
        public int? InvoiceId { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? GuestPhone { get; set; }
        public decimal Deposits { get; set; }
        public int AddressId { get; set; }
		public int? DiscountPercent { get; set; }
		public string? GuestAddress { get; set; }
        public string? ConsigneeName { get; set; }
        public string? Note { get; set; }
		public int? Type { get; set; }
        public int? DiscountId { get; set; }
        public int? PaymentStatus { get; set; }
        public int? PaymentMethods { get; set; }
    }
}
