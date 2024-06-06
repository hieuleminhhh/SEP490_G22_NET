using AutoMapper;
using EHM_API.DTOs.DishDTO;
using EHM_API.DTOs.HomeDTO;
using EHM_API.Enums;
using EHM_API.Enums.EHM_API.Models;
using EHM_API.Models;
using EHM_API.Repositories;
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

        public async Task<DishDTOAll> CreateDishAsync(CreateDishDTO createDishDTO)
        {
            var dish = _mapper.Map<Dish>(createDishDTO);
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
            var updatedDish = await _dishRepository.UpdateAsync(existingDish);
            return _mapper.Map<DishDTOAll>(updatedDish);
        }

     

        public async Task<IEnumerable<DishDTOAll>> SearchDishesAsync(string name)
        {
            var dishes = await _dishRepository.SearchAsync(name);
            return _mapper.Map<IEnumerable<DishDTOAll>>(dishes);
        }

        public async Task<IEnumerable<DishDTOAll>> GetAllSortedAsync(SortField sortField, SortOrder sortOrder)
        {
            var dishes = await _dishRepository.GetAllSortedAsync(sortField, sortOrder);
            return await MapDishesToDTOs(dishes);
        }

        private async Task<DishDTOAll> MapDishToDTO(Dish dish)
        {
            var dishDTO = _mapper.Map<DishDTOAll>(dish);

            if (dishDTO.CategoryId.HasValue)
            {
                var category = await _context.Categories.FindAsync(dishDTO.CategoryId.Value);
                if (category != null)
                {
                    dishDTO.CategoryName = category.CategoryName;
                }
            }

            if (dish.Discount != null && dish.Price.HasValue)
            {
                dishDTO.DiscountPercentage = dish.Discount.DiscountAmount;
                dishDTO.DiscountedPrice = dish.Price - (dish.Price * dish.Discount.DiscountAmount / 100);
            }

            return dishDTO;
        }

        private async Task<IEnumerable<DishDTOAll>> MapDishesToDTOs(IEnumerable<Dish> dishes)
        {
            var dishDTOs = new List<DishDTOAll>();
            foreach (var dish in dishes)
            {
                dishDTOs.Add(await MapDishToDTO(dish));
            }
            return dishDTOs;
        }
        public async Task<PagedResult<DishDTOAll>> GetDishesAsync(string search, int page, int pageSize)
        {
            var pagedDishes = await _dishRepository.GetDishesAsync(search, page, pageSize);
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

    }
}
