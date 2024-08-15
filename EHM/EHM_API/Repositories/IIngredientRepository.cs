using EHM_API.DTOs.IngredientDTO.Manager;
using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface IIngredientRepository
    {
        Task<IEnumerable<Ingredient>> GetAllIngredientsAsync();
        Task<Ingredient> GetIngredientByIdAsync(int dishId, int materialId);
        Task<Ingredient> CreateIngredientAsync(CreateIngredientDTO createIngredientDTO);
        Task<Ingredient> UpdateIngredientAsync(int dishId, int materialId, UpdateIngredientDTO updateIngredientDTO);
        Task<bool> DeleteIngredientAsync(int dishId, int materialId);
        Task<IEnumerable<Ingredient>> SearchIngredientsByDishIdAsync(int dishId);
        Task<object> GetIngredientsWithQuantityAsync(string name, int quantity);
    }
}
