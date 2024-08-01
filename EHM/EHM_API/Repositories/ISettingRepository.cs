using System.Threading.Tasks;
using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface ISettingRepository
    {
        Task<IEnumerable<Setting>> GetAllAsync();
        Task<Setting> AddAsync(Setting setting);
        Task<Setting> UpdateAsync(Setting setting);
        Task<Setting> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
