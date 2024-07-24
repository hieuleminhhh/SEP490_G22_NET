using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface ITableRepository
    {
		Task<IEnumerable<Table>> GetAllTablesAsync();

		Task<List<Table>> GetAvailableTablesByCapacityAsync(int capacity);
        Task<Table> GetTableByIdAsync(int tableId);
        Task UpdateTableAsync(Table table);
        Task<List<Table>> GetListTablesByIdsAsync(List<int> tableIds);
        Task UpdateListTablesAsync(List<Table> tables);

        Task<bool> UpdateTableStatus(int tableId, int status);
        Task<bool> ExistTable(int tableId);

	}
}
