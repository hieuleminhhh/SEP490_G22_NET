using EHM_API.DTOs.ComboDTO.EHM_API.DTOs.ComboDTO;
using EHM_API.DTOs.ComboDTO.Guest;
using EHM_API.DTOs.HomeDTO;
using EHM_API.Enums.EHM_API.Models;
using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface IComboRepository
	{
		Task<ViewComboDTO> GetComboWithDishesAsync(int comboId);

		Task<Combo> GetComboByIdAsync(int comboId);

		Task<IEnumerable<Combo>> GetAllAsync();

		Task<Combo> GetByIdAsync(int id);

		Task<bool> ComboExistsAsync(int comboId);
		Task<Combo> AddAsync(Combo combo);

		Task UpdateAsync(Combo combo);

		Task AddComboDetailAsync(ComboDetail comboDetail);
		Task UpdateStatusAsync(int comboId, bool isActive);
		Task<bool> CanActivateComboAsync(int comboId);

		Task<List<Combo>> SearchComboByNameAsync(string name);
		Task<IEnumerable<Combo>> GetAllSortedAsync(SortField? sortField, SortOrder? sortOrder);
		Task<PagedResult<ViewComboDTO>> GetComboAsync(string search, int page, int pageSize);
		Task<PagedResult<ViewComboDTO>> GetComboActive(string search, int page, int pageSize);


		Task<Combo> UpdateComboStatusAsync(int comboId, bool isActive);
		Task ClearComboDetailsAsync(int comboId);
        Task DeleteAsync(Combo combo);
        Task DeleteByComboIdAsync(int comboId);
        Task<bool> ExistsWithComboIdAsync(int comboId);
        Task<List<Combo>> GetCombosByIdsAsync(List<int> comboIds);
    }
}
