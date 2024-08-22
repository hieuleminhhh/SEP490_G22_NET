using EHM_API.DTOs;
using EHM_API.DTOs.DiscountDTO.Manager;
using EHM_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class DiscountsController : ControllerBase
{
    private readonly IDiscountService _discountService;

    public DiscountsController(IDiscountService discountService)
    {
        _discountService = discountService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiscountAllDTO>>> GetAllAsync()
    {
        var discounts = await _discountService.GetAllAsync();
        return Ok(discounts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DiscountAllDTO>> GetByIdAsync(int id)
    {
        var discount = await _discountService.GetByIdAsync(id);
        if (discount == null)
        {
            return NotFound();
        }
        return Ok(discount);
    }

	[Authorize(Roles = "Manager")]
	[HttpPost]
	public async Task<ActionResult<CreateDiscountResponse>> AddAsync([FromBody] CreateDiscount discountDto)
	{
		var errors = new Dictionary<string, string>();

		discountDto.DiscountName = discountDto.DiscountName?.Trim();
		discountDto.Note = discountDto.Note?.Trim();

		if (discountDto.DiscountPercent < 0 || discountDto.DiscountPercent > 100)
		{
			errors["discountPercent"] = "Phần trăm giảm giá phải nằm trong khoảng từ 0 đến 100";
		}

		if (string.IsNullOrEmpty(discountDto.DiscountName))
		{
			errors["discountName"] = "Tên giảm giá không được để trống";
		}
		else if (discountDto.DiscountName.Length > 100)
		{
			errors["discountName"] = "Tên giảm giá không được vượt quá 100 ký tự";
		}

		if (discountDto.Type < 0)
		{
			errors["type"] = "Loại giảm giá không hợp lệ";
		}

		if (!discountDto.StartTime.HasValue)
		{
			errors["startTime"] = "Thời gian bắt đầu không được để trống";
		}
		if (discountDto.StartTime.HasValue && discountDto.EndTime.HasValue && discountDto.StartTime > discountDto.EndTime)
		{
			errors["startTime"] = "Thời gian bắt đầu phải trước thời gian kết thúc";
		}

		if (!discountDto.EndTime.HasValue)
		{
			errors["endTime"] = "Thời gian kết thúc không được để trống";
		}

		if (discountDto.TotalMoney.HasValue && discountDto.TotalMoney < 0)
		{
			errors["totalMoney"] = "Tổng tiền không được âm";
		}

		if (discountDto.QuantityLimit.HasValue && discountDto.QuantityLimit < 0)
		{
			errors["quantityLimit"] = "Giới hạn số lượng không được âm";
		}

		if (errors.Any())
		{
			return BadRequest(errors);
		}

		var discountResponse = await _discountService.AddAsync(discountDto);
		return Ok(new
		{
			message = "Mã giảm giá đã được tạo thành công",
			discount = discountResponse
		});
	}


	[Authorize(Roles = "Manager")]
	[HttpPut("{id}")]
	public async Task<ActionResult<CreateDiscount>> UpdateAsync(int id, [FromBody] CreateDiscount discountDto)
	{
		var errors = new Dictionary<string, string>();

		discountDto.DiscountName = discountDto.DiscountName?.Trim();
		discountDto.Note = discountDto.Note?.Trim();

		if (discountDto.DiscountPercent.HasValue)
		{
			if (discountDto.DiscountPercent < 0 || discountDto.DiscountPercent > 100)
			{
				errors["discountPercent"] = "Phần trăm giảm giá phải nằm trong khoảng từ 0 đến 100";
			}
		}

		if (string.IsNullOrEmpty(discountDto.DiscountName))
		{
			errors["discountName"] = "Tên giảm giá không được để trống";
		}

		if (discountDto.Type.HasValue && discountDto.Type < 0)
		{
			errors["type"] = "Loại giảm giá không hợp lệ";
		}

		if (discountDto.StartTime.HasValue && discountDto.EndTime.HasValue && discountDto.StartTime > discountDto.EndTime)
		{
			errors["time"] = "Thời gian bắt đầu phải trước thời gian kết thúc";
		}

		if (discountDto.TotalMoney.HasValue && discountDto.TotalMoney < 0)
		{
			errors["totalMoney"] = "Tổng tiền không được âm";
		}

		if (discountDto.QuantityLimit.HasValue && discountDto.QuantityLimit < 0)
		{
			errors["quantityLimit"] = "Giới hạn số lượng không được âm";
		}

		if (errors.Any())
		{
			return BadRequest(errors);
		}

		var updatedDiscount = await _discountService.UpdateAsync(id, discountDto);
		if (updatedDiscount == null)
		{
			return NotFound("Không tìm thấy mã giảm giá với ID đã cho");
		}

		return Ok(new
		{
			message = "Mã giảm giá đã được cập nhật thành công",
			discount = updatedDiscount
		});
	}


	[HttpGet("GetDiscountByOrderId/{orderId}")]
	public async Task<ActionResult<GetDiscountByOrderID>> GetDiscountByOrderId(int orderId)
	{
		var discount = await _discountService.GetDiscountByOrderIdAsync(orderId);
		if (discount == null)
		{
			return NotFound(new { message = $"Không tìm thấy giảm giá cho đơn hàng {orderId}" });
		}
		return Ok(discount);
	}

	[HttpGet("search")]
    public async Task<ActionResult<IEnumerable<DiscountAllDTO>>> SearchAsync([FromQuery] string keyword)
    {
        var discounts = await _discountService.SearchAsync(keyword);
        return Ok(discounts);
    }

	[HttpGet("active")]
    public async Task<IActionResult> GetActiveDiscounts()
    {
        var discounts = await _discountService.GetActiveDiscountsAsync();
        return Ok(discounts);
    }

	

}
