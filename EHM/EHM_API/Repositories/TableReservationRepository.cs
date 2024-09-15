using System.Threading.Tasks;
using EHM_API.DTOs.TBDTO;
using EHM_API.Models;
using Microsoft.EntityFrameworkCore;

namespace EHM_API.Repositories
{
    public class TableReservationRepository : ITableReservationRepository
    {
        private readonly EHMDBContext _context;

        public TableReservationRepository(EHMDBContext context)
        {
            _context = context;
        }

        public async Task AddTableReservationAsync(TableReservation tableReservation)
        {
            await _context.TableReservations.AddAsync(tableReservation);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteTableReservationByReservationIdAsync(int reservationId)
        {
            var tableReservation = await _context.TableReservations
                .FirstOrDefaultAsync(tr => tr.ReservationId == reservationId);

            if (tableReservation == null)
                return false;

            _context.TableReservations.Remove(tableReservation);
            await _context.SaveChangesAsync();
            return true;
        }

		public async Task CreateOrderTablesAsync(OrderTable orderTable)
		{
			await _context.OrderTables.AddAsync(orderTable);
			await _context.SaveChangesAsync();
		}
        public async Task<IEnumerable<FindTableByReservation>> GetTableByReservationsAsync(int reservationId)
        {
            var tableReservations = await _context.TableReservations
                .Where(tr => tr.ReservationId == reservationId)
                .Select(tr => new FindTableByReservation
                {
                    TableId = tr.TableId
                  
                })
                .ToListAsync();

            return tableReservations;
        }
        public async Task AddMultipleTableReservationsAsync(List<TableReservation> tableReservations)
        {
            await _context.Set<TableReservation>().AddRangeAsync(tableReservations);
            await _context.SaveChangesAsync();
        }

    }
}
