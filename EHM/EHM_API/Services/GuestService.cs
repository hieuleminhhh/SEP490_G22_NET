using AutoMapper;
using EHM_API.DTOs.GuestDTO.Guest;
using EHM_API.DTOs.GuestDTO.Manager;
using EHM_API.DTOs.OrderDTO.Guest;
using EHM_API.Models;
using EHM_API.Repositories;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IEnumerable<GuestAddressInfoDTO>> GetAllAddress()
        {
            var address = await _guestRepository.GetListAddress();
            var addressDtos = _mapper.Map<IEnumerable<GuestAddressInfoDTO>>(address);
            return addressDtos;
        }


		public async Task<GuestAddressInfoDTO> CreateGuestAndAddressAsync(CreateGuestDTO createGuestDTO)
		{
			Guest guest = null;

			if (!string.IsNullOrWhiteSpace(createGuestDTO.GuestPhone))
			{
				guest = await _guestRepository.GetGuestByPhoneAsync(createGuestDTO.GuestPhone);
				if (guest == null)
				{
					guest = new Guest
					{
						GuestPhone = createGuestDTO.GuestPhone,
						Email = createGuestDTO.Email
					};
					await _guestRepository.AddAsync(guest);
				}
				else
				{
					guest.Email = createGuestDTO.Email;
				}
			}

			var existingAddress = await _guestRepository.GetAddressAsync(
				createGuestDTO.GuestAddress,
				createGuestDTO.ConsigneeName,
				createGuestDTO.GuestPhone);

			if (existingAddress == null)
			{
				var address = new Address
				{
					GuestAddress = createGuestDTO.GuestAddress,
					ConsigneeName = createGuestDTO.ConsigneeName,
					GuestPhone = createGuestDTO.GuestPhone,
					GuestPhoneNavigation = guest
				};

				await _guestRepository.AddAddressAsync(address);

				var guestAddressInfoDTO = _mapper.Map<GuestAddressInfoDTO>(address);

				if (guest != null)
				{
					guestAddressInfoDTO.Email = guest.Email;
				}

				return guestAddressInfoDTO;
			}
			else
			{
				return null;
			}
		}


	}
}
