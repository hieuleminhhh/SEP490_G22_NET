using EHM_API.DTOs.GuestDTO.Guest;
using EHM_API.DTOs.GuestDTO.Manager;
using EHM_API.Models;

namespace EHM_API.Services
{
    public interface IGuestService
    {
        Task<Guest> GetGuestByPhoneAsync(string guestPhone);
        Task<Guest> AddGuestAsync(Guest guest);
		Task<GuestAddressInfoDTO> GetGuestAddressInfoAsync(int addressId);
        Task<bool> GuestPhoneExistsAsync(string guestPhone);
        Task<IEnumerable<GuestAddressInfoDTO>> GetAllAddress();

		Task<GuestAddressInfoDTO> CreateGuestAndAddressAsync(CreateGuestDTO createGuestDTO);
	}
}
