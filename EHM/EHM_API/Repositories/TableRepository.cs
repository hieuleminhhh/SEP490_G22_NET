using EHM_API.Models;
using Microsoft.EntityFrameworkCore;
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
			return await _context.Tables.ToListAsync();
		}

		public async Task<List<Table>> GetAvailableTablesByCapacityAsync(int capacity)
		{
			return await _context.Tables
				.OrderBy(t => t.Capacity)
				.ToListAsync();
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
        public async Task UpdateTableAsync(Table table)
        {
            _context.Tables.Update(table);
            await _context.SaveChangesAsync();
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
    }
}
