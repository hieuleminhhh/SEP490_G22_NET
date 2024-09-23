using AutoMapper;
using EHM_API.DTOs;
using EHM_API.DTOs.CategoryDTO.Guest;
using EHM_API.DTOs.CategoryDTO.Manager;
using EHM_API.Models;
using EHM_API.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public class CategoryService : ICategoryService
	{
		private readonly ICategoryRepository _categoryRepository;
		private readonly IMapper _mapper;

		public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
		{
			_categoryRepository = categoryRepository;
			_mapper = mapper;
		}

		public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
		{
			var categories = await _categoryRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
		}

		public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
		{
			var category = await _categoryRepository.GetByIdAsync(id);
			if (category == null)
			{
				return null;
			}
			return _mapper.Map<CategoryDTO>(category);
		}

		public async Task<CategoryDTO> CreateCategoryAsync(CreateCategory categoryDTO)
		{
			if (string.IsNullOrEmpty(categoryDTO.CategoryName))
			{
				throw new ArgumentException("Category name cannot be null or empty.");
			}


			var existingCategory = await _categoryRepository.FindByNameAsync(categoryDTO.CategoryName);
			if (existingCategory != null)
			{
				throw new InvalidOperationException("Category name must be unique.");
			}

			var category = _mapper.Map<Category>(categoryDTO);
			var createdCategory = await _categoryRepository.AddAsync(category);
			return _mapper.Map<CategoryDTO>(createdCategory);
		}


        public async Task<CategoryDTO> UpdateCategoryAsync(int id, CategoryDTO categoryDTO)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return null; // Danh mục không tồn tại
            }

            // Kiểm tra xem danh mục có món ăn nào không
            var hasDishes = existingCategory.Dishes != null && existingCategory.Dishes.Any();
            if (hasDishes)
            {
                throw new InvalidOperationException("Không thể cập nhật danh mục khi danh mục đã chứa món ăn.");
            }

            // Nếu danh mục không có món ăn, tiến hành cập nhật
            _mapper.Map(categoryDTO, existingCategory);

            var updatedCategory = await _categoryRepository.UpdateAsync(existingCategory);
            return _mapper.Map<CategoryDTO>(updatedCategory);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return false; // Danh mục không tồn tại
            }

            // Kiểm tra xem danh mục có món ăn nào không
            if (category.Dishes != null && category.Dishes.Any())
            {
                throw new InvalidOperationException("Không thể xóa danh mục vì đã có món ăn liên kết.");
            }

            return await _categoryRepository.DeleteAsync(id);
        }


        public async Task<CategoryDTO> GetCategoryByNameAsync(string categoryName)
		{
			var category = await _categoryRepository.FindByNameAsync(categoryName);
			if (category == null)
			{
				return null;
			}
			return _mapper.Map<CategoryDTO>(category);
		}

		public async Task<IEnumerable<ViewCategoryDTO>> GetDishesByCategoryNameAsync(string categoryName)
		{
			return await _categoryRepository.GetDishesByCategoryNameAsync(categoryName);
		}
	}
}
