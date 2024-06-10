using EHM_API.DTOs.ComboDTO;
using EHM_API.DTOs.ComboDTO.EHM_API.DTOs.ComboDTO;
using EHM_API.Enums.EHM_API.Models;
using EHM_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EHM_API.Repositories
{
	public class ComboRepository : IComboRepository
	{
		private readonly EHMDBContext _context;

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

		public async Task<IEnumerable<Combo>> GetAllAsync()
		{
			return await _context.Combos.ToListAsync();
		}
		public async Task<List<Combo>> SearchComboByNameAsync(string name)
		{
			return await _context.Combos
				.Where(c => c.NameCombo.Contains(name))
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
				throw new ArgumentException("Combo or Dish does not exist.");

			_context.ComboDetails.Add(comboDetail);
			await _context.SaveChangesAsync();
		}

		public async Task<CreateComboDishDTO> CreateComboWithDishesAsync(CreateComboDishDTO createComboDishDTO)
		{

			var combo = new Combo
			{
				NameCombo = createComboDishDTO.NameCombo,
				Price = createComboDishDTO.Price,
				Note = createComboDishDTO.Note,
				ImageUrl = createComboDishDTO.ImageUrl,
				IsActive = true
			};

			_context.Combos.Add(combo);
			await _context.SaveChangesAsync();

			foreach (var dishDto in createComboDishDTO.Dishes)
			{
				var comboDetail = new ComboDetail
				{
					ComboId = combo.ComboId,
					DishId = dishDto.DishId
				};


				_context.ComboDetails.Add(comboDetail);
			}

			await _context.SaveChangesAsync();


			return createComboDishDTO;
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

		public async Task<IEnumerable<Combo>> GetAllSortedAsync(SortField sortField, SortOrder sortOrder)
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
                    throw new ArgumentException("Invalid sort field.");
            }

            return await query.ToListAsync();
        }
    }
}