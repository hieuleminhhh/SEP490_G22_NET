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
		Task<Dish> UpdateDishe(Dish dish);
		Task<bool> DishExistsAsync(int dishId);
		Task<bool> DishNameExistsAsync(string itemName);
		Task<IEnumerable<Dish>> SearchAsync(string name);
		Task<IEnumerable<Dish>> GetAllSortedAsync(SortField? sortField, SortOrder? sortOrder);
		Task<IEnumerable<Dish>> GetSortedDishesByCategoryAsync(string? categoryName, SortField? sortField, SortOrder? sortOrder);
		Task<PagedResult<DishDTOAll>> GetDishesActive(string search, string categorySearch, int page, int pageSize);
		Task<PagedResult<DishDTOAll>> GetDishesAsync(string search, string categorySearch, int page, int pageSize);
		Task<Dish> GetDishByIdAsync(int dishId);
		Task<Dish> UpdateDishStatusAsync(int dishId, bool isActive);
		Task<List<Dish>> GetDishesByIdsAsync(List<int> dishIds);
		Task<IEnumerable<Dish>> UpdateDiscountForDishesAsync(int discountId, List<int> dishIds);
		Task<bool> DiscountExistsAsync(int discountId);
		Task<List<Dish>> SearchDishesAsync(string search);
		Task<List<Combo>> SearchCombosAsync(string search);
        bool DishExistsInOrderDetail(int dishId);
        Task DeleteIngredientsByDishIdAsync(int dishId);
        Task DeleteComboDetailsByDishIdAsync(int dishId);
        Task DeleteDishAsync(int dishId);
		Task<Dish> Update1Async(Dish dish);
     

    }
}

