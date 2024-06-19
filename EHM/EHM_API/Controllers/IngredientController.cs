using System.Collections.Generic;
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
            var ingredient = await _ingredientService.GetIngredientByIdAsync(dishId, materialId);
            if (ingredient == null)
                return NotFound();

            return Ok(ingredient);
        }

        [HttpPost]
        public async Task<ActionResult<IngredientAllDTO>> CreateIngredient(CreateIngredientDTO createIngredientDTO)
        {
            var createdIngredient = await _ingredientService.CreateIngredientAsync(createIngredientDTO);
            return CreatedAtAction(nameof(GetIngredientById), new { dishId = createdIngredient.DishId, materialId = createdIngredient.MaterialId }, createdIngredient);
        }

        [HttpPut("{dishId}/{materialId}")]
        public async Task<IActionResult> UpdateIngredient(int dishId, int materialId, UpdateIngredientDTO updateIngredientDTO)
        {
            var updatedIngredient = await _ingredientService.UpdateIngredientAsync(dishId, materialId, updateIngredientDTO);
            if (updatedIngredient == null)
                return NotFound();

            return Ok(updatedIngredient);
        }

        [HttpDelete("{dishId}/{materialId}")]
        public async Task<IActionResult> DeleteIngredient(int dishId, int materialId)
        {
            var result = await _ingredientService.DeleteIngredientAsync(dishId, materialId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("search-by-dish-id/{dishId}")]
        public async Task<ActionResult<IEnumerable<IngredientAllDTO>>> SearchIngredientsByDishId(int dishId)
        {
            var ingredients = await _ingredientService.SearchIngredientsByDishIdAsync(dishId);
            return Ok(ingredients);
        }

        [HttpGet("search-by-dish-item-name")]
        public async Task<ActionResult<IEnumerable<IngredientAllDTO>>> SearchIngredientsByDishItemName([FromQuery] string itemName)
        {
            var ingredients = await _ingredientService.SearchIngredientsByDishItemNameAsync(itemName);
            return Ok(ingredients);
        }
    }
}
