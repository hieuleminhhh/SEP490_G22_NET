using EHM_API.DTOs;
using EHM_API.DTOs.CategoryDTO.Guest;
using EHM_API.DTOs.CategoryDTO.Manager;
using EHM_API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
		public async Task<ActionResult> CreateNewCategory([FromBody] CreateCategory categoryDTO)
		{
			var errors = new Dictionary<string, string>();

			if (categoryDTO == null || string.IsNullOrWhiteSpace(categoryDTO.CategoryName))
			{
				errors["categoryName"] = "Tên danh mục món ăn là bắt buộc.";
			}
			else
			{
				var categoryName = categoryDTO.CategoryName.Trim();

				if (categoryName.Length > 100)
				{
					errors["categoryName"] = "Tên danh mục phải ít hơn 100 ký tự.";
				}

				 if (!Regex.IsMatch(categoryDTO.CategoryName, @"^[\p{L}\p{M}\p{N} ]*$"))
				{
					errors["categoryName"] = "Tên danh mục chứa các ký tự không hợp lệ.";
				}

				var existingCategory = await _categoryService.GetCategoryByNameAsync(categoryName);
				if (existingCategory != null)
				{
					errors["categoryName"] = "Tên danh mục đã tồn tại.";
				}
			}

			if (errors.Any())
			{
				return BadRequest(errors);
			}

			try
			{
				var createdCategory = await _categoryService.CreateCategoryAsync(categoryDTO);
				return Ok(new { message = "Danh mục đã được tạo thành công.", createdCategory });
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
		}



		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO categoryDTO)
		{
			var errors = new Dictionary<string, string>();

			if (categoryDTO == null || string.IsNullOrWhiteSpace(categoryDTO.CategoryName))
			{
				errors["categoryName"] = "Tên danh mục món ăn là bắt buộc.";
			}
			else
			{
				var categoryName = categoryDTO.CategoryName.Trim();

				if (categoryName.Length > 100)
				{
					errors["categoryName"] = "Tên danh mục phải ít hơn 100 ký tự.";
				}

				if (!categoryName.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-' || c == '_'))
				{
					errors["categoryName"] = "Tên danh mục chứa các ký tự không hợp lệ.";
				}
			}

			if (errors.Any())
			{
				return BadRequest(errors);
			}

			var existingCategory = await _categoryService.GetCategoryByIdAsync(id);
			if (existingCategory == null)
			{
				return NotFound(new { message = "Không tìm thấy danh mục." });
			}

			var duplicateCategory = await _categoryService.GetCategoryByNameAsync(categoryDTO.CategoryName);
			if (duplicateCategory != null && duplicateCategory.CategoryId != id)
			{
				return Conflict(new { message = "Tên danh mục đã tồn tại." });
			}

			try
			{
				var updatedCategory = await _categoryService.UpdateCategoryAsync(id, categoryDTO);
				if (updatedCategory == null)
				{
					return NotFound(new { message = "Không tìm thấy danh mục sau khi cập nhật." });
				}

				return Ok(new { message = "Tên danh mục món ăn được cập nhật thành công", updatedCategory });
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
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

		[HttpGet("dishes/{categoryName}")]
		public async Task<ActionResult<IEnumerable<ViewCategoryDTO>>> GetDishesByCategoryName(string categoryName)
		{
			var dishes = await _categoryService.GetDishesByCategoryNameAsync(categoryName);
			if (dishes == null || !dishes.Any())
			{
				return NotFound("Không tìm thấy món ăn nào cho danh mục này.");
			}
			return Ok(dishes);
		}

		
	}
}
