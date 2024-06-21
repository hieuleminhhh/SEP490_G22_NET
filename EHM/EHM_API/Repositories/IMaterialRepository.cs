using System.Collections.Generic;
using System.Threading.Tasks;
using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface IMaterialRepository
    {
        Task<IEnumerable<Material>> GetAllAsync();
        Task<Material> GetByIdAsync(int id);
        Task<IEnumerable<Material>> SearchByNameAsync(string name);
        Task<Material> CreateAsync(Material material);
        Task<Material> UpdateAsync(Material material);
        Task DeleteAsync(int id);
        Task DeleteIngredientsByMaterialIDAsync(int materialId);
    }
}
