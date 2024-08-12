using System.Threading.Tasks;
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
    }
}
