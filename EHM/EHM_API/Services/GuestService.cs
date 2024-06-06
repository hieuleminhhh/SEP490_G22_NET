using EHM_API.Models;
using EHM_API.Repositories;

namespace EHM_API.Services
{
    public class GuestService : IGuestService
    {
        private readonly IGuestRepository _guestRepository;

        public GuestService(IGuestRepository guestRepository)
        {
            _guestRepository = guestRepository;
        }

        public async Task<Guest> GetGuestByPhoneAsync(string guestPhone)
        {
            return await _guestRepository.GetGuestByPhoneAsync(guestPhone);
        }

        public async Task<Guest> AddGuestAsync(Guest guest)
        {
            return await _guestRepository.AddAsync(guest);
        }
    } 
}
