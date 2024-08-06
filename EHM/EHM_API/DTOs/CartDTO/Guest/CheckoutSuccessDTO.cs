namespace EHM_API.DTOs.CartDTO.Guest
{
    public class CheckoutSuccessDTO
    {
        public string GuestPhone { get; set; }
        public string Email { get; set; }
        public int AddressId { get; set; }
        public string GuestAddress { get; set; }
        public string ConsigneeName { get; set; }
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int Status { get; set; }
        public DateTime? ReceivingTime { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Deposits { get; set; }
		public string? Note { get; set; }
		public int? Type { get; set; }
		public int? DiscountId { get; set; }
		public decimal? DiscountPriceOrder { get; set; }

		public List<OrderDetailsDTO> OrderDetails { get; set; }
    }

    public class OrderDetailsDTO
    {
        public string? NameCombo { get; set; }
        public string? ItemName { get; set; }
        public int DishId { get; set; }
        public int ComboId { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? Quantity { get; set; }
        public string ImageUrl { get; set; }

    }


}
