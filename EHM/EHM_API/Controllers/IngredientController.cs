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
            var errors = new Dictionary<string, string>();

            if (dishId <= 0)
            {
                errors["dishId"] = "Dish ID must be greater than 0.";
            }

         
            if (materialId <= 0)
            {
                errors["materialId"] = "Material ID must be greater than 0.";
            }

          
            if (errors.Any())
            {
                return BadRequest(errors);
            }

           
            var ingredient = await _ingredientService.GetIngredientByIdAsync(dishId, materialId);
            if (ingredient == null)
            {
                return NotFound(new { message = "No ingredient found for the provided IDs." });
            }

            return Ok(ingredient);
        }


        [HttpPost]
        public async Task<ActionResult<IngredientAllDTO>> CreateIngredient(CreateIngredientDTO createIngredientDTO)
        {
            var errors = new Dictionary<string, string>();

        
            if (createIngredientDTO.DishId <= 0)
            {
                errors["DishId"] = "Dish ID must be greater than 0.";
            }

     
            if (createIngredientDTO.MaterialId <= 0)
            {
                errors["MaterialId"] = "Material ID must be greater than 0.";
            }

   
            if (createIngredientDTO.Quantitative.HasValue && createIngredientDTO.Quantitative <= 0)
            {
                errors["Quantitative"] = "Quantitative value must be greater than 0.";
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
                errors["DishId"] = "Dish ID must be greater than 0.";
            }

         
            if (materialId <= 0)
            {
                errors["MaterialId"] = "Material ID must be greater than 0.";
            }


            if (updateIngredientDTO.Quantitative.HasValue && updateIngredientDTO.Quantitative <= 0)
            {
                errors["Quantitative"] = "Quantitative value must be greater than 0.";
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
            var errors = new Dictionary<string, string>();

            if (dishId <= 0)
            {
                errors["DishId"] = "Dish ID must be greater than 0.";
            }

       
            if (errors.Any())
            {
                return BadRequest(errors);
            }


            var ingredients = await _ingredientService.SearchIngredientsByDishIdAsync(dishId);
            if (ingredients == null || !ingredients.Any())
            {
                return NotFound(new { message = "No ingredients found for this dish ID." });
            }

            return Ok(ingredients);
        }

        [HttpGet("search-by-dish-item-name")]
        public async Task<ActionResult<IEnumerable<IngredientAllDTO>>> SearchIngredientsByDishItemName([FromQuery] string itemName)
        {
            var errors = new Dictionary<string, string>();

           
            if (string.IsNullOrWhiteSpace(itemName))
            {
                errors["itemName"] = "Item name cannot be empty or whitespace.";
            }

           
            if (errors.Any())
            {
                return BadRequest(errors);
            }

            var ingredients = await _ingredientService.SearchIngredientsByDishItemNameAsync(itemName.Trim());
            if (ingredients == null || !ingredients.Any())
            {
                return NotFound(new { message = "No ingredients found for the provided item name." });
            }

            return Ok(ingredients);
        }

    }
}
