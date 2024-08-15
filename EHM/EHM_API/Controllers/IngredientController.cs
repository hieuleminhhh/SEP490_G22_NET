using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EHM_API.DTOs.IngredientDTO.Manager;
using EHM_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EHM_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class IngredientController : ControllerBase
	{
		private readonly IIngredientService _ingredientService;

		public IngredientController(IIngredientService ingredientService)
		{
			_ingredientService = ingredientService;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<IngredientAllDTO>>> GetAllIngredients()
		{
			var ingredients = await _ingredientService.GetAllIngredientsAsync();
			return Ok(ingredients);
		}

		[HttpGet("{dishId}/{materialId}")]
		public async Task<ActionResult<IngredientAllDTO>> GetIngredientById(int dishId, int materialId)
		{
			var errors = new Dictionary<string, string>();

			if (dishId <= 0)
			{
				errors["dishId"] = "ID món ăn phải lớn hơn 0.";
			}

			if (materialId <= 0)
			{
				errors["materialId"] = "ID nguyên liệu phải lớn hơn 0.";
			}

			if (errors.Any())
			{
				return BadRequest(errors);
			}

			var ingredient = await _ingredientService.GetIngredientByIdAsync(dishId, materialId);
			if (ingredient == null)
			{
				return NotFound(new { message = "Không tìm thấy nguyên liệu với món ăn đã cung cấp." });
			}

			return Ok(ingredient);
		}

		[HttpPost]
		public async Task<ActionResult<IngredientAllDTO>> CreateIngredient(CreateIngredientDTO createIngredientDTO)
		{
			var errors = new Dictionary<string, string>();

			if (createIngredientDTO.DishId <= 0)
			{
				errors["DishId"] = "ID món ăn phải lớn hơn 0.";
			}

			if (createIngredientDTO.MaterialId <= 0)
			{
				errors["MaterialId"] = "ID nguyên liệu phải lớn hơn 0.";
			}

			if (createIngredientDTO.Quantitative.HasValue && createIngredientDTO.Quantitative <= 0)
			{
				errors["Quantitative"] = "Giá trị định lượng phải lớn hơn 0.";
			}

			if (errors.Any())
			{
				return BadRequest(errors);
			}

			var createdIngredient = await _ingredientService.CreateIngredientAsync(createIngredientDTO);
			return CreatedAtAction(nameof(GetIngredientById), new { dishId = createdIngredient.DishId, materialId = createdIngredient.MaterialId }, createdIngredient);
		}

		[HttpPut("{dishId}/{materialId}")]
		public async Task<IActionResult> UpdateIngredient(int dishId, int materialId, UpdateIngredientDTO updateIngredientDTO)
		{
			var errors = new Dictionary<string, string>();

			if (dishId <= 0)
			{
				errors["DishId"] = "ID món ăn phải lớn hơn 0.";
			}

			if (materialId <= 0)
			{
				errors["MaterialId"] = "ID nguyên liệu phải lớn hơn 0.";
			}

			if (updateIngredientDTO.Quantitative.HasValue && updateIngredientDTO.Quantitative <= 0)
			{
				errors["Quantitative"] = "Giá trị định lượng phải lớn hơn 0.";
			}

			if (errors.Any())
			{
				return BadRequest(errors);
			}

			var updatedIngredient = await _ingredientService.UpdateIngredientAsync(dishId, materialId, updateIngredientDTO);
			if (updatedIngredient == null)
			{
				return NotFound();
			}

			return Ok(updatedIngredient);
		}

		

		[HttpGet("search-by-dish-id/{dishId}")]
		public async Task<ActionResult<IEnumerable<IngredientAllDTO>>> SearchIngredientsByDishId(int dishId)
		{
			var errors = new Dictionary<string, string>();

			if (dishId <= 0)
			{
				errors["DishId"] = "ID món ăn phải lớn hơn 0.";
			}

			if (errors.Any())
			{
				return BadRequest(errors);
			}

			var ingredients = await _ingredientService.SearchIngredientsByDishIdAsync(dishId);
			if (ingredients == null || !ingredients.Any())
			{
				return NotFound(new { message = "Không tìm thấy nguyên liệu cho món ăn này." });
			}

			return Ok(ingredients);
		}

		[HttpDelete("{dishId}/{materialId}")]
		public async Task<IActionResult> DeleteIngredient(int dishId, int materialId)
		{
			var result = await _ingredientService.DeleteIngredientAsync(dishId, materialId);
			if (!result)
				return NotFound(new { message = "Không tìm thấy nguyên liệu với món ăn đã cung cấp." });

			return NoContent();
		}

        [HttpGet("search-by-dish-item-name")]
        public async Task<ActionResult<object>> GetIngredientsWithQuantity([FromQuery] string name, [FromQuery] int quantity)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { message = "Tên món hoặc combo không được để trống." });
            }

            var result = await _ingredientService.GetIngredientsWithQuantityAsync(name, quantity);
            if (result == null)
            {
                return NotFound(new { message = "Không tìm thấy nguyên liệu cho tên món hoặc combo đã cung cấp." });
            }

            return Ok(result);
        }
    }
}
