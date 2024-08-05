using EHM_API.DTOs;
using EHM_API.DTOs.DiscountDTO.Manager;
using EHM_API.Services;
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
    [HttpPost]
    public async Task<ActionResult<CreateDiscountResponse>> AddAsync([FromBody] CreateDiscount discountDto)
    {
        var discountResponse = await _discountService.AddAsync(discountDto);
        return Ok(discountResponse);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CreateDiscount>> UpdateAsync(int id, [FromBody] CreateDiscount discountDto)
    {
        var discount = await _discountService.UpdateAsync(id, discountDto);
        if (discount == null)
        {
            return NotFound();
        }
        return Ok(discount);
    }


    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<DiscountAllDTO>>> SearchAsync([FromQuery] string keyword)
    {
        var discounts = await _discountService.SearchAsync(keyword);
        return Ok(discounts);
    }

    [HttpPost("update-discount-status")]
    public async Task<IActionResult> UpdateDiscountStatus()
    {
        var result = await _discountService.UpdateDiscountStatusAsync();
        if (result)
        {
            return Ok("Discount statuses updated successfully.");
        }
        return StatusCode(500, "An error occurred while updating discount statuses.");
    }
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveDiscounts()
    {
        var discounts = await _discountService.GetActiveDiscountsAsync();
        return Ok(discounts);
    }


}
