using EHM_API.DTOs.GuestDTO.Manager;
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
        [HttpGet("ListAddress")]
        public async Task<IActionResult> GetListAddress()
        {
			var address = await _guestService.GetAllAddress();

            if (address == null)
            {
                return NotFound();
            }

            return Ok(address);
        }

		[HttpPost("CreateGuest")]
		public async Task<IActionResult> CreateGuest(CreateGuestDTO createGuestDTO)
		{
			try
			{
				var guestAddressInfoDTO = await _guestService.CreateGuestAndAddressAsync(createGuestDTO);

				if (guestAddressInfoDTO == null)
				{
					return BadRequest(new { message = "Thông tin khách hàng đã tồn tại." });
				}

				return Ok(new { message = "Thông tin khách hàng đã được tạo thành công." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Đã xảy ra lỗi khi xử lý yêu cầu của bạn." });
			}
		}


	}
}
