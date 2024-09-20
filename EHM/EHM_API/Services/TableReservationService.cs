using EHM_API.DTOs.ReservationDTO.Manager;
using EHM_API.DTOs.Table_ReservationDTO.EHM_API.DTOs;
using EHM_API.DTOs.TBDTO;
using EHM_API.Models;
using EHM_API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EHM_API.Services
{
    public class TableReservationService : ITableReservationService
    {
        private readonly ITableReservationRepository _tableReservationRepository;
        private readonly EHMDBContext _context;

        public TableReservationService(ITableReservationRepository tableReservationRepository, EHMDBContext context)
        {
            _tableReservationRepository = tableReservationRepository;
            _context = context;
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
            using var transaction = await _context.Database.BeginTransactionAsync(); // Tạo transaction để đảm bảo tính toàn vẹn
            try
            {
                // Bước 1: Xóa tất cả TableReservation theo ReservationId
                var deleteResult = await _tableReservationRepository
                    .DeleteTableReservationByReservationIdAsync(updateTableReservationDTO.ReservationId);

                if (!deleteResult)
                {
                    // Nếu không có bản ghi nào được xóa, tùy thuộc vào logic, bạn có thể quyết định tiếp tục hoặc dừng
                    // return false; // Nếu không muốn tiếp tục
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

                await transaction.CommitAsync(); // Xác nhận transaction khi tất cả thao tác thành công
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Xử lý lỗi cạnh tranh
                Console.WriteLine($"Concurrency exception: {ex.Message}");
                await transaction.RollbackAsync(); // Hủy transaction nếu có lỗi
                return false;
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi khác
                Console.WriteLine($"An error occurred: {ex.Message}");
                await transaction.RollbackAsync(); // Hủy transaction nếu có lỗi
                return false;
            }
        }

    }
}