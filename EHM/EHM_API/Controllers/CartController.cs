using EHM_API.DTOs.CartDTO;
using EHM_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHM_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CartController : ControllerBase
	{
		private readonly ICartService _cartService;

		public CartController(ICartService cartService)
		{
			_cartService = cartService;
		}

		[HttpGet("view")]
		public IActionResult ViewCart()
		{
			var cart = _cartService.GetCart();
			return Ok(cart);
		}

		[HttpPost("add")]
		public async Task<IActionResult> AddToCart([FromBody] Cart2DTO orderDTO)
		{
			if (orderDTO == null || orderDTO.Quantity <= 0)
			{
				return BadRequest("Order is null or quantity is invalid.");
			}

			try
			{
				await _cartService.AddToCart(orderDTO);
				return Ok("Order added to cart.");
			}
			catch (KeyNotFoundException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("update")]
		public async Task<IActionResult> UpdateCart([FromBody] UpdateCartDTO updateCartDTO)
		{
			if (updateCartDTO == null || updateCartDTO.Quantity <= 0)
			{
				return BadRequest("Invalid input data.");
			}

			try
			{
				await _cartService.UpdateCart(updateCartDTO);
				return Ok("Cart updated successfully.");
			}
			catch (KeyNotFoundException ex)
			{
				return BadRequest(ex.Message);
			}
		}




		[HttpDelete("clear")]
		public IActionResult ClearCart()
		{
			_cartService.ClearCart();
			return Ok("Cart cleared successfully.");
		}

		[HttpPost("checkout")]
		public async Task<IActionResult> Checkout([FromBody] CheckoutDTO checkoutDTO)
		{
			if (checkoutDTO == null)
			{
				return BadRequest("Checkout data is null.");
			}

			if (string.IsNullOrWhiteSpace(checkoutDTO.GuestPhone))
			{
				return BadRequest("GuestPhone is required.");
			}

			if (checkoutDTO.CartItems == null || !checkoutDTO.CartItems.Any())
			{
				return BadRequest("Cart items are required.");
			}

			if (checkoutDTO.OrderDate == null)
			{
				return BadRequest("Order date is required.");
			}

			try
			{
				await _cartService.Checkout(checkoutDTO);
				_cartService.ClearCart();
				return Ok(new { message = "Checkout successful." });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


		[HttpGet("checkoutsuccess/{guestPhone}")]
		public async Task<IActionResult> GetCheckoutSuccess(string guestPhone)
		{
			var checkoutSuccessInfo = await _cartService.GetCheckoutSuccessInfoAsync(guestPhone);

			if (checkoutSuccessInfo == null || string.IsNullOrWhiteSpace(checkoutSuccessInfo.GuestPhone))
			{
				return NotFound("Không tìm thấy thông tin đơn hàng cho số điện thoại khách hàng này.");
			}

			return Ok(checkoutSuccessInfo);
		}


	}

	public static class SessionExtensions
	{
		public static void Set<T>(this ISession session, string key, T value)
		{
			session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value));
		}

		public static T Get<T>(this ISession session, string key)
		{
			var value = session.GetString(key);
			return value == null ? default : System.Text.Json.JsonSerializer.Deserialize<T>(value);
		}
	}
}
