﻿using EHM_API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHM_API.Repositories
{
	public class TableRepository : ITableRepository
	{
		private readonly EHMDBContext _context;

		public TableRepository(EHMDBContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Table>> GetAllTablesAsync()
		{
			return await _context.Tables.ToListAsync();
		}
        public async Task<Table> CreateAsync(Table table)
        {
            _context.Tables.Add(table);
            await _context.SaveChangesAsync();
            return table;
        }

		public async Task<List<Table>> GetAvailableTablesByCapacityAsync(int capacity)
		{
			return await _context.Tables
				.OrderBy(t => t.Capacity)
				.ToListAsync();
		}

		public async Task<Table?> GetByIdAsync(int tableId)
		{
			return await _context.Tables.FindAsync(tableId);
		}

		//danh sach ban cua order
		public async Task<IEnumerable<Order>> GetOrdersWithTablesAsync()
		{
			return await _context.Orders
				.Include(o => o.OrderTables)
				.ThenInclude(ot => ot.Table)
				.ToListAsync();
		}
		public async Task<Table> GetTableByIdAsync(int tableId)
		{
			return await _context.Tables.FindAsync(tableId);
		}
		public async Task<bool> ExistTable(int tableId)
		{
			return await _context.Tables.AnyAsync(t => t.TableId == tableId);
		}

        public async Task<Table> UpdateTableAsync(Table table)
        {
            var existingTable = await _context.Tables.FindAsync(table.TableId);
            if (existingTable == null)
            {
                return null;
            }
            _context.Entry(existingTable).CurrentValues.SetValues(table);
            await _context.SaveChangesAsync();
            return existingTable;
        }

        public async Task<List<Table>> GetListTablesByIdsAsync(List<int> tableIds)
		{
			return await _context.Tables
								 .Where(t => tableIds.Contains(t.TableId))
								 .ToListAsync();
		}

		public async Task UpdateListTablesAsync(List<Table> tables)
		{
			_context.Tables.UpdateRange(tables);
			await _context.SaveChangesAsync();
		}


        public async Task<bool> UpdateTableStatus(int tableId, int status)
        {
            var table = await _context.Tables.FirstOrDefaultAsync(t => t.TableId == tableId);

            if (table == null)
            {
                throw new KeyNotFoundException($"Bàn {tableId} không tồn tại.");
            }

            table.Status = status;

            await _context.SaveChangesAsync();
            return true;
        }


		public async Task<bool> UpdateTableStatusByOrderId(int orderId, int status)
		{
			var tables = await _context.OrderTables
									   .Where(ot => ot.OrderId == orderId)
									   .Select(ot => ot.Table)
									   .ToListAsync();
			if (!tables.Any())
			{
				throw new KeyNotFoundException($"Không tìm thấy bàn nào cho Order {orderId}.");
			}

			foreach (var table in tables)
			{
				table.Status = status;
			}

			await _context.SaveChangesAsync();
			return true;
		}

        public bool TableExistsInReservation(int tableId)
        {
            return _context.TableReservations.Any(tr => tr.TableId == tableId);
        }

        public async Task DeleteTableIfNotInReservation(int tableId)
        {
            var table = await _context.Tables.FindAsync(tableId);
            if (table != null && !TableExistsInReservation(tableId))
            {
                _context.Tables.Remove(table);
                await _context.SaveChangesAsync();
            }
        }
    }
}

