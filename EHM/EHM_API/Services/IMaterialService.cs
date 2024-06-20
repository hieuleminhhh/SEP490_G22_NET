using System.Collections.Generic;
using System.Threading.Tasks;
using EHM_API.DTOs.MaterialDTO;
using EHM_API.Models;

namespace EHM_API.Services
{
    public interface IMaterialService
    {

        Task<IEnumerable<MaterialAllDTO>> GetAllMaterialsAsync();        
        Task<Material> GetMaterialByIdAsync(int id);
        Task<IEnumerable<Material>> SearchMaterialsByNameAsync(string name);
        Task<Material> CreateMaterialAsync(Material material);
        Task<Material> UpdateMaterialAsync(Material material);
        Task DeleteMaterialAsync(int id);
    }
}
