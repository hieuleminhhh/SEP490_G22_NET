using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EHM_API.DTOs.TableDTO;
using EHM_API.Models;
using EHM_API.Repositories;

namespace EHM_API.Services
{
	public class TableService : ITableService
	{
		private readonly ITableRepository _repository;
		private readonly IMapper _mapper;

		public TableService(ITableRepository repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		public async Task<IEnumerable<TableAllDTO>> GetAllTablesAsync()
		{
			var tables = await _repository.GetAllTablesAsync();
			return _mapper.Map<IEnumerable<TableAllDTO>>(tables);
		}

		public async Task<IEnumerable<FindTableDTO>> GetAvailableTablesForGuestsAsync(int guestNumber)
		{
			var tables = await _repository.GetAvailableTablesByCapacityAsync(guestNumber);

			// Lấy bàn có sức chứa đơn >= guestNumber
			var singleTables = tables.Where(t => t.Capacity >= guestNumber).ToList();

			if (!singleTables.Any())
			{
				// Tìm các tổ hợp bàn có sức chứa >= guestNumber
				var combinedTables = FindCombination(tables.ToList(), guestNumber);

				// Nếu tìm được tổ hợp phù hợp
				if (combinedTables.Any())
				{
					var combinedResults = combinedTables.Select(combination => new FindTableDTO
					{
						Capacity = combination.Sum(t => t.Capacity),
						CombinedTables = _mapper.Map<List<FindTableDTO>>(combination)
					}).ToList();

					return combinedResults;
				}
			}

			return _mapper.Map<IEnumerable<FindTableDTO>>(singleTables);
		}

		private List<List<Table>> FindCombination(List<Table> tables, int guestNumber)
		{
			var results = new List<List<Table>>();
			FindCombinationRecursive(tables, guestNumber, new List<Table>(), 0, results);
			return results.OrderBy(r => r.Sum(t => t.Capacity ?? 0)).ToList();
		}

		private void FindCombinationRecursive(List<Table> tables, int targetCapacity, List<Table> currentCombination, int startIndex, List<List<Table>> results)
		{
			var currentCapacity = currentCombination.Sum(t => t.Capacity ?? 0);
			if (currentCapacity >= targetCapacity)
			{
				results.Add(new List<Table>(currentCombination));
				return;
			}

			for (int i = startIndex; i < tables.Count; i++)
			{
				currentCombination.Add(tables[i]);
				FindCombinationRecursive(tables, targetCapacity, currentCombination, i + 1, results);
				currentCombination.RemoveAt(currentCombination.Count - 1);
			}
		}
	}
}
