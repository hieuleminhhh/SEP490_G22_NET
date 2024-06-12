using EHM_API.DTOs.GuestDTO;
using EHM_API.Models;

namespace EHM_API.Services
{
    public interface IGuestService
    {
        Task<Guest> GetGuestByPhoneAsync(string guestPhone);
        Task<Guest> AddGuestAsync(Guest guest);

		Task<GuestAddressInfoDTO> GetGuestAddressInfoAsync(int addressId);
	}
}
