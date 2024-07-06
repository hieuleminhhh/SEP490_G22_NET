﻿using EHM_API.DTOs.DishDTO.Manager;
using EHM_API.DTOs.HomeDTO;
using EHM_API.Enums;
using EHM_API.Enums.EHM_API.Models;
using EHM_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public interface IDishService
    {
        Task<DishDTOAll> GetDishByIdAsync(int id);
        Task<DishDTOAll> CreateDishAsync(CreateDishDTO createDishDTO);
        Task<DishDTOAll> UpdateDishAsync(int id, UpdateDishDTO updateDishDTO);

        Task<IEnumerable<DishDTOAll>> SearchDishesAsync(string name);
        Task<IEnumerable<DishDTOAll>> GetAllDishesAsync();
        Task<IEnumerable<DishDTOAll>> GetAllSortedAsync(SortField? sortField, SortOrder? sortOrder);
        Task<IEnumerable<DishDTOAll>> GetSortedDishesByCategoryAsync(string? categoryName, SortField? sortField, SortOrder? sortOrder);
        Task<PagedResult<DishDTOAll>> GetDishesAsync(string search, string categorySearch, int page, int pageSize);
        Task<Dish> UpdateDishStatusAsync(int dishId, bool isActive);


		Task<bool> DishExistsAsync(int dishId);
		Task<bool> DiscountExistsAsync(int discountId);
	}
}
