using EHM_API.DTOs.ComboDTO;
using EHM_API.DTOs.ComboDTO.EHM_API.DTOs.ComboDTO;
using EHM_API.DTOs.HomeDTO;
using EHM_API.Enums.EHM_API.Models;
using EHM_API.Models;

namespace EHM_API.Services
{
	public interface IComboService
	{
		Task<IEnumerable<ComboDTO>> GetAllCombosAsync();

		Task<ComboDTO> GetComboByIdAsync(int comboId);
		Task<ViewComboDTO> GetComboWithDishesAsync(int comboId);

		Task<CreateComboDTO> CreateComboAsync(CreateComboDTO comboDTO);

		Task UpdateComboAsync(int id, ComboDTO comboDTO);

		Task CancelComboAsync(int comboId);
		Task<bool> ReactivateComboAsync(int comboId);

		Task<List<ComboDTO>> SearchComboByNameAsync(string name);
		Task<CreateComboDishDTO> CreateComboWithDishesAsync(CreateComboDishDTO createComboDishDTO);

		Task<IEnumerable<ComboDTO>> GetAllSortedAsync(SortField? sortField, SortOrder? sortOrder);
		Task<PagedResult<ComboDTO>> GetComboAsync(string search, int page, int pageSize);
		Task<Combo> UpdateComboStatusAsync(int comboId, bool isActive);


    }
}
