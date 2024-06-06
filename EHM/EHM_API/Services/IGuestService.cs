using EHM_API.Models;

namespace EHM_API.Services
{
    public interface IGuestService
    {
        Task<Guest> GetGuestByPhoneAsync(string guestPhone);
        Task<Guest> AddGuestAsync(Guest guest);
    }
}
