﻿using EHM_API.DTOs.ComboDTO;
using EHM_API.DTOs.ComboDTO.EHM_API.DTOs.ComboDTO;
using EHM_API.Enums.EHM_API.Models;
using EHM_API.Repositories;
using EHM_API.Services;
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

		public ComboController(IComboService comboService)
		{
			_comboService = comboService;
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

		[HttpPost]
		public async Task<ActionResult> CreateCombo([FromBody] CreateComboDTO comboDTO)
		{
			var errors = new Dictionary<string, string>();

			if (string.IsNullOrEmpty(comboDTO.NameCombo))
			{
				errors["nameCombo"] = "Combo name is required";
			}
			else if (comboDTO.NameCombo.Length > 100)
			{
				errors["nameCombo"] = "The combo name cannot exceed 100 characters";
			}
			else
			{
				var existingCombos = await _comboService.SearchComboByNameAsync(comboDTO.NameCombo);
				if (existingCombos.Any())
				{
					errors["nameCombo"] = "The combo name already exists";
				}
			}

			if (!comboDTO.Price.HasValue)
			{
				errors["price"] = "Price is required";
			}
			else if (comboDTO.Price < 0 || comboDTO.Price > 1000000000)
			{
				errors["price"] = "Price must be between 0 and 1,000,000,000";
			}

			
			if (string.IsNullOrEmpty(comboDTO.Note))
			{
				errors["note"] = "Combo description is required";
			}
			else if (comboDTO.Note.Length > 500)
			{
				errors["note"] = "Combo description must not exceed 500 characters";
			}

			
			if (string.IsNullOrEmpty(comboDTO.ImageUrl))
			{
				errors["imageUrl"] = "Image URL is required";
			}
			else if (!Uri.IsWellFormedUriString(comboDTO.ImageUrl, UriKind.Absolute))
			{
				errors["imageUrl"] = "Invalid Image URL";
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
					message = "The combo has been created successfully",
					createdCombo
				});
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}


		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCombo(int id, [FromBody] ComboDTO comboDTO)
		{
			var errors = new Dictionary<string, string>();

			var existingCombo = await _comboService.GetComboByIdAsync(id);
			if (existingCombo == null)
			{
				return NotFound(new { message = "Combo not found" });
			}

			if (string.IsNullOrEmpty(comboDTO.NameCombo))
			{
				errors["nameCombo"] = "Combo name is required";
			}
			else if (comboDTO.NameCombo.Length > 100)
			{
				errors["nameCombo"] = "The combo name cannot exceed 100 characters";
			}
			else
			{
				var existingCombos = await _comboService.SearchComboByNameAsync(comboDTO.NameCombo);
				if (existingCombos.Any(c => c.ComboId != id))
				{
					errors["nameCombo"] = "The combo name already exists";
				}
			}

			if (!comboDTO.Price.HasValue)
			{
				errors["price"] = "Price is required";
			}
			else if (comboDTO.Price < 0 || comboDTO.Price > 1000000000)
			{
				errors["price"] = "Price must be between 0 and 1,000,000,000";
			}

			if (string.IsNullOrEmpty(comboDTO.Note))
			{
				errors["note"] = "Combo description is required";
			}
			else if (comboDTO.Note.Length > 500)
			{
				errors["note"] = "Combo description must not exceed 500 characters";
			}

			if (string.IsNullOrEmpty(comboDTO.ImageUrl))
			{
				errors["imageUrl"] = "Image URL is required";
			}
			else if (!Uri.IsWellFormedUriString(comboDTO.ImageUrl, UriKind.Absolute))
			{
				errors["imageUrl"] = "Invalid Image URL";
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
					message = "The combo has been successfully updated",
					comboDTO
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}


		[HttpPost("CreateComboWithDishes")]
		public async Task<ActionResult<CreateComboDishDTO>> CreateComboWithDishes([FromBody] CreateComboDishDTO createComboDishDTO)
		{
			var errors = new Dictionary<string, string>();

			if (string.IsNullOrEmpty(createComboDishDTO.NameCombo))
			{
				errors["nameCombo"] = "Combo name is required";
			}
			else if (createComboDishDTO.NameCombo.Length > 100)
			{
				errors["nameCombo"] = "The combo name cannot exceed 100 characters";
			}

			if (createComboDishDTO.Price < 0)
			{
				errors["price"] = "Price cannot be negative";
			}

			if (createComboDishDTO.Note?.Length > 500)
			{
				errors["note"] = "Combo description must not exceed 500 characters";
			}

			var existingCombos = await _comboService.SearchComboByNameAsync(createComboDishDTO.NameCombo);
			if (existingCombos.Any())
			{
				errors["nameCombo"] = "The combo name already exists";
			}

			if (errors.Any())
			{
				return BadRequest(errors);
			}

			try
			{
				var result = await _comboService.CreateComboWithDishesAsync(createComboDishDTO);
				return Ok(new
				{
					message = "The combo with dishes has been created successfully",
					result
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}


		[HttpPut("{id}/cancel")]
		public async Task<IActionResult> CancelCombo(int id)
		{
			try
			{
				await _comboService.CancelComboAsync(id);
				return Ok(new { message = "The combo has been successfully canceled" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpPut("{id}/reactivate")]
		public async Task<IActionResult> ReactivateCombo(int id)
		{
			try
			{
				var result = await _comboService.ReactivateComboAsync(id);
				if (result)
				{
					return Ok(new { message = "The combo has been successfully reactivated" });
				}
				return BadRequest(new { message = "The combo cannot be reactivated" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}



		[HttpGet("sorted-combos")]
        public async Task<IActionResult> GetSortedCombosAsync(SortField sortField, SortOrder sortOrder)
        {
            var combos = await _comboService.GetAllSortedAsync(sortField, sortOrder);
            return Ok(combos);
        }
    }
}