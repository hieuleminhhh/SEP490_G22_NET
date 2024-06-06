using EHM_API.Models;
using Microsoft.EntityFrameworkCore;

namespace EHM_API.Repositories
{
    public class GuestRepository : IGuestRepository
    {
        private readonly EHMDBContext _context;

        public GuestRepository(EHMDBContext context)
        {
            _context = context;
        }

        public async Task<Guest> GetGuestByPhoneAsync(string guestPhone)
        {
            return await _context.Guests.FirstOrDefaultAsync(g => g.GuestPhone == guestPhone);
        }

        public async Task<Guest> AddAsync(Guest guest)
        {
            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();
            return guest;
        }
    }
    }