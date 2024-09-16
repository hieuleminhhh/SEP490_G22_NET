using EHM_API.DTOs.ReservationDTO.Manager;
using EHM_API.DTOs.Table_ReservationDTO.EHM_API.DTOs;
using EHM_API.DTOs.TBDTO;

namespace EHM_API.Services
{
    public interface ITableReservationService
    {
        Task<bool> DeleteTableReservationByReservationIdAsync(int reservationId);

		Task CreateOrderTablesAsync(CreateOrderTableDTO dto);
        Task <IEnumerable<FindTableByReservation>> GetTableByReservationsAsync(int reservationId);
        Task<bool> UpdateTableReservationsAsync(UpdateTableReservationDTO updateTableReservationDTO);
    }
}
