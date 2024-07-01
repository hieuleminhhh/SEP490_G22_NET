namespace EHM_API.DTOs.ReservationDTO.Manager
{
	public class ReservationSearchDTO
	{
		public string ConsigneeName { get; set; }
		public string GuestPhone { get; set; }
		public DateTime? ReservationTime { get; set; }
		public int? GuestNumber { get; set; }
		public decimal Deposits { get; set; }
		public List<TableDTO>? Tables { get; set; }
	}

	public class TableDTO
	{
		public int? Capacity { get; set; }
		public int? Floor { get; set; }
	}
}
