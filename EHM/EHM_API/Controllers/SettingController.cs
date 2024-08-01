using EHM_API.DTOs.SettingDTO.Manager;
using EHM_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EHM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingController : ControllerBase
    {
        private readonly ISettingService _settingService;

        public SettingController(ISettingService settingService)
        {
            _settingService = settingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SettingAllDTO>>> GetAllAsync()
        {
            var settings = await _settingService.GetAllAsync();
            return Ok(settings);
        }

        [HttpPost]
        public async Task<ActionResult<SettingAllDTO>> AddAsync([FromBody] SettingAllDTO settingDto)
        {
            var setting = await _settingService.AddAsync(settingDto);
            return Created("", setting);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SettingAllDTO>> UpdateAsync(int id, [FromBody] SettingAllDTO settingDto)
        {
            var updatedSetting = await _settingService.UpdateAsync(id, settingDto);
            if (updatedSetting == null)
            {
                return NotFound();
            }
            return Ok(updatedSetting);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var success = await _settingService.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}