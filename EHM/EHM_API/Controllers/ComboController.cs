using EHM_API.DTOs.ComboDTO;
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

        [HttpPost]
        public async Task<ActionResult<ComboDTO>> CreateCombo([FromBody] ComboDTO comboDTO)
        {
            try
            {
                var createdCombo = await _comboService.CreateComboAsync(comboDTO);
                return CreatedAtAction(nameof(GetCombos), new { id = createdCombo.ComboId }, createdCombo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCombo(int id, [FromBody] ComboDTO comboDTO)
        {
            try
            {
                await _comboService.UpdateComboAsync(id, comboDTO);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCombo(int id)
        {
            try
            {
                await _comboService.DeleteComboAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}