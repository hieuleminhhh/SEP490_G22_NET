﻿using EHM_API.DTOs;
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
    public async Task<ActionResult<DiscountAllDTO>> AddAsync([FromBody] DiscountAllDTO discountDto)
    {
        var discount = await _discountService.AddAsync(discountDto);
        return Ok(discount);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DiscountAllDTO>> UpdateAsync(int id, [FromBody] DiscountAllDTO discountDto)
    {
        var discount = await _discountService.UpdateAsync(id, discountDto);
        if (discount == null)
        {
            return NotFound();
        }
        return Ok(discount);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _discountService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<DiscountAllDTO>>> SearchAsync([FromQuery] string keyword)
    {
        var discounts = await _discountService.SearchAsync(keyword);
        return Ok(discounts);
    }
}