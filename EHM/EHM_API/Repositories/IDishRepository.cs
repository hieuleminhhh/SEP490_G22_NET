using EHM_API.Enums;
using EHM_API.Enums.EHM_API.Models;
using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface IDishRepository
    {
        Task<IEnumerable<Dish>> GetAllAsync();
        Task<Dish> GetByIdAsync(int id);
        Task<Dish> AddAsync(Dish dish);
        Task<Dish> UpdateAsync(Dish dish);
      
        Task<IEnumerable<Dish>> SearchAsync(string name);
        Task<IEnumerable<Dish>> GetAllSortedAsync(SortField sortField, SortOrder sortOrder);

    }
}

