using EHM_API.DTOs.DishDTO;
using EHM_API.DTOs.HomeDTO;
using EHM_API.Enums;
using EHM_API.Enums.EHM_API.Models;
using EHM_API.Models;
using EHM_API.Services;
using Microsoft.AspNetCore.Mvc;
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

        public DishController(IDishService dishService)
        {
            _dishService = dishService;
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

            var result = await _dishService.GetDishesAsync(search, page, pageSize);

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
        public async Task<ActionResult> PostDish(CreateDishDTO createDishDTO)
        {
            string message = "";

            if (string.IsNullOrEmpty(createDishDTO.ItemName))
            {
                message = "Not empty";
                return BadRequest(new { message });
            }
            if (createDishDTO.ItemName.Length > 100)
            {
                message = "The dish name cannot exceed 100 characters";
                return BadRequest(new { message });
            }
            if (createDishDTO.Price < 0)
            {
                message = "Price cannot be negative";
                return BadRequest(new { message });
            }
            var existingDishes = await _dishService.SearchDishesAsync(createDishDTO.ItemName);
            if (existingDishes.Any())
            {
                message = "The dish name already exists";
                return BadRequest(new { message });
            }
            if (createDishDTO.ItemDescription?.Length > 500)
            {
                message = "Food description must not exceed 500 characters";
                return BadRequest(new { message });
            }

            var createdDish = await _dishService.CreateDishAsync(createDishDTO);

            message = "The dish has been created successfully";
            return Ok(new
            {
                message,
                createdDish
            });
        }

        [HttpPut("{dishId}")]
        public async Task<IActionResult> PutDish(int dishId, UpdateDishDTO updateDishDTO)
        {
            string message = "";
            var existingDish = await _dishService.GetDishByIdAsync(dishId);
            var updatedDish = await _dishService.UpdateDishAsync(dishId, updateDishDTO);

            if (string.IsNullOrEmpty(updateDishDTO.ItemName))
            {
                message = "Not empty";
                return BadRequest(new { message });
            }
            if (updateDishDTO.ItemName.Length > 100)
            {
                message = "The dish name cannot exceed 100 characters";
                return BadRequest(new { message });
            }
            if (updateDishDTO.Price < 0)
            {
                message = "Price cannot be negative";
                return BadRequest(new { message });
            }
            
            if (updateDishDTO.ItemDescription?.Length > 500)
            {
                message = "Food description must not exceed 500 characters";
                return BadRequest(new { message });
            }
            message = "The dish has been successfully updated";
            return Ok(new
            {
                message,
                updatedDish
            });
        }

      

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<DishDTOAll>>> SearchDishes([FromQuery] string? name)
        {
            if (name == null)
            {
                return BadRequest("Not empty");
            }
            var dishes = await _dishService.SearchDishesAsync(name);
            return Ok(dishes);
        }

        [HttpGet("sorted")]
        public async Task<ActionResult<IEnumerable<DishDTOAll>>> GetDishesSorted([FromQuery] SortField sortField, [FromQuery] SortOrder sortOrder)
        {
            IEnumerable<DishDTOAll> sortedDishes;

            switch (sortField)
            {
                case SortField.Name:
                    sortedDishes = await _dishService.GetAllSortedAsync(SortField.Name, sortOrder);
                    break;
                case SortField.Price:
                    sortedDishes = await _dishService.GetAllSortedAsync(SortField.Price, sortOrder);
                    break;
                default:
                    return BadRequest("Invalid sort field.");
            }

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
