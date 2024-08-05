namespace EHM_API.DTOs.OrderDTO.Guest
{
    public class SearchPhoneOrderDTO
    {
        public int OrderId { get; set; }
        public string? GuestPhone { get; set; }
        public int? AddressId { get; set; }
        public int Status { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string GuestAddress { get; set; }
        public string ConsigneeName { get; set; }
        public int PaymentMethods { get; set; }
        public string? Note { get; set; }
		public int? Type { get; set; }
		public int? DiscountId { get; set; }
		public IEnumerable<OrderDetailDTO> OrderDetails { get; set; }
    }

   

}
