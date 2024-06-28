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

		public Task<Table> GetTableByIdAsync(int tableId)
		{
			throw new NotImplementedException();
		}
	}
}
