using AutoMapper;
using EHM_API.DTOs.ComboDTO;
using EHM_API.Models;
using EHM_API.Repositories;
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
			return _mapper.Map<IEnumerable<ComboDTO>>(combos);
		}
		public async Task<List<ComboDTO>> SearchComboByNameAsync(string name)
		{
			var combos = await _comboRepository.SearchComboByNameAsync(name);
			return _mapper.Map<List<ComboDTO>>(combos);
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

		public async Task DeleteComboAsync(int id)
		{
			var existingCombo = await _comboRepository.GetByIdAsync(id);
			if (existingCombo == null)
			{
				throw new KeyNotFoundException($"Combo with ID {id} not found.");
			}

			await _comboRepository.DeleteAsync(id);
		}

		public async Task<CreateComboDishDTO> CreateComboWithDishesAsync(CreateComboDishDTO createComboDishDTO)
		{
			var combo = _mapper.Map<Combo>(createComboDishDTO);

			// Thêm Combo vào cơ sở dữ liệu
			var createdCombo = await _comboRepository.AddAsync(combo);

			// Thêm các Dish vào ComboDetail
			foreach (var dishDto in createComboDishDTO.Dishes)
			{
				var comboDetail = new ComboDetail
				{
					ComboId = createdCombo.ComboId,
					DishId = dishDto.DishId
				};
				await _comboRepository.AddComboDetailAsync(comboDetail);
			}

			return _mapper.Map<CreateComboDishDTO>(createdCombo);
		}
	}
}