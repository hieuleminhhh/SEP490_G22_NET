using EHM_API.Repositories;

namespace EHM_API.Services
{
    public class TableReservationService : ITableReservationService
    {
        private readonly ITableReservationRepository _tableReservationRepository;

        public TableReservationService(ITableReservationRepository tableReservationRepository)
        {
            _tableReservationRepository = tableReservationRepository;
        }

        public async Task<bool> DeleteTableReservationByReservationIdAsync(int reservationId)
        {
            return await _tableReservationRepository.DeleteTableReservationByReservationIdAsync(reservationId);
        }
    }
}