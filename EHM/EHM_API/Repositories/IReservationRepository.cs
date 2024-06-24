using EHM_API.DTOs.ReservationDTO.Guest;
using EHM_API.Models;

namespace EHM_API.Repositories
{
	public interface IReservationRepository
	{
		Task<IEnumerable<Reservation>> GetReservationsByStatus(int? status);

		Task CreateReservationAsync(Reservation reservation);
		Task<Guest> GetOrCreateGuest(string guestPhone, string email);
		Task<Address> GetOrCreateAddress(string guestPhone, string? guestAddress, string? consigneeName);
		Task<Reservation> GetReservationDetailAsync(int reservationId);
		Task<bool> UpdateStatusAsync(UpdateStatusReservationDTO updateStatusDto);
		Task<bool> UpdateTableIdAsync(UpdateTableIdDTO updateTableIdDTO);
	}
}
