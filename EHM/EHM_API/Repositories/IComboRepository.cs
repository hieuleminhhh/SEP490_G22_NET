using EHM_API.DTOs.ComboDTO;
using EHM_API.Models;

public interface IComboRepository
{
    Task<ViewComboDTO> GetComboWithDishesAsync(int comboId);

    Task<Combo> GetComboByIdAsync(int comboId);

    Task<IEnumerable<Combo>> GetAllAsync();

    Task<Combo> GetByIdAsync(int id);

    Task<Combo> AddAsync(Combo combo);

    Task UpdateAsync(Combo combo);

    Task DeleteAsync(int id);
}