﻿using EHM_API.DTOs.DishDTO.Manager;
using EHM_API.DTOs.HomeDTO;
using EHM_API.Enums;
using EHM_API.Enums.EHM_API.Models;
using EHM_API.Models;
using EHM_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;
        private readonly EHMDBContext _context;
        public DishController(IDishService dishService, EHMDBContext context)
        {
            _dishService = dishService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DishDTOAll>>> GetDishes()
        {
            var dishes = await _dishService.GetAllDishesAsync();
            return Ok(dishes);
        }

        [HttpGet("ListDishes")]
        public async Task<ActionResult<PagedResult<DishDTOAll>>> GetListDishes(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var result = await _dishService.GetDishesAsync(search?.Trim(), page, pageSize);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DishDTOAll>> GetDish(int id)
        {
            var dish = await _dishService.GetDishByIdAsync(id);
            if (dish == null)
            {
                return NotFound();
            }
            return Ok(dish);
        }

        [HttpPost]
        public async Task<ActionResult> CreateNewDish(CreateDishDTO createDishDTO)
        {
            var errors = new Dictionary<string, string>();

            createDishDTO.ItemName = createDishDTO.ItemName?.Trim();
            createDishDTO.ItemDescription = createDishDTO.ItemDescription?.Trim();
            createDishDTO.ImageUrl = createDishDTO.ImageUrl?.Trim();

            if (string.IsNullOrEmpty(createDishDTO.ItemName))
            {
                errors["itemName"] = "Tên món ăn không được để trống";
            }
            else if (createDishDTO.ItemName.Length > 100)
            {
                errors["itemName"] = "Tên món ăn không được vượt quá 100 ký tự";
            }
            else
            {
                var existingDishes = await _dishService.SearchDishesAsync(createDishDTO.ItemName);
                if (existingDishes.Any())
                {
                    errors["itemName"] = "Tên món ăn đã tồn tại";
                }
            }

            if (!createDishDTO.Price.HasValue)
            {
                errors["price"] = "Giá của món ăn không được để trống";
            }
            else if (createDishDTO.Price < 0 || createDishDTO.Price > 1000000000)
            {
                errors["price"] = "Giá phải nằm trong khoảng từ 0 đến 1.000.000.000";
            }

            if (string.IsNullOrEmpty(createDishDTO.ItemDescription))
            {
                errors["itemDescription"] = "Mô tả không được để trống";
            }
            else if (createDishDTO.ItemDescription.Length > 500)
            {
                errors["itemDescription"] = "Mô tả món ăn không được vượt quá 500 ký tự";
            }

            if (!createDishDTO.CategoryId.HasValue)
            {
                errors["categoryId"] = "Danh mục món ăn không được để trống";
            }
            else
            {
                var category = await _context.Categories.FindAsync(createDishDTO.CategoryId.Value);
                if (category == null)
                {
                    errors["categoryId"] = "Danh mục món ăn không tồn tại";
                }
            }

            if (string.IsNullOrEmpty(createDishDTO.ImageUrl))
            {
                errors["image"] = "Hình ảnh không được để trống";
            }
            else
            {
                string extension = Path.GetExtension(createDishDTO.ImageUrl).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                if (!allowedExtensions.Contains(extension))
                {
                    errors["image"] = "Hình ảnh không hợp lệ. Chỉ cho phép các tệp JPG, JPEG, PNG, GIF.";
                }
            }


            if (errors.Any())
            {
                return BadRequest(errors);
            }

            var createdDish = await _dishService.CreateDishAsync(createDishDTO);

            return Ok(new
            {
                message = "The dish has been created successfully",
                createdDish
            });
        }

        [HttpPut("{dishId}")]
        public async Task<IActionResult> UpdateDish(int dishId, UpdateDishDTO updateDishDTO)
        {
            var errors = new Dictionary<string, string>();

            var existingDish = await _dishService.GetDishByIdAsync(dishId);
            if (existingDish == null)
            {
                return NotFound(new { message = "Dish not found" });
            }

            updateDishDTO.ItemName = updateDishDTO.ItemName?.Trim();
            updateDishDTO.ItemDescription = updateDishDTO.ItemDescription?.Trim();
            updateDishDTO.ImageUrl = updateDishDTO.ImageUrl?.Trim();

            if (string.IsNullOrEmpty(updateDishDTO.ItemName))
            {
                errors["itemName"] = "Item name is required";
            }
            else if (updateDishDTO.ItemName.Length > 100)
            {
                errors["itemName"] = "The dish name cannot exceed 100 characters";
            }
            else
            {
                var existingDishes = await _dishService.SearchDishesAsync(updateDishDTO.ItemName);
                if (existingDishes.Any(d => d.DishId != dishId))
                {
                    errors["itemName"] = "The dish name already exists";
                }
            }

            if (!updateDishDTO.Price.HasValue)
            {
                errors["price"] = "Price is required";
            }
            else if (updateDishDTO.Price < 0 || updateDishDTO.Price > 1000000000)
            {
                errors["price"] = "Price must be between 0 and 1,000,000,000";
            }

            if (string.IsNullOrEmpty(updateDishDTO.ItemDescription))
            {
                errors["itemDescription"] = "Item description is required";
            }
            else if (updateDishDTO.ItemDescription.Length > 500)
            {
                errors["itemDescription"] = "Food description must not exceed 500 characters";
            }

            if (!updateDishDTO.CategoryId.HasValue)
            {
                errors["categoryId"] = "Category is required";
            }
            else
            {
                var category = await _context.Categories.FindAsync(updateDishDTO.CategoryId.Value);
                if (category == null)
                {
                    errors["categoryId"] = "Invalid category";
                }
            }

            if (string.IsNullOrEmpty(updateDishDTO.ImageUrl))
            {
                errors["image"] = "Image is required";
            }

            if (errors.Any())
            {
                return BadRequest(errors);
            }

            var updatedDish = await _dishService.UpdateDishAsync(dishId, updateDishDTO);
            if (updatedDish == null)
            {
                return StatusCode(500, new { message = "An error occurred while updating the dish" });
            }

            var message = "The dish has been successfully updated";
            return Ok(new
            {
                message,
                updatedDish
            });
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<DishDTOAll>>> SearchDishes([FromQuery] string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Search term cannot be empty");
            }
            var dishes = await _dishService.SearchDishesAsync(name.Trim());
            return Ok(dishes);
        }


        [HttpGet("sorted-dishes")]
        public async Task<IActionResult> GetSortedDishesByCategoryAsync(string? categoryName, SortField? sortField, SortOrder? sortOrder)
        {
            if (string.IsNullOrEmpty(categoryName) && !sortField.HasValue && !sortOrder.HasValue)
            {
                var allDishes = await _dishService.GetAllDishesAsync();
                return Ok(allDishes);
            }

            if (string.IsNullOrEmpty(categoryName))
            {
                var dishes = await _dishService.GetAllSortedAsync(sortField.Value, sortOrder.Value);
                return Ok(dishes);
            }
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == categoryName);
            if (category == null)
            {
                return NotFound("Category not found.");
            }

            var sortedDishes = await _dishService.GetSortedDishesByCategoryAsync(categoryName, sortField, sortOrder);
            return Ok(sortedDishes);
        }



        [HttpPatch("{dishId}/status")]
        public async Task<IActionResult> UpdateDishStatus(int dishId, [FromBody] UpdateDishStatusDTO updateDishStatusDTO)
        {
            if (updateDishStatusDTO == null)
            {
                return BadRequest(new { message = "Invalid data" });
            }

            var existingDish = await _dishService.GetDishByIdAsync(dishId);
            if (existingDish == null)
            {
                return NotFound(new { message = "Dish not found" });
            }

            var updatedDish = await _dishService.UpdateDishStatusAsync(dishId, updateDishStatusDTO.IsActive);
            if (updatedDish == null)
            {
                return StatusCode(500, new { message = "An error occurred while updating the dish status" });
            }

            return Ok(new
            {
                message = "Dish status updated successfully",                
            });
        }

    }
}
