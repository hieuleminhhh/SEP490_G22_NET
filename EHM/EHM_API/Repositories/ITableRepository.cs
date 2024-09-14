using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface ITableRepository
    {
		Task<IEnumerable<Table>> GetAllTablesAsync();
        Task<Table?> GetByIdAsync(int tableId);

		Task<Table> CreateAsync(Table table);
        Task<List<Table>> GetAvailableTablesByCapacityAsync(int capacity);
        Task<Table> GetTableByIdAsync(int tableId);
        Task<Table> UpdateTableAsync(Table table);
        Task<List<Table>> GetListTablesByIdsAsync(List<int> tableIds);
        Task UpdateListTablesAsync(List<Table> tables);
        bool TableExistsInReservation(int tableId);
        Task<bool> UpdateTableStatus(int tableId, int status);
        Task<bool> ExistTable(int tableId);

        Task<bool> UpdateTableStatusByOrderId(int orderId, int status);
        Task<bool> HasOrderTableAsync(int tableId);
        Task<bool> HasTableReservationAsync(int tableId);
        Task DeleteTableAsync(int tableId);
        Task<List<Table>> GetTablesByFloorAsync(string floor);
        Task UpdateTableFloorToNullAsync(Table table);
        IEnumerable<Reservation> GetByReservationTime(DateTime reservationTime);
        Task<List<Table>> GetTablesWithStatus2AndFloorNullAsync();
        Task<List<Table>> GetAvailableTablesAsync(DateTime reservationTime);
    }
}

