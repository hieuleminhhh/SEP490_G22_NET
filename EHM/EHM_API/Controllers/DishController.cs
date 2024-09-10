using EHM_API.DTOs.ComboDTO.Guest;
using EHM_API.DTOs.DishDTO.Manager;
using EHM_API.DTOs.HomeDTO;
using EHM_API.Enums;
using EHM_API.Enums.EHM_API.Models;
using EHM_API.Models;
using EHM_API.Services;
using Microsoft.AspNetCore.Authorization;
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
			[FromQuery] string? search = null,
			[FromQuery] string? searchCategory = null)
		{
			if (page <= 0) page = 1;
			if (pageSize <= 0) pageSize = 10;

			var result = await _dishService.GetDishesAsync(search?.Trim(), searchCategory, page, pageSize);

			return Ok(result);
		}

		[HttpGet("ListDishesActive")]
		public async Task<ActionResult<PagedResult<DishDTOAll>>> GetListDishesActive(
		 [FromQuery] int page = 1,
		 [FromQuery] int pageSize = 10,
		 [FromQuery] string? search = null,
		 [FromQuery] string? searchCategory = null)
		{
			if (page <= 0) page = 1;
			if (pageSize <= 0) pageSize = 10;

			var result = await _dishService.GetDishesActive(search?.Trim(), searchCategory, page, pageSize);

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


		[Authorize(Roles = "Manager")]
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
			else if (await _dishService.DishNameExistsAsync(createDishDTO.ItemName))
			{
				errors["itemName"] = "Tên món ăn đã tồn tại";
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

			if (createDishDTO.DiscountId.HasValue)
			{
				var discountExists = await _dishService.DiscountExistsAsync(createDishDTO.DiscountId.Value);
				if (!discountExists)
				{
					errors["discountId"] = "Mã giảm giá không tồn tại";
				}
			}


			if (errors.Any())
			{
				return BadRequest(errors);
			}

			var createdDish = await _dishService.CreateDishAsync(createDishDTO);

			return Ok(new
			{
				message = "Món ăn đã được tạo thành công",
				createdDish
			});
		}


		[Authorize(Roles = "Manager")]
		[HttpPut("{dishId}")]
		public async Task<IActionResult> UpdateDish(int dishId, UpdateDishDTO updateDishDTO)
		{
			var errors = new Dictionary<string, string>();

			var existingDish = await _dishService.GetDishByIdAsync(dishId);
			if (existingDish == null)
			{
				return NotFound(new { message = "Không tìm thấy món ăn" });
			}

			updateDishDTO.ItemName = updateDishDTO.ItemName?.Trim();
			updateDishDTO.ItemDescription = updateDishDTO.ItemDescription?.Trim();
			updateDishDTO.ImageUrl = updateDishDTO.ImageUrl?.Trim();

			if (string.IsNullOrEmpty(updateDishDTO.ItemName))
			{
				errors["itemName"] = "Tên món ăn không được để trống";
			}
			else if (updateDishDTO.ItemName.Length > 100)
			{
				errors["itemName"] = "Tên món ăn không được vượt quá 100 ký tự";
			}
			else
			{
				var existingDishes = await _dishService.SearchDishesAsync(updateDishDTO.ItemName);
				if (existingDishes.Any(d => d.DishId != dishId))
				{
					errors["itemName"] = "Tên món ăn đã tồn tại";
				}
			}

			if (!updateDishDTO.Price.HasValue)
			{
				errors["price"] = "Giá của món ăn không được để trống";
			}
			else if (updateDishDTO.Price < 0 || updateDishDTO.Price > 1000000000)
			{
				errors["price"] = "Giá phải nằm trong khoảng từ 0 đến 1.000.000.000";
			}

			if (string.IsNullOrEmpty(updateDishDTO.ItemDescription))
			{
				errors["itemDescription"] = "Mô tả không được để trống";
			}
			else if (updateDishDTO.ItemDescription.Length > 500)
			{
				errors["itemDescription"] = "Mô tả món ăn không được vượt quá 500 ký tự";
			}

			if (!updateDishDTO.CategoryId.HasValue)
			{
				errors["categoryId"] = "Danh mục món ăn không được để trống";
			}
			else
			{
				var category = await _context.Categories.FindAsync(updateDishDTO.CategoryId.Value);
				if (category == null)
				{
					errors["categoryId"] = "Danh mục món ăn không tồn tại";
				}
			}

			if (string.IsNullOrEmpty(updateDishDTO.ImageUrl))
			{
				errors["image"] = "Hình ảnh không được để trống";
			}
			else
			{
				string extension = Path.GetExtension(updateDishDTO.ImageUrl).ToLower();
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

			var updatedDish = await _dishService.UpdateDishAsync(dishId, updateDishDTO);
			if (updatedDish == null)
			{
				return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật món ăn" });
			}

			var message = "Món ăn đã được cập nhật thành công";
			return Ok(new
			{
				message,
				updatedDish
			});
		}

		[HttpGet("sorted-dishes")]
		public async Task<IActionResult> GetSortedDishesByCategoryAsync(string? categoryName, SortField? sortField, SortOrder? sortOrder)
		{
			try
			{
				if (string.IsNullOrEmpty(categoryName) && !sortField.HasValue && !sortOrder.HasValue)
				{
					var allDishes = await _dishService.GetAllDishesAsync();
					return Ok(new { message = "Lấy danh sách tất cả món ăn thành công.", data = allDishes });
				}

				if (string.IsNullOrEmpty(categoryName))
				{
					if (!sortField.HasValue || !sortOrder.HasValue)
					{
						return BadRequest(new { message = "Cần cung cấp cả sortField và sortOrder khi không có categoryName." });
					}
					var dishes = await _dishService.GetAllSortedAsync(sortField.Value, sortOrder.Value);
					return Ok(new { message = "Lấy danh sách món ăn đã sắp xếp thành công.", data = dishes });
				}

				var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == categoryName);
				if (category == null)
				{
					return NotFound(new { message = $"Không tìm thấy danh mục '{categoryName}'." });
				}

				var sortedDishes = await _dishService.GetSortedDishesByCategoryAsync(categoryName, sortField, sortOrder);
				return Ok(new { message = $"Lấy danh sách món ăn đã sắp xếp theo danh mục '{categoryName}' thành công.", data = sortedDishes });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Đã xảy ra lỗi trong quá trình xử lý yêu cầu. Vui lòng thử lại sau." });
			}
		}

		[Authorize(Roles = "Manager")]
		[HttpPatch("{dishId}/status")]
		public async Task<IActionResult> UpdateDishStatus(int dishId, [FromBody] UpdateDishStatusDTO updateDishStatusDTO)
		{
			try
			{
				if (updateDishStatusDTO == null)
				{
					return BadRequest(new { message = "Dữ liệu cập nhật không hợp lệ." });
				}

				var existingDish = await _dishService.GetDishByIdAsync(dishId);
				if (existingDish == null)
				{
					return NotFound(new { message = $"Không tìm thấy món ăn với ID {dishId}." });
				}

				var updatedDish = await _dishService.UpdateDishStatusAsync(dishId, updateDishStatusDTO.IsActive);
				if (updatedDish == null)
				{
					return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật trạng thái món ăn. Vui lòng thử lại sau." });
				}

				string statusMessage = updateDishStatusDTO.IsActive ? "Kích hoạt" : "Không kích hoạt";
				return Ok(new
				{
					message = $"Trạng thái món ăn đã được {statusMessage} thành công.",
					data = updatedDish
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại sau." });
			}
		}

		[HttpGet("search")]
		public async Task<ActionResult<IEnumerable<DishDTOAll>>> SearchDishes([FromQuery] string? name)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(name))
				{
					return BadRequest(new { message = "Từ khóa tìm kiếm không được để trống." });
				}

				var dishes = await _dishService.SearchDishesAsync(name.Trim());

				if (dishes == null || !dishes.Any())
				{
					return NotFound(new { message = "Không tìm thấy món ăn nào phù hợp với từ khóa." });
				}

				return Ok(new { message = "Tìm kiếm thành công.", data = dishes });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Đã xảy ra lỗi trong quá trình tìm kiếm. Vui lòng thử lại sau." });
			}
		}


		[Authorize(Roles = "Manager")]
		[HttpPut("{discountId}/dishes")]
		public async Task<ActionResult<IEnumerable<DishDTO>>> UpdateDiscountForDishesAsync(int discountId, [FromBody] List<int> dishIds)
		{
			try
			{
				var dishes = await _dishService.UpdateDiscountForDishesAsync(discountId, dishIds);
				return Ok(dishes);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("searchDishAndCombo")]
		public async Task<ActionResult<SearchDishAndComboDTO>> SearchDishAndCombo([FromQuery] string search)
		{
			if (string.IsNullOrWhiteSpace(search))
			{
				return BadRequest("Từ khóa tìm kiếm không được để trống.");
			}

			var result = await _dishService.SearchDishAndComboAsync(search);

			if (result == null || (result.Dishes.Count == 0 && result.Combos.Count == 0))
			{
				return NotFound("Không tìm thấy kết quả nào.");
			}

			return Ok(result);
		}
        [HttpDelete("{dishId}")]
        public async Task<IActionResult> DeleteDish(int dishId)
        {
            try
            {
                bool isDeleted = await _dishService.DeleteDishWithDependenciesAsync(dishId);
                if (isDeleted)
                {
                    return Ok(new { success = true, message = "Dish and related records deleted successfully." });
                }
                return BadRequest(new { success = false, message = "Dish could not be deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpPut("update-quantity")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateQuantityDish([FromBody] UpdateDishQuantityDTO dto)
        {
            try
            {
                // Kiểm tra DTO có null hoặc giá trị không hợp lệ
                if (dto == null || dto.QuantityDish < 0)
                {
                    return BadRequest(new { message = "Số lượng món ăn không hợp lệ." });
                }

                // Gọi service để cập nhật số lượng
                await _dishService.UpdateQuantityDishAsync(dto);

                return Ok(new { message = "Số lượng món ăn đã được cập nhật thành công." });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
