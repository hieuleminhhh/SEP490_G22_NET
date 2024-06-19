using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EHM_API.DTOs.MaterialDTO;
using EHM_API.Models;
using EHM_API.Repositories;

namespace EHM_API.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _materialRepository;
        private readonly IMapper _mapper;
        public MaterialService(IMaterialRepository materialRepository, IMapper mapper)
        {
            _materialRepository = materialRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MaterialAllDTO>> GetAllMaterialsAsync()
        {
            var materials = await _materialRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<MaterialAllDTO>>(materials);
        }

        public async Task<Material> GetMaterialByIdAsync(int id)
        {
            return await _materialRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Material>> SearchMaterialsByNameAsync(string name)
        {
            return await _materialRepository.SearchByNameAsync(name);
        }

        public async Task<Material> CreateMaterialAsync(Material material)
        {
            return await _materialRepository.CreateAsync(material);
        }

        public async Task<Material> UpdateMaterialAsync(Material material)
        {
            return await _materialRepository.UpdateAsync(material);
        }

        public async Task DeleteMaterialAsync(int id)
        {
            await _materialRepository.DeleteAsync(id);
        }
    }
}
