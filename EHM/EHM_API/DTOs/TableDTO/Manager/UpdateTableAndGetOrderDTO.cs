namespace EHM_API.DTOs.TableDTO.Manager
{
	public class UpdateTableAndGetOrderDTO
	{
		public List<UpdateOrderDetailDTO> OrderDetails { get; set; } = new();
	}

	public class UpdateOrderDetailDTO
	{
		public int? DishId { get; set; }
		public int? ComboId { get; set; }
		public decimal UnitPrice { get; set; }
		public int Quantity { get; set; }
		public decimal? DiscountedPrice { get; set; }
	}
}
