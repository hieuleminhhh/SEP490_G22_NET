using EHM_API.DTOs.ReservationDTO.Manager;

namespace EHM_API.Services
{
    public interface ITableReservationService
    {
        Task<bool> DeleteTableReservationByReservationIdAsync(int reservationId);

		Task CreateOrderTablesAsync(CreateOrderTableDTO dto);
	}
}
