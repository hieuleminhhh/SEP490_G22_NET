using EHM_API.DTOs.ComboDTO.Guest;
using EHM_API.DTOs.ComboDTO.Manager;
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

            var existingIngredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.DishId == createIngredientDTO.DishId && i.MaterialId == createIngredientDTO.MaterialId);

            if (existingIngredient != null)
            {
                throw new InvalidOperationException("The ingredient with the specified DishId and MaterialId already exists.");
            }

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

            var existingIngredient = await _context.Ingredients
                                                    .FirstOrDefaultAsync(i => i.DishId == dishId && i.MaterialId == materialId);

            if (existingIngredient == null)
            {
                return null;
            }


            var duplicateIngredient = await _context.Ingredients
                                                     .FirstOrDefaultAsync(i => i.DishId == dishId && i.MaterialId == updateIngredientDTO.MaterialId);

            if (duplicateIngredient != null && duplicateIngredient.MaterialId != materialId)
            {
                throw new InvalidOperationException("An ingredient with the same DishId and MaterialId already exists.");
            }

            existingIngredient.MaterialId = updateIngredientDTO.MaterialId;
            existingIngredient.Quantitative = updateIngredientDTO.Quantitative;

            _context.Ingredients.Update(existingIngredient);
            await _context.SaveChangesAsync();

            return existingIngredient;
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

        public async Task<object> GetIngredientsWithQuantityAsync(string name, int quantity)
        {
            // Tìm món ăn
            var dish = await _context.Dishes
                .Include(d => d.Ingredients)
                .ThenInclude(i => i.Material)
                .FirstOrDefaultAsync(d => d.ItemName.Contains(name));

            if (dish != null)
            {
                return new
                {
                    DishId = dish.DishId,
                    DishName = dish.ItemName,
                    Ingredients = dish.Ingredients.Select(i => new
                    {
                        i.MaterialId,
                        i.Material.Name,
                        Quantitative = i.Quantitative * quantity,
                        Unit = i.Material.Unit
                    }).ToList()
                };
            }

            // Tìm Combo
            var combo = await _context.Combos
                .Include(c => c.ComboDetails)
                .ThenInclude(cd => cd.Dish)
                .ThenInclude(d => d.Ingredients)
                .ThenInclude(i => i.Material)
                .FirstOrDefaultAsync(c => c.NameCombo.Contains(name));

            if (combo != null)
            {
                var dishesWithIngredients = new List<DishSearchDTO>();

                foreach (var comboDetail in combo.ComboDetails)
                {
                    var dishInCombo = comboDetail.Dish;
                    dishesWithIngredients.Add(new DishSearchDTO
                    {
                        DishId = dishInCombo.DishId,
                        DishName = dishInCombo.ItemName,
                        Ingredients = dishInCombo.Ingredients.Select(i => new IngredientSearchNameDTO
                        {
                            MaterialId = i.MaterialId,
                            MaterialName = i.Material.Name,
                            Quantitative = i.Quantitative * comboDetail.QuantityDish.Value * quantity,
                            Unit = i.Material.Unit
                        }).ToList()
                    });
                }

                return new ComboSearchDTO
                {
                    ComboId = combo.ComboId,
                    ComboName = combo.NameCombo,
                    Dishes = dishesWithIngredients
                };
            }

            return null;
        }
    }
}
