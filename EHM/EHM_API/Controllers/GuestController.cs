using EHM_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EHM_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GuestController : ControllerBase
	{
		private readonly IGuestService _guestService;

		public GuestController(IGuestService addressService)
		{
			_guestService = addressService;
		}

		[HttpGet("{addressId}")]
		public async Task<IActionResult> GetGuestAddressInfo(int addressId)
		{
			var guestAddressInfo = await _guestService.GetGuestAddressInfoAsync(addressId);

			if (guestAddressInfo == null)
			{
				return NotFound();
			}

			return Ok(guestAddressInfo);
		}


		[HttpGet("phoneExists/{guestPhone}")]
		public async Task<IActionResult> GuestPhoneExists(string guestPhone)
		{
			var exists = await _guestService.GuestPhoneExistsAsync(guestPhone);

			return Ok(new { Exists = exists });
		}
	}
}
