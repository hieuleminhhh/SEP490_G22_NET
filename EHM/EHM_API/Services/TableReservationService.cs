using EHM_API.DTOs.ReservationDTO.Manager;
using EHM_API.DTOs.TBDTO;
using EHM_API.Models;
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

		public async Task CreateOrderTablesAsync(CreateOrderTableDTO dto)
		{
			foreach (var tableId in dto.TableIds)
			{
				var orderTable = new OrderTable
				{
					OrderId = dto.OrderId,
					TableId = tableId
				};
				await _tableReservationRepository.CreateOrderTablesAsync(orderTable);
			}
		}
		public async Task<IEnumerable<FindTableByReservation>> GetTableByReservationsAsync(int reservationId)
		{
            return (IEnumerable<FindTableByReservation>)await _tableReservationRepository.GetTableByReservationsAsync(reservationId);
		}

    }
}