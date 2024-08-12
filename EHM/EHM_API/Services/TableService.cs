﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EHM_API.DTOs.DiscountDTO.Manager;
using EHM_API.DTOs.SettingDTO.Manager;
using EHM_API.DTOs.TableDTO;
using EHM_API.DTOs.TableDTO.Manager;
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
		public async Task<CreateTableDTO> AddAsync(CreateTableDTO tabledto) 
		{

			var table = _mapper.Map<Table>(tabledto);
			var addedtable = await _repository.CreateAsync(table);
			return _mapper.Map<CreateTableDTO>(addedtable);
		}
		public async Task<TableAllDTO> GetTableByIdAsync(int tableId)
		{
			var table = await _repository.GetTableByIdAsync(tableId);
			if (table == null)
			{
				return null;
			}
			return _mapper.Map<TableAllDTO>(table);
		}

		public async Task<CreateTableDTO> UpdateAsync(int tableId, CreateTableDTO tabledto)
		{
            var table = _mapper.Map<Table>(tabledto);
            table.TableId = tableId;
            var updatedTable = await _repository.UpdateTableAsync(table);
            return updatedTable != null ? _mapper.Map<CreateTableDTO>(updatedTable) : null;
        }

		public async Task<IEnumerable<FindTableDTO>> GetAvailableTablesForGuestsAsync(int guestNumber)
		{
			var tables = await _repository.GetAvailableTablesByCapacityAsync(guestNumber);

			var groupedTables = tables.GroupBy(t => t.Floor);

			var results = new List<FindTableDTO>();

			foreach (var group in groupedTables)
			{
				var singleTables = group.Where(t => t.Capacity >= guestNumber).ToList();

				if (singleTables.Any())
				{
					results.AddRange(_mapper.Map<List<FindTableDTO>>(singleTables));
				}

				var combinedTables = FindCombination(group.ToList(), guestNumber);

				if (combinedTables.Any())
				{
					var combinedResults = combinedTables
						.Where(combination => combination.Count > 1)
						.Select(combination => new FindTableDTO
						{
							Capacity = combination.Sum(t => t.Capacity),
							Floor = combination.First().Floor,
							CombinedTables = _mapper.Map<List<FindTableDTO>>(combination)
						}).ToList();

					results.AddRange(combinedResults);
				}
			}

			return results;
		}

		private List<List<Table>> FindCombination(List<Table> tables, int guestNumber)
		{
			var results = new List<List<Table>>();
			FindCombinationRecursive(tables, guestNumber, new List<Table>(), 0, results);

			return results
				.Where(r => r.Sum(t => t.Capacity ?? 0) >= guestNumber && r.Sum(t => t.Capacity ?? 0) <= guestNumber + 2)
				.OrderBy(r => r.Sum(t => t.Capacity ?? 0))
				.ToList();
		}

		private void FindCombinationRecursive(List<Table> tables, int targetCapacity, List<Table> currentCombination, int startIndex, List<List<Table>> results)
		{
			var currentCapacity = currentCombination.Sum(t => t.Capacity ?? 0);
			var currentFloor = currentCombination.Any() ? currentCombination[0].Floor : tables[startIndex].Floor;

			if (currentCapacity >= targetCapacity && currentCapacity <= targetCapacity + 2)
			{
				results.Add(new List<Table>(currentCombination));
				return;
			}

			for (int i = startIndex; i < tables.Count; i++)
			{
				var table = tables[i];

				if (currentCombination.Any() && currentCombination[0].Floor != table.Floor)
				{
					continue;
				}

				currentCombination.Add(table);
				FindCombinationRecursive(tables, targetCapacity, currentCombination, i + 1, results);
				currentCombination.RemoveAt(currentCombination.Count - 1);
			}
		}

		public async Task<bool> ExistTable(int tableId)
		{
			return await _repository.ExistTable(tableId);
		}

	}
}
