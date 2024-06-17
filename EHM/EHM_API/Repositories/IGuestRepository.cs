using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface IGuestRepository
    {
        Task<Guest> GetGuestByPhoneAsync(string guestPhone);
        Task<Guest> AddAsync(Guest guest);

		Task<Address> GetAddressByIdAsync(int addressId);

		Task<bool> GuestPhoneExistsAsync(string guestPhone);

	}
}
