using EHM_API.DTOs.IngredientDTO.Manager;
using EHM_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHM_API.Repositories
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly EHMDBContext _context;

        public IngredientRepository(EHMDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ingredient>> GetAllIngredientsAsync()
        {
            return await _context.Ingredients
                .Include(i => i.Dish)
                .Include(i => i.Material)
                .ToListAsync();
        }

        public async Task<Ingredient> GetIngredientByIdAsync(int dishId, int materialId)
        {
            return await _context.Ingredients
                .Include(i => i.Dish)
                .Include(i => i.Material)
                .FirstOrDefaultAsync(i => i.DishId == dishId && i.MaterialId == materialId);
        }

        public async Task<Ingredient> CreateIngredientAsync(CreateIngredientDTO createIngredientDTO)
        {
            var ingredient = new Ingredient
            {
                DishId = createIngredientDTO.DishId,
                MaterialId = createIngredientDTO.MaterialId,
                Quantitative = createIngredientDTO.Quantitative
            };

            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();

            return ingredient;
        }

        public async Task<Ingredient> UpdateIngredientAsync(int dishId, int materialId, UpdateIngredientDTO updateIngredientDTO)
        {
            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.DishId == dishId && i.MaterialId == materialId);

            if (ingredient == null)
            {
                return null;
            }

            ingredient.MaterialId = updateIngredientDTO.MaterialId;
            ingredient.Quantitative = updateIngredientDTO.Quantitative;

            await _context.SaveChangesAsync();

            return ingredient;
        }

        public async Task<bool> DeleteIngredientAsync(int dishId, int materialId)
        {
            var sqlQuery = "DELETE FROM Ingredient WHERE DishId = {0} AND MaterialId = {1}";
            var result = await _context.Database.ExecuteSqlRawAsync(sqlQuery, dishId, materialId);
            return result > 0;
        }

        public async Task<IEnumerable<Ingredient>> SearchIngredientsByDishIdAsync(int dishId)
        {
            return await _context.Ingredients
                .Include(i => i.Dish)
                .Include(i => i.Material)
                .Where(i => i.DishId == dishId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ingredient>> SearchIngredientsByDishItemNameAsync(string dishItemName)
        {
            return await _context.Ingredients
                .Include(i => i.Dish)
                .Include(i => i.Material)
                .Where(i => i.Dish.ItemName.Contains(dishItemName))
                .ToListAsync();
        }
    }
}
