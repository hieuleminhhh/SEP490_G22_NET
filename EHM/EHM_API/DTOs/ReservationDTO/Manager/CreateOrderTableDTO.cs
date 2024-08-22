namespace EHM_API.DTOs.ReservationDTO.Manager
{
	public class CreateOrderTableDTO
	{
		public int OrderId { get; set; }
		public List<int> TableIds { get; set; } = new List<int>();
	}
}
