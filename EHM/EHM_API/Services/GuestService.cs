using AutoMapper;
using EHM_API.DTOs.GuestDTO.Guest;
using EHM_API.Models;
using EHM_API.Repositories;

namespace EHM_API.Services
{
    public class GuestService : IGuestService
    {
        private readonly IGuestRepository _guestRepository;
		private readonly IMapper _mapper;

		public GuestService(IGuestRepository guestRepository, IMapper mapper)
        {
            _guestRepository = guestRepository;
			_mapper = mapper;
        }

        public async Task<Guest> GetGuestByPhoneAsync(string guestPhone)
        {
            return await _guestRepository.GetGuestByPhoneAsync(guestPhone);
        }

        public async Task<Guest> AddGuestAsync(Guest guest)
        {
            return await _guestRepository.AddAsync(guest);
        }

		public async Task<GuestAddressInfoDTO> GetGuestAddressInfoAsync(int addressId)
		{
			var address = await _guestRepository.GetAddressByIdAsync(addressId);

			if (address == null)
			{
				return null;
			}

			var dto = _mapper.Map<GuestAddressInfoDTO>(address);

			
			if (address.GuestPhoneNavigation != null)
			{
				dto.Email = address.GuestPhoneNavigation.Email;
			}

			return dto;
		}

		public async Task<bool> GuestPhoneExistsAsync(string guestPhone)
		{
			return await _guestRepository.GuestPhoneExistsAsync(guestPhone);
		}


	} 
}
