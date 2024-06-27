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
    }
}
