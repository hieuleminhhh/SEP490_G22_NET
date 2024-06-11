using AutoMapper;
using EHM_API.DTOs.ComboDTO;
using EHM_API.DTOs.ComboDTO.EHM_API.DTOs.ComboDTO;
using EHM_API.DTOs.DishDTO;
using EHM_API.DTOs.HomeDTO;
using EHM_API.Enums.EHM_API.Models;
using EHM_API.Models;
using EHM_API.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
	public class ComboService : IComboService
	{
		private readonly IComboRepository _comboRepository;
		private readonly IMapper _mapper;

		public ComboService(IComboRepository comboRepository, IMapper mapper)
		{
			_comboRepository = comboRepository;
			_mapper = mapper;
		}

		public async Task<IEnumerable<ComboDTO>> GetAllCombosAsync()
		{
			var combos = await _comboRepository.GetAllAsync();
			var activeCombos = combos.Where(c => c.IsActive == true);
			return _mapper.Map<IEnumerable<ComboDTO>>(activeCombos);
		}
		public async Task<List<ComboDTO>> SearchComboByNameAsync(string name)
		{
			var combos = await _comboRepository.SearchComboByNameAsync(name);
			var activeCombos = combos.Where(c => c.IsActive == true);
			return _mapper.Map<List<ComboDTO>>(activeCombos);
		}

		public async Task<ViewComboDTO> GetComboWithDishesAsync(int comboId)
		{
			return await _comboRepository.GetComboWithDishesAsync(comboId);
		}
		public async Task<ComboDTO> GetComboByIdAsync(int comboId)
		{
			var combo = await _comboRepository.GetComboByIdAsync(comboId);
			if (combo == null)
			{
				return null;
			}

			return _mapper.Map<ComboDTO>(combo);
		}

		public async Task<CreateComboDTO> CreateComboAsync(CreateComboDTO comboDTO)
		{
			var combo = _mapper.Map<Combo>(comboDTO);
			var createdCombo = await _comboRepository.AddAsync(combo);
			return _mapper.Map<CreateComboDTO>(createdCombo);
		}

		public async Task UpdateComboAsync(int id, ComboDTO comboDTO)
		{
			var existingCombo = await _comboRepository.GetByIdAsync(id);
			if (existingCombo == null)
			{
				throw new KeyNotFoundException($"Combo with ID {id} not found.");
			}

			_mapper.Map(comboDTO, existingCombo);
			await _comboRepository.UpdateAsync(existingCombo);
		}

		public async Task CancelComboAsync(int comboId)
		{
			await _comboRepository.UpdateStatusAsync(comboId, false);
		}

		public async Task<bool> ReactivateComboAsync(int comboId)
		{
			if (await _comboRepository.CanActivateComboAsync(comboId))
			{
				await _comboRepository.UpdateStatusAsync(comboId, true);
				return true;
			}
			return false;
		}

		public async Task<CreateComboDishDTO> CreateComboWithDishesAsync(CreateComboDishDTO createComboDishDTO)
		{
			var result = await _comboRepository.CreateComboWithDishesAsync(createComboDishDTO);
			return result;
		}


		public async Task<IEnumerable<ComboDTO>> GetAllSortedAsync(SortField sortField, SortOrder sortOrder)
		{
			var combos = await _comboRepository.GetAllSortedAsync(sortField, sortOrder);
			return _mapper.Map<IEnumerable<ComboDTO>>(combos);
		}
        public async Task<PagedResult<ComboDTO>> GetComboAsync(string search, int page, int pageSize)
        {
            var pagedDishes = await _comboRepository.GetComboAsync(search, page, pageSize);
            var comboDTO = _mapper.Map<IEnumerable<ComboDTO>>(pagedDishes.Items);
            return new PagedResult<ComboDTO>(comboDTO, pagedDishes.TotalCount, pagedDishes.Page, pagedDishes.PageSize);
        }
        public async Task<Combo> UpdateComboStatusAsync(int comboId, bool isActive)
        {
            return await _comboRepository.UpdateComboStatusAsync(comboId, isActive);
        }
    }
}