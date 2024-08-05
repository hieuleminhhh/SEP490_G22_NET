namespace EHM_API.DTOs.TableDTO.Manager
{
	public class UpdateTableAndGetOrderDTO
	{
		public int? DiscountId { get; set; }
		public List<UpdateOrderDetailDTO> OrderDetails { get; set; } = new();
	}

	public class UpdateOrderDetailDTO
	{
		public int? DishId { get; set; }
		public int? ComboId { get; set; }
		public int Quantity { get; set; }
		public string? Note { get; set; }
		public DateTime? OrderTime { get; set; }
	}
}
