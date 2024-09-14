namespace EHM_API.DTOs.ReservationDTO.Manager
{
	public class CheckTimeReservationDTO
	{
		public DateTime? ReservationTime { get; set; }
		public List<CheckTimeTableDTO> CurrentDayReservationTables { get; set; } = new();
		public List<CheckTimeTableDTO> AllTables { get; set; } = new();
	}

	public class CheckTimeTableDTO
	{
		public int TableId { get; set; }
		public int? Status { get; set; }
		public int? Capacity { get; set; }
		public string? Floor { get; set; }
		public DateTime? ReservationTime { get; set; }
	}
}
