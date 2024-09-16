using EHM_API.DTOs.ReservationDTO.Manager;
using EHM_API.DTOs.Table_ReservationDTO.EHM_API.DTOs;
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
        public async Task<bool> UpdateTableReservationsAsync(UpdateTableReservationDTO updateTableReservationDTO)
        {
            // Bước 1: Xóa tất cả TableReservation theo ReservationId
            var deleteResult = await _tableReservationRepository
                .DeleteTableReservationByReservationIdAsync(updateTableReservationDTO.ReservationId);

            if (!deleteResult)
            {
                // Nếu không có bản ghi nào được xóa, có thể vẫn tiếp tục thêm mới, tùy thuộc vào logic
            }

            // Bước 2: Thêm lại các TableReservation mới
            var tableReservations = new List<TableReservation>();

            foreach (var tableId in updateTableReservationDTO.TableIds)
            {
                tableReservations.Add(new TableReservation
                {
                    ReservationId = updateTableReservationDTO.ReservationId,
                    TableId = tableId
                });
            }

            await _tableReservationRepository.AddMultipleTableReservationsAsync(tableReservations);
            return true;
        }
    }
}