using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface ITableReservationRepository
    {
        Task AddTableReservationAsync(TableReservation tableReservation);
        Task<bool> DeleteTableReservationByReservationIdAsync(int reservationId);
    }
}
