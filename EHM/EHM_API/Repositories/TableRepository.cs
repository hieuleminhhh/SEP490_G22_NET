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

        public async Task<IEnumerable<Table>> GetAllAsync()
        {
            return await _context.Tables.ToListAsync();
        }

        public async Task<Table> GetByIdAsync(int id)
        {
            return await _context.Tables.FindAsync(id);
        }

        public async Task<Table> CreateAsync(Table table)
        {
            _context.Tables.Add(table);
            await _context.SaveChangesAsync();
            return table;
        }

        public async Task<Table> UpdateAsync(Table table)
        {
            _context.Entry(table).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return table;
        }

        public async Task<bool> ChangeStatusAsync(int id, int status)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null)
            {
                return false;
            }

            table.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Table>> SearchAsync(string keyword)
        {
            /*  return await _context.Tables
                  .Where(t => t.Status.Contains(keyword) || t.Capacity.ToString().Contains(keyword))
                  .ToListAsync();*/
           return null;
        }

		public Task<bool> ChangeStatusAsync(int id, string status)
		{
			throw new NotImplementedException();
		}
	}
}
