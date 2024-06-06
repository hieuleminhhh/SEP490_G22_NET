using EHM_API.DTOs.ComboDTO;

public interface IComboService
{
    Task<IEnumerable<ComboDTO>> GetAllCombosAsync();

    Task<ComboDTO> GetComboByIdAsync(int comboId);
    Task<ViewComboDTO> GetComboWithDishesAsync(int comboId);

    Task<ComboDTO> CreateComboAsync(ComboDTO comboDTO);

    Task UpdateComboAsync(int id, ComboDTO comboDTO);

    Task DeleteComboAsync(int id);
}