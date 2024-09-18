using EHM_API.DTOs.ComboDTO.EHM_API.DTOs.ComboDTO;
using EHM_API.DTOs.ComboDTO.Guest;
using EHM_API.DTOs.ComboDTO.Manager;
using EHM_API.DTOs.DishDTO;
using EHM_API.DTOs.DishDTO.Manager;
using EHM_API.DTOs.HomeDTO;
using EHM_API.Enums.EHM_API.Models;
using EHM_API.Repositories;
using EHM_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ComboController : ControllerBase
	{
		private readonly IComboService _comboService;
		private readonly IDishRepository _dishRepository;

        public ComboController(IComboService comboService, IDishRepository dishRepository)
        {
            _comboService = comboService;
            _dishRepository = dishRepository;
        }

        [HttpGet]
		public async Task<ActionResult<IEnumerable<ComboDTO>>> GetCombos()
		{
			var combos = await _comboService.GetAllCombosAsync();
			return Ok(combos);
		}

		[HttpGet("{comboId}")]
		public async Task<ActionResult<ViewComboDTO>> GetComboWithDishes(int comboId)
		{
			var combo = await _comboService.GetComboWithDishesAsync(comboId);
			if (combo == null)
			{
				return NotFound();
			}
			return Ok(combo);
		}

		[HttpGet("search/{comboId}")]
		public async Task<ActionResult<ComboDTO>> GetComboById(int comboId)
		{
			var comboDTO = await _comboService.GetComboByIdAsync(comboId);
			if (comboDTO == null)
			{
				return NotFound();
			}

			return Ok(comboDTO);
		}


		[HttpGet("search")]
		public async Task<ActionResult<IEnumerable<ComboDTO>>> SearchComboByName([FromQuery] string name)
		{
			var combos = await _comboService.SearchComboByNameAsync(name);
			if (combos == null || combos.Count == 0)
			{
				return NotFound();
			}
			return Ok(combos);
		}


		[Authorize(Roles = "Manager")]
		[HttpPost]
		public async Task<ActionResult> CreateCombo([FromBody] CreateComboDTO comboDTO)
		{
			var errors = new Dictionary<string, string>();

			if (string.IsNullOrEmpty(comboDTO.NameCombo))
			{
				errors["nameCombo"] = "Tên combo là bắt buộc.";
			}
			else if (comboDTO.NameCombo.Length > 100)
			{
				errors["nameCombo"] = "Tên combo không được vượt quá 100 ký tự.";
			}
			else
			{
				var existingCombos = await _comboService.SearchComboByNameAsync(comboDTO.NameCombo);
				if (existingCombos.Any())
				{
					errors["nameCombo"] = "Tên combo đã tồn tại.";
				}
			}

			if (!comboDTO.Price.HasValue)
			{
				errors["price"] = "Giá là bắt buộc.";
			}
			else if (comboDTO.Price < 0 || comboDTO.Price > 1000000000)
			{
				errors["price"] = "Giá phải nằm trong khoảng từ 0 đến 1,000,000,000.";
			}

			if (string.IsNullOrEmpty(comboDTO.Note))
			{
				errors["note"] = "Mô tả combo là bắt buộc.";
			}
			else if (comboDTO.Note.Length > 500)
			{
				errors["note"] = "Mô tả combo không được vượt quá 500 ký tự.";
			}

			if (string.IsNullOrEmpty(comboDTO.ImageUrl))
			{
				errors["imageUrl"] = "URL hình ảnh là bắt buộc.";
			}
			else if (!Uri.IsWellFormedUriString(comboDTO.ImageUrl, UriKind.Absolute))
			{
				errors["imageUrl"] = "URL hình ảnh không hợp lệ.";
			}

			if (errors.Any())
			{
				return BadRequest(errors);
			}

			try
			{
				var createdCombo = await _comboService.CreateComboAsync(comboDTO);
				return Ok(new
				{
					message = "Combo đã được tạo thành công.",
					createdCombo
				});
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}


		[Authorize(Roles = "Manager")]
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCombo(int id, [FromBody] ComboDTO comboDTO)
		{
			var errors = new Dictionary<string, string>();

			var existingCombo = await _comboService.GetComboByIdAsync(id);
			if (existingCombo == null)
			{
				return NotFound(new { message = "Không tìm thấy combo" });
			}

			if (string.IsNullOrEmpty(comboDTO.NameCombo))
			{
				errors["nameCombo"] = "Tên combo là bắt buộc";
			}
			else if (comboDTO.NameCombo.Length > 100)
			{
				errors["nameCombo"] = "Tên combo không được vượt quá 100 ký tự";
			}
			else
			{
				var existingCombos = await _comboService.SearchComboByNameAsync(comboDTO.NameCombo);
				if (existingCombos.Any(c => c.ComboId != id))
				{
					errors["nameCombo"] = "Tên combo đã tồn tại";
				}
			}

			if (!comboDTO.Price.HasValue)
			{
				errors["price"] = "Giá là bắt buộc";
			}
			else if (comboDTO.Price < 0 || comboDTO.Price > 1000000000)
			{
				errors["price"] = "Giá phải nằm trong khoảng từ 0 đến 1,000,000,000";
			}

			if (string.IsNullOrEmpty(comboDTO.Note))
			{
				errors["note"] = "Mô tả combo là bắt buộc";
			}
			else if (comboDTO.Note.Length > 500)
			{
				errors["note"] = "Mô tả combo không được vượt quá 500 ký tự";
			}

			if (string.IsNullOrEmpty(comboDTO.ImageUrl))
			{
				errors["imageUrl"] = "URL hình ảnh là bắt buộc";
			}

			if (errors.Any())
			{
				return BadRequest(errors);
			}

			try
			{
				await _comboService.UpdateComboAsync(id, comboDTO);
				return Ok(new
				{
					message = "Combo đã được cập nhật thành công",
					comboDTO
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}

		[Authorize(Roles = "Manager")]
		[HttpPut("{id}/cancel")]
		public async Task<IActionResult> CancelCombo(int id)
		{
			try
			{
				await _comboService.CancelComboAsync(id);
				return Ok(new { message = "Combo đã được hủy thành công" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}


		[Authorize(Roles = "Manager")]
		[HttpPut("{id}/reactivate")]
		public async Task<IActionResult> ReactivateCombo(int id)
		{
			try
			{
				var result = await _comboService.ReactivateComboAsync(id);
				if (result)
				{
					return Ok(new { message = "Combo đã được kích hoạt thành công" });
				}
				return BadRequest(new { message = "Combo không thể kích hoạt lại" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}



		[HttpGet("sorted-combos")]
		public async Task<IActionResult> GetSortedCombosAsync(SortField? sortField, SortOrder? sortOrder)
		{
			if (!sortField.HasValue && !sortOrder.HasValue)
			{
				var allDishes = await _comboService.GetAllCombosAsync();
				return Ok(allDishes);
			}

			var combos = await _comboService.GetAllSortedAsync(sortField, sortOrder);
			return Ok(combos);
		}

		[HttpGet("ListCombo")]
		public async Task<ActionResult<PagedResult<ViewComboDTO>>> GetListCombo(
		[FromQuery] int page = 1,
		[FromQuery] int pageSize = 10,
		[FromQuery] string? search = null)
		{
			if (page <= 0) page = 1;
			if (pageSize <= 0) pageSize = 10;

			var result = await _comboService.GetComboAsync(search, page, pageSize);

			return Ok(result);
		}

		[HttpGet("ListComboActive")]
		public async Task<ActionResult<PagedResult<ViewComboDTO>>> GetListComboActive(
		[FromQuery] int page = 1,
		[FromQuery] int pageSize = 10,
		[FromQuery] string? search = null)
		{
			if (page <= 0) page = 1;
			if (pageSize <= 0) pageSize = 10;

			var result = await _comboService.GetComboActive(search, page, pageSize);

			return Ok(result);
		}


		[Authorize(Roles = "Manager")]
		[HttpPatch("{comboId}/status")]
		public async Task<IActionResult> UpdateDishStatus(int comboId, [FromBody] UpdateComboDTO updateCombo)
		{
			if (updateCombo == null)
			{
				return BadRequest(new { message = "Dữ liệu không hợp lệ" });
			}

			var existingCombo = await _comboService.GetComboByIdAsync(comboId);
			if (existingCombo == null)
			{
				return NotFound(new { message = "Không tìm thấy Combo" });
			}

			var updatedCombo = await _comboService.UpdateComboStatusAsync(comboId, (bool)updateCombo.IsActive);
			if (updatedCombo == null)
			{
				return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật trạng thái Combo" });
			}

			return Ok(new
			{
				message = "Trạng thái Combo được cập nhật thành công",
			});
		}

		[HttpGet("GetComboById/{id}")]
		public async Task<ActionResult<ComboDTO>> GetCombo(int id)
		{
			var combo = await _comboService.GetComboByIdAsync(id);
			if (combo == null)
			{
				return NotFound();
			}
			return Ok(combo);
		}

		[Authorize(Roles = "Manager")]
		[HttpPut("UpdateComboWithDishes/{comboId}")]
		public async Task<ActionResult<UpdateComboDishDTO>> UpdateComboWithDishes(int comboId, [FromBody] UpdateComboDishDTO updateComboWithDishesDTO)
		{
			var errors = new Dictionary<string, string>();

			if (comboId <= 0)
			{
				errors["comboId"] = "ID combo không hợp lệ";
				return BadRequest(errors);
			}
			else if (!await _comboService.ComboExistsAsync(comboId))
			{
				errors["comboId"] = "Combo không tồn tại";
				return BadRequest(errors);
			}


			if (string.IsNullOrEmpty(updateComboWithDishesDTO.NameCombo))
			{
				errors["nameCombo"] = "Tên combo là bắt buộc";
			}
			else if (updateComboWithDishesDTO.NameCombo.Length > 100)
			{
				errors["nameCombo"] = "Tên combo không được vượt quá 100 ký tự";
			}

			if (!updateComboWithDishesDTO.Price.HasValue)
			{
				errors["price"] = "Giá của Combo là bắt buộc";
			}
			else if (updateComboWithDishesDTO.Price < 0 || updateComboWithDishesDTO.Price > 1000000000)
			{
				errors["price"] = "Giá của Combo phải nằm trong khoảng từ 0 đến 1,000,000,000";
			}

			if (string.IsNullOrEmpty(updateComboWithDishesDTO.Note))
			{
				errors["note"] = "Mô tả Combo là bắt buộc";
			}
			else if (updateComboWithDishesDTO.Note?.Length > 500)
			{
				errors["note"] = "Mô tả Combo không được vượt quá 500 ký tự";
			}

			if (string.IsNullOrEmpty(updateComboWithDishesDTO.ImageUrl))
			{
				errors["image"] = "Hình ảnh là bắt buộc";
			}

			var existingCombos = await _comboService.SearchComboByNameAsync(updateComboWithDishesDTO.NameCombo);
			if (existingCombos.Any(c => c.ComboId != comboId))
			{
				errors["nameCombo"] = "Tên combo đã tồn tại";
			}

            if (updateComboWithDishesDTO.Dishes == null || updateComboWithDishesDTO.Dishes.Count == 0)
            {
                errors["dishes"] = "Thông tin món ăn là bắt buộc";
            }

            if (string.IsNullOrEmpty(updateComboWithDishesDTO.ImageUrl))
			{
				errors["image"] = "Hình ảnh không được để trống";
			}
			else
			{
				string extension = Path.GetExtension(updateComboWithDishesDTO.ImageUrl).ToLower();
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

			try
			{
				var result = await _comboService.UpdateComboWithDishesAsync(comboId, updateComboWithDishesDTO);
				return Ok(new
				{
					message = "Combo đã được cập nhật thành công",
					result
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
			}
		}


        [Authorize(Roles = "Manager")]
        [HttpPost("CreateComboWithDishes")]
        public async Task<ActionResult<ComboDTO>> CreateComboWithDishes([FromBody] UpdateComboDishDTO createComboWithDishesDTO)
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(createComboWithDishesDTO.NameCombo))
            {
                errors["nameCombo"] = "Tên Combo không được để trống";
            }
            else if (createComboWithDishesDTO.NameCombo.Length > 100)
            {
                errors["nameCombo"] = "Tên Combo không được vượt quá 100 ký tự";
            }

            if (!createComboWithDishesDTO.Price.HasValue)
            {
                errors["price"] = "Giá của Combo không được để trống";
            }
            else if (createComboWithDishesDTO.Price < 0 || createComboWithDishesDTO.Price > 1000000000)
            {
                errors["price"] = "Giá của Combo phải nằm trong khoảng từ 0 đến 1,000,000,000";
            }

            if (string.IsNullOrEmpty(createComboWithDishesDTO.Note))
            {
                errors["note"] = "Mô tả không được để trống";
            }
            else if (createComboWithDishesDTO.Note?.Length > 500)
            {
                errors["note"] = "Mô tả combo không được vượt quá 500 ký tự";
            }
            if (string.IsNullOrEmpty(createComboWithDishesDTO.ImageUrl))
            {
                errors["image"] = "Hình ảnh không được để trống";
            }

            var existingCombos = await _comboService.SearchComboByNameAsync(createComboWithDishesDTO.NameCombo);
            if (existingCombos.Any())
            {
                errors["nameCombo"] = "Tên combo đã tồn tại";
            }

            if (createComboWithDishesDTO.Dishes == null || createComboWithDishesDTO.Dishes.Count == 0)
            {
                errors["dish"] = "Món ăn không tồn tại";
            }
            else
            {
                // Kiểm tra các DishId trong Dishes
                var dishIds = createComboWithDishesDTO.Dishes.Select(d => d.DishId).ToList();
                var existingDishes = await _dishRepository.GetDishesByIdsAsync(dishIds);
                if (existingDishes.Count != dishIds.Count)
                {
                    errors["dish"] = "Một số món ăn không tồn tại.";
                }
            }

            if (string.IsNullOrEmpty(createComboWithDishesDTO.ImageUrl))
            {
                errors["image"] = "Hình ảnh không được để trống";
            }
            else
            {
                string extension = Path.GetExtension(createComboWithDishesDTO.ImageUrl).ToLower();
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

            try
            {
                var result = await _comboService.CreateComboWithDishesAsync(createComboWithDishesDTO);
                return Ok(new
                {
                    message = "Combo đã được tạo thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{comboId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteCombo(int comboId)
        {
            try
            {
                await _comboService.DeleteComboAsync(comboId);
                return Ok(new { message = "Combo đã được xóa thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update-quantity")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateQuantityCombo([FromBody] UpdateQuantityComboDTO updateQuantityComboDTO)
        {
            try
            {
                await _comboService.UpdateQuantityComboAsync(updateQuantityComboDTO);
                return Ok(new { message = "Số lượng combo đã được cập nhật thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
