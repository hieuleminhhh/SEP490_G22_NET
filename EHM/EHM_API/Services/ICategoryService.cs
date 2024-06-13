using EHM_API.DTOs;
using EHM_API.DTOs.CategoryDTO.Guest;
using EHM_API.DTOs.CategoryDTO.Manager;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public interface ICategoryService
	{
		Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
		Task<CategoryDTO> GetCategoryByIdAsync(int id);
		Task<CategoryDTO> CreateCategoryAsync(CreateCategory categoryDTO);
		Task<CategoryDTO> UpdateCategoryAsync(int id, CategoryDTO categoryDTO);
		Task<bool> DeleteCategoryAsync(int id);
		Task<CategoryDTO> GetCategoryByNameAsync(string categoryName);

		Task<IEnumerable<ViewCategoryDTO>> GetDishesByCategoryNameAsync(string categoryName);
	}
}
