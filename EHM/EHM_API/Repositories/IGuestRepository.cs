using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface IGuestRepository
    {
        Task<Guest> GetGuestByPhoneAsync(string guestPhone);
        Task<Guest> AddAsync(Guest guest);
    }
}
