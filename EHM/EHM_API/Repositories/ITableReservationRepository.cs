using EHM_API.DTOs.TBDTO;
using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface ITableReservationRepository
    {
        Task AddTableReservationAsync(TableReservation tableReservation);
        Task<bool> DeleteTableReservationByReservationIdAsync(int reservationId);
		Task CreateOrderTablesAsync(OrderTable orderTable);
        Task<IEnumerable<FindTableByReservation>> GetTableByReservationsAsync(int reservationId);
        Task AddMultipleTableReservationsAsync(List<TableReservation> tableReservations);
    }
}
