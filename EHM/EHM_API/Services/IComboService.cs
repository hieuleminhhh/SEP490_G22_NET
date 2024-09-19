using EHM_API.DTOs.ComboDTO.EHM_API.DTOs.ComboDTO;
using EHM_API.DTOs.ComboDTO.Guest;
using EHM_API.DTOs.ComboDTO.Manager;
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
		Task<ComboDTO> CreateComboWithDishesAsync(UpdateComboDishDTO createComboWithDishesDTO);
        Task<UpdateComboDishDTO> UpdateComboWithDishesAsync(int comboId, UpdateComboDishDTO updateComboWithDishesDTO);
        Task<IEnumerable<ComboDTO>> GetAllSortedAsync(SortField? sortField, SortOrder? sortOrder);
		Task<PagedResult<ViewComboDTO>> GetComboActive(string search, int page, int pageSize);
		Task<PagedResult<ViewComboDTO>> GetComboAsync(string search, int page, int pageSize);

        Task<Combo> UpdateComboStatusAsync(int comboId, bool isActive);
		Task<bool> ComboExistsAsync(int comboId);
        Task DeleteComboAsync(int comboId);
        Task UpdateQuantityComboAsync(UpdateQuantityComboDTO updateQuantityComboDTO);

    }
}
