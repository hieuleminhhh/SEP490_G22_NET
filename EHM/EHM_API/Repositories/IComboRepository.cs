using EHM_API.DTOs.ComboDTO;
using EHM_API.DTOs.ComboDTO.EHM_API.DTOs.ComboDTO;
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

		Task<Combo> AddAsync(Combo combo);

		Task UpdateAsync(Combo combo);

		Task AddComboDetailAsync(ComboDetail comboDetail);
		Task<CreateComboDishDTO> CreateComboWithDishesAsync(CreateComboDishDTO createComboDishDTO);

		Task DeleteAsync(int id);

		Task<List<Combo>> SearchComboByNameAsync(string name);
        Task<IEnumerable<Combo>> GetAllSortedAsync(SortField sortField, SortOrder sortOrder);
    }
}
