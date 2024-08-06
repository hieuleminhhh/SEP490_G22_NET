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

  
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveDiscounts()
    {
        var discounts = await _discountService.GetActiveDiscountsAsync();
        return Ok(discounts);
    }


}
