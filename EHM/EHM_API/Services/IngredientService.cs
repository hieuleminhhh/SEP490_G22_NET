using AutoMapper;
using EHM_API.DTOs.IngredientDTO.Manager;
using EHM_API.Models;
using EHM_API.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        public IngredientService(IIngredientRepository ingredientRepository, IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<IngredientAllDTO>> GetAllIngredientsAsync()
        {
            var ingredients = await _ingredientRepository.GetAllIngredientsAsync();
            return _mapper.Map<IEnumerable<IngredientAllDTO>>(ingredients);
        }

        public async Task<IngredientAllDTO> GetIngredientByIdAsync(int dishId, int materialId)
        {
            var ingredient = await _ingredientRepository.GetIngredientByIdAsync(dishId, materialId);
            if (ingredient == null)
            {
                return null;
            }
            return _mapper.Map<IngredientAllDTO>(ingredient);
        }

        public async Task<IngredientAllDTO> CreateIngredientAsync(CreateIngredientDTO createIngredientDTO)
        {
            var ingredient = _mapper.Map<Ingredient>(createIngredientDTO);
            var createdIngredient = await _ingredientRepository.CreateIngredientAsync(createIngredientDTO);
            return _mapper.Map<IngredientAllDTO>(createdIngredient);
        }

        public async Task<IngredientAllDTO> UpdateIngredientAsync(int dishId, int materialId, UpdateIngredientDTO updateIngredientDTO)
        {
            var ingredient = await _ingredientRepository.UpdateIngredientAsync(dishId, materialId, updateIngredientDTO);
            if (ingredient == null)
            {
                return null;
            }
            return _mapper.Map<IngredientAllDTO>(ingredient);
        }

        public async Task<bool> DeleteIngredientAsync(int dishId, int materialId)
        {
            return await _ingredientRepository.DeleteIngredientAsync(dishId, materialId);
        }

        public async Task<IEnumerable<IngredientAllDTO>> SearchIngredientsByDishIdAsync(int dishId)
        {
            var ingredients = await _ingredientRepository.SearchIngredientsByDishIdAsync(dishId);
            return _mapper.Map<IEnumerable<IngredientAllDTO>>(ingredients);
        }

        public async Task<object> GetIngredientsWithQuantityAsync(string name, int quantity)
        {
            return await _ingredientRepository.GetIngredientsWithQuantityAsync(name, quantity);
        }
    }
}
