using EHM_API.DTOs.GuestDTO.Manager;
using EHM_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

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


		[Authorize(Roles = "OrderStaff,Cashier")]
		[HttpPost("CreateGuest")]
		public async Task<IActionResult> CreateGuest(CreateGuestDTO createGuestDTO)
		{
			var errors = new Dictionary<string, string>();

			if (string.IsNullOrEmpty(createGuestDTO.ConsigneeName))
			{
				errors["consigneeName"] = "Tên khách hàng không được để trống.";
			}

			if (string.IsNullOrEmpty(createGuestDTO.GuestPhone))
			{
				errors["phone"] = "Số điện thoại không được để trống.";
			}
			else if (!Regex.IsMatch(createGuestDTO.GuestPhone, @"^[0-9]+$"))
			{
				errors["phone"] = "Số điện thoại không hợp lệ. Chỉ được chứa các chữ số.";
			}
			else if (createGuestDTO.GuestPhone.Length < 10 || createGuestDTO.GuestPhone.Length > 11)
			{
				errors["phone"] = "Số điện thoại phải có từ 10 đến 11 chữ số.";
			}

			if (errors.Any())
			{
				return BadRequest(errors);
			}
			try
			{
				var guestAddressInfoDTO = await _guestService.CreateGuestAndAddressAsync(createGuestDTO);

				return Ok(new { message = "Thông tin khách hàng đã được tạo thành công.", data = guestAddressInfoDTO });
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				if (ex.Message.Contains("Thông tin khách hàng và địa chỉ đã tồn tại"))
				{
					return Conflict(new { message = "Thông tin khách hàng đã tồn tại." });
				}
				return StatusCode(500, new { message = "Đã xảy ra lỗi khi xử lý yêu cầu của bạn." });
			}
		}

		[HttpGet("phoneExists/{guestPhone}")]
		public async Task<IActionResult> GuestPhoneExists(string guestPhone)
		{
			var exists = await _guestService.GuestPhoneExistsAsync(guestPhone);

			return Ok(new { Exists = exists });
		}

	}
}
