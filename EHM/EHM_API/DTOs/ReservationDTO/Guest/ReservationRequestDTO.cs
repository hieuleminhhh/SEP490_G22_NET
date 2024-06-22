namespace EHM_API.DTOs.ReservationDTO.Guest
{
	public class ReservationRequestDTO
	{
		public string? ConsigneeName { get; set; }
		public string? GuestPhone { get; set; }

		public DateTime? ReservationTime { get; set; }

		public int? Status { get; set; }
		public int? StatusOrder { get; set; }
		public int? GuestNumber { get; set; }
		public decimal Deposits { get; set; }

	}
}
