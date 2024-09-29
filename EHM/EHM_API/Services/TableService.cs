using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EHM_API.DTOs.DiscountDTO.Manager;
using EHM_API.DTOs.ReservationDTO.Guest;
using EHM_API.DTOs.SettingDTO.Manager;
using EHM_API.DTOs.TableDTO;
using EHM_API.DTOs.TableDTO.Manager;
using EHM_API.DTOs.Table_ReservationDTO;
using EHM_API.Models;
using EHM_API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EHM_API.Services
{
	public class TableService : ITableService
	{
		private readonly ITableRepository _repository;
		private readonly IMapper _mapper;
		private readonly EHMDBContext _context;

        public TableService(ITableRepository repository, IMapper mapper, EHMDBContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<TableAllDTO>> GetAllTablesAsync()
        {
            var tables = await _repository.GetAllTablesAsync();

            return tables.Select(t => new TableAllDTO
            {
                TableId = t.TableId,
                Status = t.Status,
                Capacity = t.Capacity,
                Floor = t.Floor,
                Lable = t.Lable,
                TableReservations = t.TableReservations
    .Where(tr => tr.Reservation != null && tr.Reservation.ReservationTime >= DateTime.Now) 
    .OrderBy(tr => tr.Reservation.ReservationTime)
    .Select(tr => new TableReservationDetailDTO
    {
        ReservationId = tr.Reservation?.ReservationId ?? 0, 
        ReservationTime = tr.Reservation?.ReservationTime ?? DateTime.MinValue, 
        GuestNumber = tr.Reservation?.GuestNumber ?? 0, 
        Status = tr.Reservation?.Status ?? 0, 
        GuestPhone = tr.Reservation?.Address?.GuestPhone ?? string.Empty, 
        ConsigneeName = tr.Reservation?.Address?.ConsigneeName ?? string.Empty 
    })
    .ToList()

            }).ToList();
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
        public async Task DeleteTableWithDependenciesAsync(int tableId)
        {
            var table = await _repository.GetTableByIdAsync(tableId);
            if (table == null)
            {
                throw new Exception("Table not found.");
            }

            // Kiểm tra xem bàn có trong OrderTable hoặc TableReservation không
            bool hasOrderTable = await _repository.HasOrderTableAsync(tableId);
            bool hasTableReservation = await _repository.HasTableReservationAsync(tableId);

            if (hasOrderTable || hasTableReservation)
            {
                // Nếu bàn có trong OrderTable hoặc TableReservation, chỉ cập nhật status lên 2
                table.Status = 2;
                await _repository.UpdateTableAsync(table); // Hàm cập nhật thông tin của Table
            }
            else
            {
                // Nếu không có, xóa bàn
                await _repository.DeleteTableAsync(tableId);
            }
        }

        public async Task SetTablesFloorAsync(UpdateFloorDTO updateFloorDTO)
        {
            var currentFloor = updateFloorDTO.CurrentFloor;
            var newFloor = updateFloorDTO.NewFloor;

            // Lấy danh sách các bàn ở tầng hiện tại
            var tables = await _repository.GetTablesByFloorAsync(currentFloor);
            if (tables == null || !tables.Any())
            {
                throw new Exception($"Không có bàn nào ở tầng {currentFloor}");
            }

            if (newFloor == "0") // Kiểm tra nếu newFloor là "0"
            {
                // Nếu newFloor là 0, cập nhật Floor = null và Status = 2
                foreach (var table in tables)
                {
                    table.Floor = null;   // Cập nhật Floor thành null
                    table.Status = 2;     // Cập nhật trạng thái thành 2
                    _context.Tables.Update(table);
                }
            }
            else
            {
                // Nếu newFloor khác "0", cập nhật tầng mới
                foreach (var table in tables)
                {
                    table.Floor = newFloor; // Cập nhật tầng mới
                    _context.Tables.Update(table);
                }
            }

            await _context.SaveChangesAsync();
        }
        public IEnumerable<TableReservationAllDTO> GetTableReservationsByDate(DateTime reservationTime)
        {
            var reservations = _repository.GetByReservationTime(reservationTime);

            return reservations.Select(r => new TableReservationAllDTO
            {
                ReservationId = r.ReservationId,
                ReservationTime = r.ReservationTime,
                GuestNumber = r.GuestNumber,
                Note = r.Note,
                Status = r.Status,
                ReasonCancel = r.ReasonCancel,
                TableId = r.TableReservations?.FirstOrDefault()?.TableId, // Lấy TableId từ TableReservation

                // Lấy thông tin từ Address
                ConsigneeName = r.Address.ConsigneeName,
                GuestPhone = r.Address.GuestPhone
            }).ToList();
        }

    }
}
