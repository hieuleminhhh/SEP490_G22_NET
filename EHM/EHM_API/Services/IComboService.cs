using EHM_API.DTOs.ComboDTO;

namespace EHM_API.Services
{
	public interface IComboService
	{
		Task<IEnumerable<ComboDTO>> GetAllCombosAsync();

		Task<ComboDTO> GetComboByIdAsync(int comboId);
		Task<ViewComboDTO> GetComboWithDishesAsync(int comboId);

		Task<CreateComboDTO> CreateComboAsync(CreateComboDTO comboDTO);

		Task UpdateComboAsync(int id, ComboDTO comboDTO);

		Task DeleteComboAsync(int id);
		Task<List<ComboDTO>> SearchComboByNameAsync(string name);
		Task<CreateComboDishDTO> CreateComboWithDishesAsync(CreateComboDishDTO createComboDishDTO);
	}
}
