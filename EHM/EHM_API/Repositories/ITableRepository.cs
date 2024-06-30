using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface ITableRepository
    {
		Task<IEnumerable<Table>> GetAllTablesAsync();

		Task<List<Table>> GetAvailableTablesByCapacityAsync(int capacity);
        Task<Table> GetTableByIdAsync(int tableId);
       
    }
}
