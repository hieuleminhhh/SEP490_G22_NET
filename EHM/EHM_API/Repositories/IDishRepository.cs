using EHM_API.DTOs.DishDTO.Manager;
using EHM_API.DTOs.HomeDTO;
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
		Task<bool> DishExistsAsync(int dishId);
		Task<IEnumerable<Dish>> SearchAsync(string name);
        Task<IEnumerable<Dish>> GetAllSortedAsync(SortField? sortField, SortOrder? sortOrder);
        Task<IEnumerable<Dish>> GetSortedDishesByCategoryAsync(string? categoryName, SortField? sortField, SortOrder? sortOrder);
        Task<PagedResult<DishDTOAll>> GetDishesAsync(string search, int page, int pageSize);
        Task<Dish> GetDishByIdAsync(int dishId);
        Task<Dish> UpdateDishStatusAsync(int dishId, bool isActive);
        Task<List<Dish>> GetDishesByIdsAsync(List<int> dishIds);

		Task<bool> DiscountExistsAsync(int discountId);
	}
}

