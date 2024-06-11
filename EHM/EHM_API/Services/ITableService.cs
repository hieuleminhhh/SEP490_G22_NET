using EHM_API.Models;

namespace EHM_API.Services
{
    public interface ITableService
    {
        Task<IEnumerable<Table>> GetAllAsync();
        Task<Table> GetByIdAsync(int id);
        Task<Table> CreateAsync(Table table);
        Task<Table> UpdateAsync(Table table);
        Task<bool> ChangeStatusAsync(int id, string status);
        Task<IEnumerable<Table>> SearchAsync(string keyword);
    }
}
