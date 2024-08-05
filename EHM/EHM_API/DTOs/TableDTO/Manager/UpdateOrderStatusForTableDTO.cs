namespace EHM_API.DTOs.TableDTO.Manager
{
	public class UpdateOrderStatusForTableDTO
	{
		public DateTime? RecevingOrder { get; set; }
		public DateTime? PaymentTime { get; set; }
		public string? Taxcode { get; set; }
		public int? AccountId { get; set; }
		public string? Description { get; set; }

	}
}
