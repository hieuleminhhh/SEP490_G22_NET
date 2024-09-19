using EHM_API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHM_API.Repositories
{
	public class TableRepository : ITableRepository
	{
		private readonly EHMDBContext _context;

		public TableRepository(EHMDBContext context)
		{
			_context = context;
		}

        public async Task<IEnumerable<Table>> GetAllTablesAsync()
        {
            return await _context.Tables
                .Include(t => t.TableReservations)
                    .ThenInclude(tr => tr.Reservation)
                    .ThenInclude(r => r.Address)
                .Select(t => new Table
                {
                    TableId = t.TableId,
                    Status = t.Status,
                    Capacity = t.Capacity,
                    Floor = t.Floor,
                    Lable = t.Lable,
                    TableReservations = t.TableReservations
                        .Where(tr => tr.Reservation.Status == 2) 
                        .ToList()
                })
                .ToListAsync();
        }


        public async Task<Table> CreateAsync(Table table)
        {
            _context.Tables.Add(table);
            await _context.SaveChangesAsync();
            return table;
        }

		public async Task<List<Table>> GetAvailableTablesByCapacityAsync(int capacity)
		{
			return await _context.Tables
				.OrderBy(t => t.Capacity)
				.ToListAsync();
		}

		public async Task<Table?> GetByIdAsync(int tableId)
		{
			return await _context.Tables.FindAsync(tableId);
		}

		//danh sach ban cua order
		public async Task<IEnumerable<Order>> GetOrdersWithTablesAsync()
		{
			return await _context.Orders
				.Include(o => o.OrderTables)
				.ThenInclude(ot => ot.Table)
				.ToListAsync();
		}
		public async Task<Table> GetTableByIdAsync(int tableId)
		{
			return await _context.Tables.FindAsync(tableId);
		}
		public async Task<bool> ExistTable(int tableId)
		{
			return await _context.Tables.AnyAsync(t => t.TableId == tableId);
		}

        public async Task<Table> UpdateTableAsync(Table table)
        {
            var existingTable = await _context.Tables.FindAsync(table.TableId);
            if (existingTable == null)
            {
                return null;
            }
            _context.Entry(existingTable).CurrentValues.SetValues(table);
            await _context.SaveChangesAsync();
            return existingTable;
        }

        public async Task<List<Table>> GetListTablesByIdsAsync(List<int> tableIds)
		{
			return await _context.Tables
								 .Where(t => tableIds.Contains(t.TableId))
								 .ToListAsync();
		}

		public async Task UpdateListTablesAsync(List<Table> tables)
		{
			_context.Tables.UpdateRange(tables);
			await _context.SaveChangesAsync();
		}


        public async Task<bool> UpdateTableStatus(int tableId, int status)
        {
            var table = await _context.Tables.FirstOrDefaultAsync(t => t.TableId == tableId);

            if (table == null)
            {
                throw new KeyNotFoundException($"Bàn {tableId} không tồn tại.");
            }

            table.Status = status;

            await _context.SaveChangesAsync();
            return true;
        }


		public async Task<bool> UpdateTableStatusByOrderId(int orderId, int status)
		{
			var tables = await _context.OrderTables
									   .Where(ot => ot.OrderId == orderId)
									   .Select(ot => ot.Table)
									   .ToListAsync();
			if (!tables.Any())
			{
				throw new KeyNotFoundException($"Không tìm thấy bàn nào cho Order {orderId}.");
			}

			foreach (var table in tables)
			{
				table.Status = status;
			}

			await _context.SaveChangesAsync();
			return true;
		}

        public bool TableExistsInReservation(int tableId)
        {
            return _context.TableReservations.Any(tr => tr.TableId == tableId);
        }

        public async Task<bool> HasOrderTableAsync(int tableId)
        {
            return await _context.OrderTables.AnyAsync(ot => ot.TableId == tableId);
        }

        public async Task<bool> HasTableReservationAsync(int tableId)
        {
            return await _context.TableReservations.AnyAsync(tr => tr.TableId == tableId);
        }

        public async Task<List<Table>> GetTablesByFloorAsync(string floor)
        {
            return await _context.Tables.Where(t => t.Floor == floor).ToListAsync();
        }
        public async Task DeleteTableAsync(int tableId)
        {
            var table = await GetTableByIdAsync(tableId);
            if (table != null)
            {
                _context.Tables.Remove(table);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateTableFloorToNullAsync(Table table)
        {
            table.Floor = null;
            table.Status = 2;
            _context.Tables.Update(table);
            await _context.SaveChangesAsync();
        }
        public IEnumerable<Reservation> GetByReservationTime(DateTime reservationTime)
        {
            // Lấy các reservation có Table gán vào (có trong TableReservation)
            var reservationsWithTable = _context.TableReservations
                .Include(tr => tr.Reservation)
                    .ThenInclude(r => r.TableReservations)
                .Include(tr => tr.Reservation.Address) // Bao gồm Address để lấy ConsigneeName và GuestPhone
                .Where(tr => tr.Reservation.ReservationTime.HasValue
                             && tr.Reservation.ReservationTime.Value.Date == reservationTime.Date
                             && tr.Reservation.Status == 2)
                .Select(tr => tr.Reservation)
                .Distinct()
                .ToList();

            // Lấy các reservation chưa có Table gán vào (không có trong TableReservation)
            var reservationsWithoutTable = _context.Reservations
                .Include(r => r.Address)  // Bao gồm Address để lấy ConsigneeName và GuestPhone
                .Where(r => r.ReservationTime.HasValue
                            && r.ReservationTime.Value.Date == reservationTime.Date
                            && r.Status == 2
                            && !_context.TableReservations.Any(tr => tr.ReservationId == r.ReservationId))
                .ToList();

            // Kết hợp hai danh sách
            return reservationsWithTable.Union(reservationsWithoutTable).ToList();
        }

        public async Task<List<Table>> GetTablesWithStatus2AndFloorNullAsync()
        {
            return await _context.Tables
                .Where(t => t.Status == 2 && t.Floor == null)
                .ToListAsync();
        }
        public async Task<List<Table>> GetAvailableTablesAsync(DateTime reservationTime)
        {
            // Khoảng thời gian +/- 3 tiếng so với reservationTime
            var timeRangeStart = reservationTime.AddHours(-3);
            var timeRangeEnd = reservationTime.AddHours(3);
            var now = DateTime.Now;

            // Lấy các bàn chưa có trong bất kỳ đơn đặt bàn nào (bàn trống) và có Status khác 2 và Floor khác null
            var freeTables = await _context.Tables
                .Where(t => !t.TableReservations.Any() && t.Status != 2 && t.Floor != null)
                .ToListAsync();

            // Lấy các bàn có đơn đặt bàn trong khoảng thời gian +/- 3 tiếng so với reservationTime
            var reservedTablesInRange = await _context.TableReservations
                .Where(tr => tr.Reservation.ReservationTime >= timeRangeStart
                          && tr.Reservation.ReservationTime <= timeRangeEnd)
                .Select(tr => tr.TableId)
                .ToListAsync();

            // Lọc ra các bàn đang hoạt động nếu reservationTime gần thời gian hiện tại
            var activeTables = new List<int>();
            if ((reservationTime - now).TotalHours < 3)
            {
                activeTables = await _context.Tables
                    .Where(t => t.Status == 1) // Status 1: Đang hoạt động
                    .Select(t => t.TableId)
                    .ToListAsync();
            }

            // Lấy tất cả các bàn có Status khác 2 và Floor khác null
            var allTables = await _context.Tables
                .Where(t => t.Status != 2 && t.Floor != null)
                .ToListAsync();

            // Chọn những bàn không có trong reservedTablesInRange và activeTables
            var availableTables = allTables
                .Where(t => !reservedTablesInRange.Contains(t.TableId) && !activeTables.Contains(t.TableId))
                .ToList();

            return availableTables;
        }

    }
}

