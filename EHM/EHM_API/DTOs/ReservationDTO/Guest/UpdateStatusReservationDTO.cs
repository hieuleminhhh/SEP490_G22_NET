using EHM_API.Models;

namespace EHM_API.DTOs.ReservationDTO.Guest
{
	public class UpdateStatusReservationDTO
	{
		public int ReservationId { get; set; }
		public int? Status { get; set; }
	}
}
