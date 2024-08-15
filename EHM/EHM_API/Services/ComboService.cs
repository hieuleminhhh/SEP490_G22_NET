using AutoMapper;
using EHM_API.DTOs.ComboDetailsDTO;
using EHM_API.DTOs.ComboDTO.EHM_API.DTOs.ComboDTO;
using EHM_API.DTOs.ComboDTO.Guest;
using EHM_API.DTOs.ComboDTO.Manager;
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
		private readonly IDishRepository _dishRepository;
		private readonly IMapper _mapper;

		public ComboService(IComboRepository comboRepository, IMapper mapper, IDishRepository dishRepository)
		{
			_comboRepository = comboRepository;
			_dishRepository = dishRepository;
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

		public async Task<bool> ComboExistsAsync(int comboId)
		{
			return await _comboRepository.ComboExistsAsync(comboId);
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

		public async Task<IEnumerable<ComboDTO>> GetAllSortedAsync(SortField? sortField, SortOrder? sortOrder)
		{
			var combos = await _comboRepository.GetAllSortedAsync(sortField, sortOrder);
			return _mapper.Map<IEnumerable<ComboDTO>>(combos);
		}
		public async Task<PagedResult<ViewComboDTO>> GetComboAsync(string search, int page, int pageSize)
		{
			var pagedDishes = await _comboRepository.GetComboAsync(search, page, pageSize);
			var comboDTO = _mapper.Map<IEnumerable<ViewComboDTO>>(pagedDishes.Items);
			return new PagedResult<ViewComboDTO>(comboDTO, pagedDishes.TotalCount, pagedDishes.Page, pagedDishes.PageSize);
		}
		public async Task<PagedResult<ViewComboDTO>> GetComboActive(string search, int page, int pageSize)
		{
			var pagedDishes = await _comboRepository.GetComboActive(search, page, pageSize);
			var comboDTO = _mapper.Map<IEnumerable<ViewComboDTO>>(pagedDishes.Items);
			return new PagedResult<ViewComboDTO>(comboDTO, pagedDishes.TotalCount, pagedDishes.Page, pagedDishes.PageSize);
		}

		public async Task<Combo> UpdateComboStatusAsync(int comboId, bool isActive)
		{
			return await _comboRepository.UpdateComboStatusAsync(comboId, isActive);
		}
		public async Task<ComboDTO> CreateComboWithDishesAsync(CreateComboDishDTO createComboWithDishesDTO)
		{
			var dishes = await _dishRepository.GetDishesByIdsAsync(createComboWithDishesDTO.DishIds);
			if (dishes.Count != createComboWithDishesDTO.DishIds.Count)
			{
				throw new Exception("Some dishes were not found.");
			}

			var combo = new Combo
			{
				NameCombo = createComboWithDishesDTO.NameCombo,
				Price = createComboWithDishesDTO.Price,
				Note = createComboWithDishesDTO.Note,
				ImageUrl = createComboWithDishesDTO.ImageUrl,
                IsActive = createComboWithDishesDTO.IsActive ?? true,
                ComboDetails = createComboWithDishesDTO.DishIds.Select(dishId => new ComboDetail
				{
					DishId = dishId,
				}).ToList() 
			};

			await _comboRepository.AddAsync(combo);

			var comboDTO = new ComboDTO
			{
				ComboId = combo.ComboId,
				NameCombo = combo.NameCombo,
				Price = combo.Price,
				Note = combo.Note,
				ImageUrl = combo.ImageUrl,
				IsActive = combo.IsActive,
				DishIds = combo.ComboDetails.Select(cd => cd.DishId).ToList()
			};

			return comboDTO;
		}
        public async Task<ComboDTO> UpdateComboWithDishesAsync(int comboId, UpdateComboDishDTO updateComboWithDishesDTO)
        {
            // Fetch the dishes by IDs
            var dishes = await _dishRepository.GetDishesByIdsAsync(updateComboWithDishesDTO.DishIds);
            if (dishes == null || dishes.Count != updateComboWithDishesDTO.DishIds.Count)
            {
                throw new Exception("Some dishes were not found.");
            }

            // Fetch the combo by ID
            var combo = await _comboRepository.GetByIdAsync(comboId);
            if (combo == null)
            {
                throw new Exception("Combo not found");
            }

            // Update combo properties
            combo.NameCombo = updateComboWithDishesDTO.NameCombo;
            combo.Price = updateComboWithDishesDTO.Price;
            combo.Note = updateComboWithDishesDTO.Note;
            combo.ImageUrl = updateComboWithDishesDTO.ImageUrl;

            // Clear old combo details
            await ClearComboDetailsAsync(comboId);

            // Add new combo details
            combo.ComboDetails = new List<ComboDetail>();
            foreach (var dishId in updateComboWithDishesDTO.DishIds)
            {
                combo.ComboDetails.Add(new ComboDetail
                {
                    ComboId = comboId,
                    DishId = dishId
                });
            }

            // Update combo in the repository
            await _comboRepository.UpdateAsync(combo);

            // Prepare and return the ComboDTO
            var comboDTO = new ComboDTO
            {
                ComboId = combo.ComboId,
                NameCombo = combo.NameCombo,
                Price = combo.Price,
                Note = combo.Note,
                ImageUrl = combo.ImageUrl,
                DishIds = combo.ComboDetails.Select(cd => cd.DishId).ToList()
            };

            return comboDTO;
        }
        public async Task ClearComboDetailsAsync(int comboId)
        {
            await _comboRepository.ClearComboDetailsAsync(comboId);
        }


	}
}