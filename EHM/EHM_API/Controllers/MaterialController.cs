using System.Collections.Generic;
using System.Linq;
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
			if (id <= 0)
			{
				return BadRequest(new { message = "ID phải lớn hơn 0." });
			}

			var material = await _materialService.GetMaterialByIdAsync(id);
			if (material == null)
			{
				return NotFound(new { message = "Không tìm thấy nguyên liệu." });
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
			if (string.IsNullOrWhiteSpace(name))
			{
				return BadRequest(new { message = "Tên không được để trống." });
			}

			var materials = await _materialService.SearchMaterialsByNameAsync(name.Trim());
			if (materials == null || !materials.Any())
			{
				return NotFound(new { message = "Không tìm thấy nguyên liệu với tên đã cung cấp." });
			}

			var materialDTOs = materials.Select(material => new MaterialAllDTO
			{
				MaterialId = material.MaterialId,
				Name = material.Name,
				Category = material.Category,
				Unit = material.Unit
			});

			return Ok(materialDTOs);
		}

		[HttpPost]
		public async Task<ActionResult<MaterialAllDTO>> CreateMaterial(CreateMaterialDTO createMaterialDTO)
		{
			var errors = new Dictionary<string, string>();

			// Trim input values
			createMaterialDTO.Name = createMaterialDTO.Name?.Trim();
			createMaterialDTO.Category = createMaterialDTO.Category?.Trim();
			createMaterialDTO.Unit = createMaterialDTO.Unit?.Trim();

			// Validate Name
			if (string.IsNullOrEmpty(createMaterialDTO.Name))
			{
				errors["name"] = "Tên không được để trống.";
			}
			else if (createMaterialDTO.Name.Length > 100)
			{
				errors["name"] = "Tên không được vượt quá 100 ký tự.";
			}

			// Validate Category
			if (string.IsNullOrEmpty(createMaterialDTO.Category))
			{
				errors["category"] = "Danh mục không được để trống.";
			}
			else if (createMaterialDTO.Category.Length > 50)
			{
				errors["category"] = "Danh mục không được vượt quá 50 ký tự.";
			}

			// Validate Unit
			if (string.IsNullOrEmpty(createMaterialDTO.Unit))
			{
				errors["unit"] = "Đơn vị không được để trống.";
			}
			else if (createMaterialDTO.Unit.Length > 50)
			{
				errors["unit"] = "Đơn vị không được vượt quá 50 ký tự.";
			}

			// Return validation errors if any
			if (errors.Any())
			{
				return BadRequest(errors);
			}

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
			if (id <= 0)
			{
				return BadRequest(new { message = "ID phải lớn hơn 0." });
			}

			var errors = new Dictionary<string, string>();

			// Trim input values
			updateMaterialDTO.Name = updateMaterialDTO.Name?.Trim();
			updateMaterialDTO.Category = updateMaterialDTO.Category?.Trim();
			updateMaterialDTO.Unit = updateMaterialDTO.Unit?.Trim();

			// Validate Name
			if (string.IsNullOrEmpty(updateMaterialDTO.Name))
			{
				errors["name"] = "Tên không được để trống.";
			}
			else if (updateMaterialDTO.Name.Length > 100)
			{
				errors["name"] = "Tên không được vượt quá 100 ký tự.";
			}

			// Validate Category
			if (string.IsNullOrEmpty(updateMaterialDTO.Category))
			{
				errors["category"] = "Danh mục không được để trống.";
			}
			else if (updateMaterialDTO.Category.Length > 50)
			{
				errors["category"] = "Danh mục không được vượt quá 50 ký tự.";
			}

			// Validate Unit
			if (string.IsNullOrEmpty(updateMaterialDTO.Unit))
			{
				errors["unit"] = "Đơn vị không được để trống.";
			}
			else if (updateMaterialDTO.Unit.Length > 50)
			{
				errors["unit"] = "Đơn vị không được vượt quá 50 ký tự.";
			}

			if (errors.Any())
			{
				return BadRequest(errors);
			}

			var existingMaterial = await _materialService.GetMaterialByIdAsync(id);
			if (existingMaterial == null)
			{
				return NotFound(new { message = "Không tìm thấy nguyên liệu." });
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
			if (id <= 0)
			{
				return BadRequest(new { message = "ID phải lớn hơn 0." });
			}

			var existingMaterial = await _materialService.GetMaterialByIdAsync(id);
			if (existingMaterial == null)
			{
				return NotFound(new { message = "Không tìm thấy nguyên liệu." });
			}

			await _materialService.DeleteMaterialAsync(id);
			return NoContent();
		}



	}
}
