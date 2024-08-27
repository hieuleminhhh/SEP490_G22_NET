namespace EHM_API.DTOs.ReservationDTO.Manager
{
	public class GetReservationByOrderDTO
	{
		public int ReservationId { get; set; }
		public DateTime? ReservationTime { get; set; }
		public int? GuestNumber { get; set; }
		public string? Note { get; set; }
		public int? Status { get; set; }
		public int? AddressId { get; set; }
		public string? GuestAddress { get; set; }
		public string? ConsigneeName { get; set; }
		public string? GuestPhone { get; set; }

		public string? Email { get; set; }
	}
}
