﻿namespace EHM_API.DTOs.InvoiceDTO
{
	public class GetOrderCancelInfo
	{
		public int OrderId { get; set; }
		public DateTime? OrderDate { get; set; }
		public int? Status { get; set; }
		public DateTime? RecevingOrder { get; set; }
		public int? AccountId { get; set; }
		public int? InvoiceId { get; set; }
		public decimal? TotalAmount { get; set; }
		public string? GuestAddress { get; set; }
		public string? ConsigneeName { get; set; }
		public string? GuestPhone { get; set; }
		public decimal? Deposits { get; set; }
		public string? Note { get; set; }
		public int? Type { get; set; }
		public int? DiscountId { get; set; }
		public string? CancelationReason { get; set; }



	}
}