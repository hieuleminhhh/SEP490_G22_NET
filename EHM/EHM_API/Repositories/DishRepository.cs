using System.Linq;
using System.Threading.Tasks;
using EHM_API.DTOs.DishDTO.Manager;
using EHM_API.DTOs.HomeDTO;
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

		public async Task<bool> DishExistsAsync(int dishId)
		{
			return await _context.Dishes.AnyAsync(d => d.DishId == dishId);
		}


		public async Task<Dish> AddAsync(Dish dish)
        {
          
            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();
            return dish;
        }

        public async Task<Dish> UpdateAsync(Dish dish)
        {
            var discount = await _context.Discounts.FindAsync(dish.DiscountId);
            if (discount != null || discount.Type != 2)
            {
                throw new InvalidOperationException("Nhập discountID có Type = 2 .");
            }
            _context.Dishes.Update(dish);
            await _context.SaveChangesAsync();
            return dish;
        }

		public async Task<Dish> UpdateDishe(Dish dish)
		{
			var discount = await _context.Discounts.FindAsync(dish.DiscountId);
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
                .Where(d => d.ItemName.Equals(name))
                .ToListAsync();
        }

        public async Task<IEnumerable<Dish>> GetAllSortedAsync(SortField? sortField, SortOrder? sortOrder)
        {
            IQueryable<Dish> query = _context.Dishes
                .Include(d => d.Category)
                .Include(d => d.Discount);

            if (sortField.HasValue && sortOrder.HasValue)
            {
                query = ApplySorting(query, sortField.Value, sortOrder.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Dish>> GetSortedDishesByCategoryAsync(string? categoryName, SortField? sortField, SortOrder? sortOrder)
        {
            IQueryable<Dish> query = _context.Dishes
                .Include(d => d.Category)
                .Include(d => d.Discount);

            if (!string.IsNullOrEmpty(categoryName))
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName.Equals(categoryName));
                if (category != null)
                {
                    query = query.Where(d => d.CategoryId == category.CategoryId);
                }
            }

            if (sortField.HasValue && sortOrder.HasValue)
            {
                query = ApplySorting(query, sortField.Value, sortOrder.Value);
            }

            return await query.ToListAsync();
        }

        private IQueryable<Dish> ApplySorting(IQueryable<Dish> query, SortField sortField, SortOrder sortOrder)
        {
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
                    throw new ArgumentException("Trường sắp xếp không hợp lệ.");
            }
            return query;
        }

        public async Task<PagedResult<DishDTOAll>> GetDishesAsync(string search, string categorySearch, int page, int pageSize)
        {
            var query = _context.Dishes.AsQueryable();

            if (!string.IsNullOrEmpty(categorySearch))
            {
                categorySearch = categorySearch.ToLower();
                query = query.Where(d => d.Category.CategoryName.ToLower().Contains(categorySearch));
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(d => d.ItemName.ToLower().Contains(search));
            }

            var totalDishes = await query.CountAsync();

            var dishes = await query
                .Include(d => d.Category).Include(d => d.Discount)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dishDTOs = dishes.Select(d => new DishDTOAll
            {
                DishId = d.DishId,
                ItemName = d.ItemName,
                ItemDescription = d.ItemDescription,
                Price = d.Price,
                ImageUrl = d.ImageUrl,
                CategoryId = d.CategoryId,
                CategoryName = d.Category?.CategoryName,
                IsActive = d.IsActive,
                DiscountId = d.DishId,
                DiscountedPrice = d.Price - (d.Price * d.Discount?.DiscountPercent / 100),
                DiscountPercentage = d.Discount?.DiscountPercent
            }).ToList();

            return new PagedResult<DishDTOAll>(dishDTOs, totalDishes, page, pageSize);
        }


		public async Task<PagedResult<DishDTOAll>> GetDishesActive(string search, string categorySearch, int page, int pageSize)
		{
			var query = _context.Dishes.AsQueryable();
			// Lọc các món ăn có IsActive = true
			query = query.Where(d => d.IsActive);

			if (!string.IsNullOrEmpty(categorySearch))
			{
				categorySearch = categorySearch.ToLower();
				query = query.Where(d => d.Category.CategoryName.ToLower().Contains(categorySearch));
			}

			if (!string.IsNullOrEmpty(search))
			{
				search = search.ToLower();
				query = query.Where(d => d.ItemName.ToLower().Contains(search));
			}

			var totalDishes = await query.CountAsync();

			var dishes = await query
				.Include(d => d.Category)
				.Include(d => d.Discount)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var dishDTOs = dishes.Select(d => new DishDTOAll
			{
				DishId = d.DishId,
				ItemName = d.ItemName,
				ItemDescription = d.ItemDescription,
				Price = d.Price,
				ImageUrl = d.ImageUrl,
				CategoryId = d.CategoryId,
				CategoryName = d.Category?.CategoryName,
				IsActive = d.IsActive,
				DiscountId = d.DishId,
				DiscountedPrice = d.Price - (d.Price * d.Discount?.DiscountPercent / 100),
				DiscountPercentage = d.Discount?.DiscountPercent
			}).ToList();

			return new PagedResult<DishDTOAll>(dishDTOs, totalDishes, page, pageSize);
		}




		public async Task<Dish> GetDishByIdAsync(int dishId)
        {
            return await _context.Dishes.FindAsync(dishId);
        }
        public async Task<List<Dish>> GetDishesByIdsAsync(List<int> dishIds)
        {
            return await _context.Dishes.Where(d => dishIds.Contains(d.DishId)).ToListAsync();
        }
        public async Task<Dish> UpdateDishStatusAsync(int dishId, bool isActive)
        {
            var dish = await _context.Dishes.FindAsync(dishId);
            if (dish == null)
            {
                return null;
            }

            dish.IsActive = isActive;
            _context.Dishes.Update(dish);
            await _context.SaveChangesAsync();

            return dish;
        }

		public async Task<bool> DiscountExistsAsync(int discountId)
		{
			return await _context.Discounts.AnyAsync(d => d.DiscountId == discountId);
		}

		public async Task<List<Dish>> SearchDishesAsync(string search)
		{
			return await _context.Dishes
				.Where(d => d.ItemName != null && d.ItemName.Contains(search))
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<List<Combo>> SearchCombosAsync(string search)
		{
			return await _context.Combos
				.Where(c => c.NameCombo != null && c.NameCombo.Contains(search))
				.AsNoTracking()
				.ToListAsync();
		}
        public async Task<IEnumerable<Dish>> UpdateDiscountForDishesAsync(int discountId, List<int> dishIds)
        {
            var discount = await _context.Discounts.FindAsync(discountId);
            if (discount == null || discount.Type != 2)
            {
                throw new InvalidOperationException("Nhập discountID có Type = 2 .");
            }

            var dishes = await _context.Dishes.Where(d => dishIds.Contains(d.DishId)).ToListAsync();
            foreach (var dish in dishes)
            {
                dish.DiscountId = discountId;
            }

            await _context.SaveChangesAsync();
            return dishes;
        }
    }

}
