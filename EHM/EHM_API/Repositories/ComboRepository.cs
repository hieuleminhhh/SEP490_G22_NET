using EHM_API.DTOs.ComboDTO;
using EHM_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EHM_API.Repositories
{
    public class ComboRepository : IComboRepository
    {
        private readonly EHMDBContext _context;

        public ComboRepository(EHMDBContext context)
        {
            _context = context;
        }

        public async Task<ViewComboDTO> GetComboWithDishesAsync(int comboId)
        {
            var comboDetails = await _context.ComboDetails
                .Include(cd => cd.Combo)
                .Include(cd => cd.Dish)
                .ThenInclude(d => d.Category)
                .Where(cd => cd.ComboId == comboId)
                .ToListAsync();

            if (!comboDetails.Any())
            {
                return null;
            }

            var combo = comboDetails.FirstOrDefault()?.Combo;

            var viewComboDTO = new ViewComboDTO
            {
                ComboId = combo.ComboId,
                NameCombo = combo.NameCombo,
                Price = combo.Price,
                Note = combo.Note,
                ImageUrl = combo.ImageUrl,
                Dishes = comboDetails.Select(cd => new DishDTO
                {
                    DishId = cd.Dish.DishId,
                    ItemName = cd.Dish.ItemName,
                    ItemDescription = cd.Dish.ItemDescription,
                    Price = cd.Dish.Price,
                    ImageUrl = cd.Dish.ImageUrl,
                    CategoryName = cd.Dish.Category.CategoryName
                }).ToList()
            };

            return viewComboDTO;
        }

        public async Task<Combo> GetComboByIdAsync(int comboId)
        {
            return await _context.Combos.FindAsync(comboId);
        }

        public async Task<IEnumerable<Combo>> GetAllAsync()
        {
            return await _context.Combos.ToListAsync();
        }

        public async Task<Combo> GetByIdAsync(int id)
        {
            return await _context.Combos.FindAsync(id);
        }

        public async Task<Combo> AddAsync(Combo combo)
        {
            _context.Combos.Add(combo);
            await _context.SaveChangesAsync();
            return combo;
        }

        public async Task UpdateAsync(Combo combo)
        {
            _context.Entry(combo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var combo = await _context.Combos.FindAsync(id);
            if (combo != null)
            {
                _context.Combos.Remove(combo);
                await _context.SaveChangesAsync();
            }
        }
    }
}