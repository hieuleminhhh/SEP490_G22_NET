using EHM_API.DTOs;
using EHM_API.DTOs.CategoryDTO.Guest;
using EHM_API.DTOs.CategoryDTO.Manager;
using EHM_API.Services;
using Microsoft.AspNetCore.Authorization;
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

		[Authorize(Roles = "Manager")]
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

        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryDTO categoryDTO)
        {
            var errors = new Dictionary<string, string>();

            // Kiểm tra xem categoryDTO có null hoặc CategoryName có trống không
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

                if (!categoryName.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-' || c == '_'))
                {
                    errors["categoryName"] = "Tên danh mục chứa các ký tự không hợp lệ.";
                }
            }

            if (errors.Any())
            {
                return BadRequest(errors);
            }

            // Kiểm tra nếu CategoryId có giá trị
            if (!categoryDTO.CategoryId.HasValue)
            {
                return BadRequest(new { message = "CategoryId là bắt buộc." });
            }

            var existingCategory = await _categoryService.GetCategoryByIdAsync(categoryDTO.CategoryId.Value);
            if (existingCategory == null)
            {
                return NotFound(new { message = "Không tìm thấy danh mục." });
            }

            // Kiểm tra tên danh mục trùng lặp
            var duplicateCategory = await _categoryService.GetCategoryByNameAsync(categoryDTO.CategoryName);
            if (duplicateCategory != null && duplicateCategory.CategoryId != categoryDTO.CategoryId)
            {
                return Conflict(new { message = "Tên danh mục đã tồn tại." });
            }

            try
            {
                var updatedCategory = await _categoryService.UpdateCategoryAsync(categoryDTO.CategoryId.Value, categoryDTO);
                if (updatedCategory == null)
                {
                    return NotFound(new { message = "Không tìm thấy danh mục sau khi cập nhật." });
                }

                return Ok(new { message = "Tên danh mục món ăn được cập nhật thành công", updatedCategory });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật danh mục." });
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Không tìm thấy danh mục." });
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa danh mục." });
            }
        }





    }
}
