using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface IGuestRepository
    {
        Task<Guest> GetGuestByPhoneAsync(string guestPhone);
        Task<Guest> AddAsync(Guest guest);

		Task<Address> GetAddressByIdAsync(int addressId);

        Task<bool> GuestPhoneExistsAsync(string guestPhone);
        Task<IEnumerable<Address>> GetListAddress();

		Task AddAddressAsync(Address address);
        Task<Address> GetAddressAsync(string guestAddress, string consigneeName, string guestPhone);

        Task<Guest> AddGuestAndAddressAsync(Guest guest, Address address);

        Task<bool> GuestAndAddressExistsAsync(string guestAddress, string consigneeName, string guestPhone);
	}
}
