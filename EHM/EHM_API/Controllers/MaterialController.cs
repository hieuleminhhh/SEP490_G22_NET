using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EHM_API.DTOs.MaterialDTO;
using EHM_API.Services;
using EHM_API.Models;

namespace EHM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;

        public MaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialAllDTO>>> GetMaterials()
        {
            var materials = await _materialService.GetAllMaterialsAsync();
            return Ok(materials);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<MaterialAllDTO>> GetMaterialById(int id)
        {
            var material = await _materialService.GetMaterialByIdAsync(id);
            if (material == null)
            {
                return NotFound();
            }
            return Ok(new MaterialAllDTO
            {
                MaterialId = material.MaterialId,
                Name = material.Name,
                Category = material.Category,
                Unit = material.Unit
            });
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<MaterialAllDTO>>> SearchMaterials([FromQuery] string name)
        {
            var materials = await _materialService.SearchMaterialsByNameAsync(name);
            var materialDTOs = new List<MaterialAllDTO>();
            foreach (var material in materials)
            {
                materialDTOs.Add(new MaterialAllDTO
                {
                    MaterialId = material.MaterialId,
                    Name = material.Name,
                    Category = material.Category,
                    Unit = material.Unit
                });
            }
            return Ok(materialDTOs);
        }

        [HttpPost]
        public async Task<ActionResult<MaterialAllDTO>> CreateMaterial(CreateMaterialDTO createMaterialDTO)
        {
            var material = new Material
            {
                Name = createMaterialDTO.Name,
                Category = createMaterialDTO.Category,
                Unit = createMaterialDTO.Unit
            };
            var createdMaterial = await _materialService.CreateMaterialAsync(material);
            return CreatedAtAction(nameof(GetMaterialById), new { id = createdMaterial.MaterialId }, new MaterialAllDTO
            {
                MaterialId = createdMaterial.MaterialId,
                Name = createdMaterial.Name,
                Category = createdMaterial.Category,
                Unit = createdMaterial.Unit
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterial(int id, UpdateMaterialDTO updateMaterialDTO)
        {
            var existingMaterial = await _materialService.GetMaterialByIdAsync(id);
            if (existingMaterial == null)
            {
                return NotFound();
            }

            existingMaterial.Name = updateMaterialDTO.Name;
            existingMaterial.Category = updateMaterialDTO.Category;
            existingMaterial.Unit = updateMaterialDTO.Unit;

            await _materialService.UpdateMaterialAsync(existingMaterial);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterial(int id)
        {
            var existingMaterial = await _materialService.GetMaterialByIdAsync(id);
            if (existingMaterial == null)
            {
                return NotFound();
            }

            await _materialService.DeleteMaterialAsync(id);
            return NoContent();
        }
    }
}
