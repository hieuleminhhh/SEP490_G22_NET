using EHM_API.DTOs.CategoryDTO.Guest;
using EHM_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Repositories
{
    public class CategoryRepository : ICategoryRepository
	{
		private readonly EHMDBContext _context;

		public CategoryRepository(EHMDBContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Category>> GetAllAsync()
		{
			return await _context.Categories.ToListAsync();
		}

		public async Task<Category> GetByIdAsync(int id)
		{
			return await _context.Categories.FindAsync(id);
		}

		public async Task<Category> AddAsync(Category category)
		{
			_context.Categories.Add(category);
			await _context.SaveChangesAsync();
			return category;
		}

		public async Task<Category> UpdateAsync(Category category)
		{
			_context.Categories.Update(category);
			await _context.SaveChangesAsync();
			return category;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var category = await _context.Categories.FindAsync(id);
			if (category == null)
			{
				return false;
			}

			_context.Categories.Remove(category);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<Category> FindByNameAsync(string categoryName)
		{
			return await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == categoryName);
		}

		public async Task<IEnumerable<ViewCategoryDTO>> GetDishesByCategoryNameAsync(string categoryName)
		{
			return await _context.Dishes
				.Where(d => d.Category.CategoryName == categoryName)
				.Select(d => new ViewCategoryDTO
				{
					DishId = d.DishId,
					ItemName = d.ItemName,
					ItemDescription = d.ItemDescription,
					Price = d.Price,
					ImageUrl = d.ImageUrl,
					CategoryName = d.Category.CategoryName
				})
				.ToListAsync();
		}
	}
}
