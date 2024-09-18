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
        private readonly IOrderDetailRepository _orderDetailRepository;
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
        public async Task<ComboDTO> CreateComboWithDishesAsync(UpdateComboDishDTO createComboWithDishesDTO)
        {
            var dishes = await _dishRepository.GetDishesByIdsAsync(createComboWithDishesDTO.Dishes.Select(d => d.DishId).ToList());

            if (dishes.Count != createComboWithDishesDTO.Dishes.Count)
            {
                throw new Exception("Some dishes were not found.");
            }

            var combo = new Combo
            {
                NameCombo = createComboWithDishesDTO.NameCombo,
                Price = createComboWithDishesDTO.Price,
                Note = createComboWithDishesDTO.Note,
                ImageUrl = createComboWithDishesDTO.ImageUrl,
                IsActive = true, // Hoặc giá trị khác tùy thuộc vào yêu cầu của bạn
                ComboDetails = createComboWithDishesDTO.Dishes.Select(d => new ComboDetail
                {
                    DishId = d.DishId,
                    QuantityDish = d.QuantityDish ?? 1 // Sử dụng giá trị mặc định nếu không có số lượng
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

        public async Task<UpdateComboDishDTO> UpdateComboWithDishesAsync(int comboId, UpdateComboDishDTO updateComboWithDishesDTO)
        {
            // Fetch the dishes by IDs
            var dishIds = updateComboWithDishesDTO.Dishes.Select(d => d.DishId).ToList();
            var dishes = await _dishRepository.GetDishesByIdsAsync(dishIds);

            if (dishes == null || dishes.Count != dishIds.Count)
            {
                throw new Exception("Some dishes were not found.");
            }

            // Fetch the combo by ID
            var combo = await _comboRepository.GetByIdAsync(comboId);
            if (combo == null)
            {
                throw new Exception("Combo not found.");
            }

            // Update combo properties
            combo.NameCombo = updateComboWithDishesDTO.NameCombo;
            combo.Price = updateComboWithDishesDTO.Price;
            combo.Note = updateComboWithDishesDTO.Note;
            combo.ImageUrl = updateComboWithDishesDTO.ImageUrl;

            // Clear old combo details
            await _comboRepository.ClearComboDetailsAsync(comboId);

            // Add new combo details with quantity
            combo.ComboDetails = new List<ComboDetail>();
            foreach (var dishDto in updateComboWithDishesDTO.Dishes)
            {
                combo.ComboDetails.Add(new ComboDetail
                {
                    ComboId = comboId,
                    DishId = dishDto.DishId,
                    QuantityDish = dishDto.QuantityDish ?? 1 // Use a default value if QuantityDish is null
                });
            }

            // Update combo in the repository
            await _comboRepository.UpdateAsync(combo);

            // Prepare and return the updated DTO
            var updatedComboDTO = new UpdateComboDishDTO
            {
                NameCombo = combo.NameCombo,
                Price = combo.Price,
                Note = combo.Note,
                ImageUrl = combo.ImageUrl,
                Dishes = combo.ComboDetails.Select(cd => new DishComboDTO
                {
                    DishId = cd.DishId,
                    QuantityDish = cd.QuantityDish
                }).ToList()
            };

            return updatedComboDTO;
        }


        public async Task ClearComboDetailsAsync(int comboId)
        {
            await _comboRepository.ClearComboDetailsAsync(comboId);
        }

        public async Task DeleteComboAsync(int comboId)
        {
            // Kiểm tra combo có tồn tại không
            var combo = await _comboRepository.GetByIdAsync(comboId);
            if (combo == null)
            {
                throw new Exception("Combo không tồn tại.");
            }

            // Kiểm tra combo có trong order detail không
            var existsInOrderDetail = await _comboRepository.ExistsWithComboIdAsync(comboId);
            if (existsInOrderDetail)
            {
                throw new Exception("Combo đang tồn tại trong đơn hàng, không thể xóa.");
            }

            // Xóa combo details trước
            await _comboRepository.DeleteByComboIdAsync(comboId);

            // Xóa combo
            await _comboRepository.DeleteAsync(combo);
        }
        public async Task UpdateQuantityComboAsync(UpdateQuantityComboDTO updateQuantityComboDTO)
        {
            var combo = await _comboRepository.GetByIdAsync(updateQuantityComboDTO.ComboId);
            if (combo == null)
            {
                throw new Exception("Combo không tồn tại.");
            }

            combo.QuantityCombo = updateQuantityComboDTO.QuantityCombo;

            await _comboRepository.UpdateAsync(combo);
        }

    }
}