using AutoMapper;
using EHM_API.DTOs.ComboDTO.Guest;
using EHM_API.DTOs.DishDTO.Manager;
using EHM_API.DTOs.HomeDTO;
using EHM_API.Enums;
using EHM_API.Enums.EHM_API.Models;
using EHM_API.Models;
using EHM_API.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
	public class DishService : IDishService
	{
		private readonly IDishRepository _dishRepository;
		private readonly IMapper _mapper;
		private readonly EHMDBContext _context;

		public DishService(IDishRepository dishRepository, IMapper mapper, EHMDBContext context)
		{
			_dishRepository = dishRepository;
			_mapper = mapper;
			_context = context;
		}

		public async Task<IEnumerable<DishDTOAll>> GetAllDishesAsync()
		{
			var dishes = await _dishRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<DishDTOAll>>(dishes);
		}

		public async Task<DishDTOAll> GetDishByIdAsync(int id)
		{
			var dish = await _dishRepository.GetByIdAsync(id);
			if (dish == null)
			{
				return null;
			}
			return _mapper.Map<DishDTOAll>(dish);
		}

		public async Task<bool> DishExistsAsync(int dishId)
		{
			return await _dishRepository.DishExistsAsync(dishId);
		}

		public async Task<bool> DishNameExistsAsync(string itemName)
		{
			return await _dishRepository.DishNameExistsAsync(itemName);
		}

		public async Task<DishDTOAll> CreateDishAsync(CreateDishDTO createDishDTO)
		{
			var dish = _mapper.Map<Dish>(createDishDTO);
			if (dish.Price.HasValue)
			{
				dish.Price = Math.Round(dish.Price.Value, 2);
			}
			var createdDish = await _dishRepository.AddAsync(dish);
			return _mapper.Map<DishDTOAll>(createdDish);
		}

		public async Task<DishDTOAll> UpdateDishAsync(int id, UpdateDishDTO updateDishDTO)
		{
			var existingDish = await _dishRepository.GetByIdAsync(id);
			if (existingDish == null)
			{
				return null;
			}

			_mapper.Map(updateDishDTO, existingDish);
			if (existingDish.Price.HasValue)
			{
				existingDish.Price = Math.Round(existingDish.Price.Value, 2);
			}
			var updatedDish = await _dishRepository.UpdateDishe(existingDish);
			return _mapper.Map<DishDTOAll>(updatedDish);
		}



		public async Task<IEnumerable<DishDTOAll>> SearchDishesAsync(string name)
		{
			var dishes = await _dishRepository.SearchAsync(name);
			return _mapper.Map<IEnumerable<DishDTOAll>>(dishes);
		}



		public async Task<IEnumerable<DishDTOAll>> GetAllSortedAsync(SortField? sortField, SortOrder? sortOrder)
		{
			var dishes = await _dishRepository.GetAllSortedAsync(sortField, sortOrder);
			return _mapper.Map<IEnumerable<DishDTOAll>>(dishes);
		}
		public async Task<IEnumerable<DishDTOAll>> GetSortedDishesByCategoryAsync(string? categoryName, SortField? sortField, SortOrder? sortOrder)
		{
			var dishes = await _dishRepository.GetSortedDishesByCategoryAsync(categoryName, sortField, sortOrder);
			return _mapper.Map<IEnumerable<DishDTOAll>>(dishes);
		}


		public async Task<PagedResult<DishDTOAll>> GetDishesAsync(string search, string categorySearch, int page, int pageSize)
		{
			var pagedDishes = await _dishRepository.GetDishesAsync(search, categorySearch, page, pageSize);
			var dishDTOs = _mapper.Map<IEnumerable<DishDTOAll>>(pagedDishes.Items);

			foreach (var dishDto in dishDTOs)
			{
				if (dishDto.CategoryId.HasValue)
				{
					var category = await _context.Categories.FindAsync(dishDto.CategoryId.Value);
					if (category != null)
					{
						dishDto.CategoryName = category.CategoryName;
					}
				}
			}

			return new PagedResult<DishDTOAll>(dishDTOs, pagedDishes.TotalCount, pagedDishes.Page, pagedDishes.PageSize);
		}

		public async Task<PagedResult<DishDTOAll>> GetDishesActive(string search, string categorySearch, int page, int pageSize)
		{
			var pagedDishes = await _dishRepository.GetDishesActive(search, categorySearch, page, pageSize);
			var dishDTOs = _mapper.Map<IEnumerable<DishDTOAll>>(pagedDishes.Items);

			foreach (var dishDto in dishDTOs)
			{
				if (dishDto.CategoryId.HasValue)
				{
					var category = await _context.Categories.FindAsync(dishDto.CategoryId.Value);
					if (category != null)
					{
						dishDto.CategoryName = category.CategoryName;
					}
				}
			}

			return new PagedResult<DishDTOAll>(dishDTOs, pagedDishes.TotalCount, pagedDishes.Page, pagedDishes.PageSize);
		}


		public async Task<Dish> UpdateDishStatusAsync(int dishId, bool isActive)
		{
			return await _dishRepository.UpdateDishStatusAsync(dishId, isActive);
		}

		public async Task<bool> DiscountExistsAsync(int discountId)
		{
			return await _dishRepository.DiscountExistsAsync(discountId);
		}


		public async Task<SearchDishAndComboDTO> SearchDishAndComboAsync(string search)
		{
			var dishes = await _dishRepository.SearchDishesAsync(search);
			var combos = await _dishRepository.SearchCombosAsync(search);

			return new SearchDishAndComboDTO
			{
				Dishes = _mapper.Map<List<SearchDishDTO>>(dishes),
				Combos = _mapper.Map<List<SearchComboDTO>>(combos)
			};
		}
		public async Task<IEnumerable<DishDTOAll>> UpdateDiscountForDishesAsync(int discountId, List<int> dishIds)
		{
			var dishes = await _dishRepository.UpdateDiscountForDishesAsync(discountId, dishIds);
			return _mapper.Map<IEnumerable<DishDTOAll>>(dishes);
		}
        public async Task<bool> DeleteDishWithDependenciesAsync(int dishId)
        {
            var dish = await _dishRepository.GetDishByIdAsync(dishId);
            if (dish == null)
            {
                return false; // Không tồn tại
            }

            if (_dishRepository.DishExistsInOrderDetail(dishId))
            {
                return false; // Đang được tham chiếu trong OrderDetail
            }

            await _dishRepository.DeleteIngredientsByDishIdAsync(dishId);
            await _dishRepository.DeleteComboDetailsByDishIdAsync(dishId);
            await _dishRepository.DeleteDishAsync(dishId);

            return true; // Xóa thành công
        }

        public async Task UpdateQuantityDishAsync(UpdateDishQuantityDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "DTO cannot be null.");
            }

            // Kiểm tra món ăn có tồn tại không
            var dish = await _dishRepository.GetByIdAsync(dto.DishId);
            if (dish == null)
            {
                throw new Exception("Món ăn không tồn tại.");
            }

            // Cập nhật số lượng món ăn
            dish.QuantityDish = dto.QuantityDish;

            // Lưu thay đổi vào database
            await _dishRepository.Update1Async(dish);
        }

    }
}
