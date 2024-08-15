using EHM_API.DTOs.ComboDTO.EHM_API.DTOs.ComboDTO;
using EHM_API.DTOs.ComboDTO.Guest;
using EHM_API.DTOs.DishDTO;
using EHM_API.DTOs.HomeDTO;
using EHM_API.Enums.EHM_API.Models;
using EHM_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EHM_API.Repositories
{
    public class ComboRepository : IComboRepository
	{
		private readonly EHMDBContext _context;
        private static SemaphoreSlim _lockObject = new SemaphoreSlim(1);
        public ComboRepository(EHMDBContext context)
		{
			_context = context;
		}

		public async Task<ViewComboDTO> GetComboWithDishesAsync(int comboId)
		{
			var comboDetails = await _context.ComboDetails
				.Include(cd => cd.Combo)
				.Include(cd => cd.Dish)
				.ThenInclude(d => d.Category)
				.Where(cd => cd.ComboId == comboId)
				.ToListAsync();

			if (!comboDetails.Any())
			{
				return null;
			}

			var combo = comboDetails.FirstOrDefault()?.Combo;

			var viewComboDTO = new ViewComboDTO
			{
				ComboId = combo.ComboId,
				NameCombo = combo.NameCombo,
				Price = combo.Price,
				Note = combo.Note,
				ImageUrl = combo.ImageUrl,
				Dishes = comboDetails.Select(cd => new DishDTO
				{
					DishId = cd.Dish.DishId,
					ItemName = cd.Dish.ItemName,
					ItemDescription = cd.Dish.ItemDescription,
					Price = cd.Dish.Price,
					ImageUrl = cd.Dish.ImageUrl,
					CategoryName = cd.Dish.Category.CategoryName
				}).ToList()
			};

			return viewComboDTO;
		}

		public async Task<Combo> GetComboByIdAsync(int comboId)
		{
			return await _context.Combos.FindAsync(comboId);
		}
		public async Task<bool> ComboExistsAsync(int comboId)
		{
			return await _context.Combos.AnyAsync(c => c.ComboId == comboId);
		}

		public async Task<IEnumerable<Combo>> GetAllAsync()
		{
			return await _context.Combos.Include(d => d.ComboDetails).ThenInclude(d => d.Dish).ToListAsync();
		}
		public async Task<List<Combo>> SearchComboByNameAsync(string name)
		{
			return await _context.Combos
				.Where(c => c.NameCombo.Equals(name))
				.ToListAsync();
		}

		public async Task<Combo> GetByIdAsync(int id)
		{
			return await _context.Combos.FindAsync(id);
		}

		public async Task<Combo> AddAsync(Combo combo)
		{
			_context.Combos.Add(combo);
			await _context.SaveChangesAsync();
			return combo;
		}

		public async Task UpdateAsync(Combo combo)
		{
			_context.Entry(combo).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task AddComboDetailAsync(ComboDetail comboDetail)
		{
			if (comboDetail == null)
				throw new ArgumentNullException(nameof(comboDetail));

			var combo = await _context.Combos.FindAsync(comboDetail.ComboId);
			var dish = await _context.Dishes.FindAsync(comboDetail.DishId);

			if (combo == null || dish == null)
				throw new ArgumentException("Combo hoặc Món ăn không tồn tại.");

			_context.ComboDetails.Add(comboDetail);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateStatusAsync(int comboId, bool isActive)
		{
			var combo = await _context.Combos.FindAsync(comboId);
			if (combo != null)
			{
				combo.IsActive = isActive;
				await _context.SaveChangesAsync();
			}
		}

		public async Task<bool> CanActivateComboAsync(int comboId)
		{
			var combo = await _context.Combos
				.Where(c => c.ComboId == comboId && c.IsActive == false)
				.FirstOrDefaultAsync();
			return combo != null;
		}

		public async Task<IEnumerable<Combo>> GetAllSortedAsync(SortField? sortField, SortOrder? sortOrder)
		{
			IQueryable<Combo> query = _context.Combos;

			switch (sortField)
			{
				case SortField.Name:
					query = sortOrder == SortOrder.Ascending ? query.OrderBy(c => c.NameCombo) : query.OrderByDescending(c => c.NameCombo);
					break;
				case SortField.Price:
					query = sortOrder == SortOrder.Ascending ? query.OrderBy(c => c.Price) : query.OrderByDescending(c => c.Price);
					break;
				default:
					throw new ArgumentException("Trường sắp xếp không hợp lệ.");
			}

			return await query.ToListAsync();
		}
        public async Task<PagedResult<ViewComboDTO>> GetComboAsync(string search, int page, int pageSize)
        {
            var query = _context.Combos.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(d => d.NameCombo.ToLower().Contains(search));
            }

            var totalCombos = await query.CountAsync();

            var combos = await query.Include(c => c.ComboDetails)
                                    .ThenInclude(cd => cd.Dish)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            var comboDTOs = combos.Select(c => new ViewComboDTO
            {
                ComboId = c.ComboId,
                NameCombo = c.NameCombo,
                Price = c.Price,
                Note = c.Note,
                ImageUrl = c.ImageUrl,
                IsActive = c.IsActive,
                Dishes = c.ComboDetails.Select(cd => new DishDTO
                {
                    DishId = cd.Dish.DishId,
                    ItemName = cd.Dish.ItemName,
                    Price = cd.Dish.Price
                }).ToList()
            }).ToList();

            return new PagedResult<ViewComboDTO>(comboDTOs, totalCombos, page, pageSize);
        }

		public async Task<PagedResult<ViewComboDTO>> GetComboActive(string search, int page, int pageSize)
		{
			var query = _context.Combos.AsQueryable();
			query = query.Where(d => d.IsActive == true);

			if (!string.IsNullOrEmpty(search))
			{
				search = search.ToLower();
				query = query.Where(d => d.NameCombo.ToLower().Contains(search));
			}

			var totalCombos = await query.CountAsync();

			var combos = await query.Include(c => c.ComboDetails)
									.ThenInclude(cd => cd.Dish)
									.Skip((page - 1) * pageSize)
									.Take(pageSize)
									.ToListAsync();

			var comboDTOs = combos.Select(c => new ViewComboDTO
			{
				ComboId = c.ComboId,
				NameCombo = c.NameCombo,
				Price = c.Price,
				Note = c.Note,
				ImageUrl = c.ImageUrl,
				IsActive = c.IsActive,
				Dishes = c.ComboDetails.Select(cd => new DishDTO
				{
					DishId = cd.Dish.DishId,
					ItemName = cd.Dish.ItemName,
					Price = cd.Dish.Price
				}).ToList()
			}).ToList();

			return new PagedResult<ViewComboDTO>(comboDTOs, totalCombos, page, pageSize);
		}
		public async Task<Combo> UpdateComboStatusAsync(int comboId, bool isActive)
        {
            var cb = await _context.Combos.FindAsync(comboId);
            if (cb == null)
            {
                return null;
            }

            cb.IsActive = isActive;
            _context.Combos.Update(cb);
            await _context.SaveChangesAsync();

            return cb;
        }
        public async Task ClearComboDetailsAsync(int comboId)
        {
            var sql = $"DELETE FROM [EHMDB].[dbo].[ComboDetails] WHERE [ComboID] = '{comboId}';";
            await _context.Database.ExecuteSqlRawAsync(sql);
        }
    }
}