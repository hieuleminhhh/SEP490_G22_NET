namespace EHM_API.DTOs.OrderDTO.Manager
{
	public class GetDishOrderDetailDTO
	{
		public int? ComboId { get; set; }
		public int? DishId { get; set; }
		public string? ItemName { get; set; }
		public string? NameCombo { get; set; }
		public int? Quantity { get; set; }
	}
}
