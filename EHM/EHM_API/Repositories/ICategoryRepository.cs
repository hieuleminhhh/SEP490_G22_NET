using EHM_API.DTOs.CategoryDTO.Guest;
using EHM_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Repositories
{
    public interface ICategoryRepository
	{
		Task<IEnumerable<Category>> GetAllAsync();
		Task<Category> GetByIdAsync(int id);
		Task<Category> AddAsync(Category category);
		Task<Category> UpdateAsync(Category category);
		Task<bool> DeleteAsync(int id);
		Task<Category> FindByNameAsync(string categoryName);

		Task<IEnumerable<ViewCategoryDTO>> GetDishesByCategoryNameAsync(string categoryName);
	}
}
