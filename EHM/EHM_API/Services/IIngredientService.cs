using EHM_API.DTOs.IngredientDTO.Manager;

namespace EHM_API.Services
{
    public interface IIngredientService
    {
        Task<IEnumerable<IngredientAllDTO>> GetAllIngredientsAsync();
        Task<IngredientAllDTO> GetIngredientByIdAsync(int dishId, int materialId);
        Task<IngredientAllDTO> CreateIngredientAsync(CreateIngredientDTO createIngredientDTO);
        Task<IngredientAllDTO> UpdateIngredientAsync(int dishId, int materialId, UpdateIngredientDTO updateIngredientDTO);
        Task<bool> DeleteIngredientAsync(int dishId, int materialId);
        Task<IEnumerable<IngredientAllDTO>> SearchIngredientsByDishIdAsync(int dishId);
        Task<object> GetIngredientsWithQuantityAsync(string name, int quantity);
    }
}
