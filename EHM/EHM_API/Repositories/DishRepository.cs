using System.Linq;
using System.Threading.Tasks;
using EHM_API.Enums.EHM_API.Models;
using EHM_API.Models;
using Microsoft.EntityFrameworkCore;

namespace EHM_API.Repositories
{
    public class DishRepository : IDishRepository
    {
        private readonly EHMDBContext _context;

        public DishRepository(EHMDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dish>> GetAllAsync()
        {
            return await _context.Dishes
                .Include(d => d.Category)
                .Include(d => d.Discount)
                .ToListAsync();
        }

        public async Task<Dish> GetByIdAsync(int id)
        {
            return await _context.Dishes
                .Include(d => d.Category)
                .Include(d => d.Discount)
                .FirstOrDefaultAsync(d => d.DishId == id);
        }

        public async Task<Dish> AddAsync(Dish dish)
        {
            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();
            return dish;
        }

        public async Task<Dish> UpdateAsync(Dish dish)
        {
            _context.Dishes.Update(dish);
            await _context.SaveChangesAsync();
            return dish;
        }

       

        public async Task DeleteIngredientsAsync(int dishId)
        {
            var ingredients = await _context.Ingredients
                .Where(i => i.DishId == dishId)
                .ToListAsync();

            _context.Ingredients.RemoveRange(ingredients);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteComboDetailsAsync(int dishId)
        {
            var sql = "DELETE FROM ComboDetails WHERE DishId = @p0";
            await _context.Database.ExecuteSqlRawAsync(sql, dishId);
        }

        public async Task DeleteOrderDetailsAsync(int dishId)
        {
            var orderDetails = await _context.OrderDetails
                .Where(od => od.DishId == dishId)
                .ToListAsync();

            _context.OrderDetails.RemoveRange(orderDetails);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Dish>> SearchAsync(string name)
        {
            return await _context.Dishes
                .Include(d => d.Category)
                .Include(d => d.Discount)
                .Where(d => d.ItemName.Contains(name))
                .ToListAsync();
        }

        public async Task<IEnumerable<Dish>> GetAllSortedAsync(SortField sortField, SortOrder sortOrder)
        {
            IQueryable<Dish> query = _context.Dishes;

            switch (sortField)
            {
                case SortField.Name:
                    query = sortOrder == SortOrder.Ascending ? query.OrderBy(d => d.ItemName) : query.OrderByDescending(d => d.ItemName);
                    break;
                case SortField.Price:
                    query = sortOrder == SortOrder.Ascending ? query.OrderBy(d => d.Price) : query.OrderByDescending(d => d.Price);
                    break;
                case SortField.OrderQuantity:
                    query = sortOrder == SortOrder.Ascending ? query.OrderBy(d => d.OrderDetails.Sum(od => od.Quantity)) : query.OrderByDescending(d => d.OrderDetails.Sum(od => od.Quantity));
                    break;
                default:
                    throw new ArgumentException("Invalid sort field.");
            }

            return await query.ToListAsync();
        }
    }
}
