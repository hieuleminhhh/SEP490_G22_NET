using EHM_API.DTOs;
using EHM_API.DTOs.CategoryDTO.Guest;
using EHM_API.DTOs.CategoryDTO.Manager;
using EHM_API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHM_API.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class CategoryController : ControllerBase
	{
		private readonly ICategoryService _categoryService;

		public CategoryController(ICategoryService categoryService)
		{
			_categoryService = categoryService;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
		{
			var categories = await _categoryService.GetAllCategoriesAsync();
			return Ok(categories);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
		{
			var category = await _categoryService.GetCategoryByIdAsync(id);
			if (category == null)
			{
				return NotFound();
			}
			return Ok(category);
		}

		[HttpPost]
		public async Task<ActionResult<CategoryDTO>> PostCategory([FromBody] CreateCategory categoryDTO)
		{
			if (categoryDTO == null || string.IsNullOrWhiteSpace(categoryDTO.CategoryName))
			{
				return BadRequest("Category name is required.");
			}

			var categoryName = categoryDTO.CategoryName.Trim();

			if (categoryName.Length > 100)
			{
				return BadRequest("Category name must be less than 100 characters.");
			}

			if (!categoryName.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-' || c == '_'))
			{
				return BadRequest("Category name contains invalid characters.");
			}

			var existingCategory = await _categoryService.GetCategoryByNameAsync(categoryName);
			if (existingCategory != null)
			{
				return Conflict("Category name already exists.");
			}

			try
			{
				var createdCategory = await _categoryService.CreateCategoryAsync(categoryDTO);
				return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.CategoryId }, createdCategory);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(ex.Message);
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> PutCategory(int id, [FromBody] CategoryDTO categoryDTO)
		{
			if (categoryDTO == null || string.IsNullOrWhiteSpace(categoryDTO.CategoryName))
			{
				return BadRequest("Category name is required.");
			}

			var categoryName = categoryDTO.CategoryName.Trim();

			if (categoryName.Length > 100)
			{
				return BadRequest("Category name must be less than 100 characters.");
			}

			if (!categoryName.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-' || c == '_'))
			{
				return BadRequest("Category name contains invalid characters.");
			}

			var existingCategory = await _categoryService.GetCategoryByIdAsync(id);
			if (existingCategory == null)
			{
				return NotFound();
			}


			var duplicateCategory = await _categoryService.GetCategoryByNameAsync(categoryName);
			if (duplicateCategory != null && duplicateCategory.CategoryId != id)
			{
				return Conflict("Category name already exists.");
			}

			try
			{
				var updatedCategory = await _categoryService.UpdateCategoryAsync(id, categoryDTO);
				if (updatedCategory == null)
				{
					return NotFound();
				}

				return NoContent();
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(ex.Message);
			}
		}

		[HttpGet("dishes/{categoryName}")]
		public async Task<ActionResult<IEnumerable<ViewCategoryDTO>>> GetDishesByCategoryName(string categoryName)
		{
			var dishes = await _categoryService.GetDishesByCategoryNameAsync(categoryName);
			if (dishes == null || !dishes.Any())
			{
				return NotFound("No dishes found for the specified category.");
			}
			return Ok(dishes);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCategory(int id)
		{
			var result = await _categoryService.DeleteCategoryAsync(id);
			if (!result)
			{
				return NotFound();
			}
			return NoContent();
		}
	}
}
