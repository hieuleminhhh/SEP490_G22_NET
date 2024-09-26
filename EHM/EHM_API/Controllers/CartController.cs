using EHM_API.DTOs.CartDTO.Guest;
using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EHM_API.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class CartController : ControllerBase
	{
		private readonly ICartService _cartService;
		private readonly IComboService _comboService;
		private readonly IDishService _dishService;
        private readonly IOrderDetailService _orderDetailService;

        public CartController(ICartService cartService, IComboService comboService, IDishService dishService, IOrderDetailService orderDetailService)
        {
            _cartService = cartService;
            _comboService = comboService;
            _dishService = dishService;
            _orderDetailService = orderDetailService;
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
			var errors = new Dictionary<string, string>();

			if (checkoutDTO == null)
			{
				errors["checkoutData"] = "Dữ liệu thanh toán là bắt buộc.";
			}
			else
			{
				if (string.IsNullOrWhiteSpace(checkoutDTO.GuestPhone))
				{
					errors["guestPhone"] = "Số điện thoại không được bỏ trống.";
				}
				else if (!System.Text.RegularExpressions.Regex.IsMatch(checkoutDTO.GuestPhone, @"^\d{10,15}$"))
				{
					errors["guestPhone"] = "Số điện thoại không hợp lệ.";
				}

				if (string.IsNullOrWhiteSpace(checkoutDTO.GuestAddress))
				{
					errors["guestAddress"] = "Địa chỉ không được bỏ trống.";
				}

				if (string.IsNullOrWhiteSpace(checkoutDTO.ConsigneeName))
				{
					errors["consigneeName"] = "Tên người nhận không được bỏ trống.";
				}

				if (checkoutDTO.OrderDate == null)
				{
					errors["orderDate"] = "Ngày đặt hàng không được để trống.";
				}
/*				else if (checkoutDTO.OrderDate < DateTime.UtcNow)
				{
					errors["orderDate"] = "Ngày đặt hàng không hợp lệ.";
				}

				if (checkoutDTO.RecevingOrder < DateTime.UtcNow)
				{
					errors["receivingDate"] = "Ngày nhận không hợp lệ.";
				}*/

				if (checkoutDTO.Deposits < 0)
				{
					errors["deposit"] = "Tiền cọc không hợp lệ.";
				}

/*				if (checkoutDTO.TotalAmount <= 0)
				{
					errors["totalAmount"] = "Tổng tiền không hợp lệ.";
				}*/

				if (!string.IsNullOrWhiteSpace(checkoutDTO.Note) && checkoutDTO.Note.Length > 500)
				{
					errors["note"] = "Ghi chú không vượt quá 500 ký tự.";
				}

				else
				{
					foreach (var detail in checkoutDTO.OrderDetails)
					{
						if (detail.DishId.HasValue && detail.DishId.Value != 0)
						{
							if (!await _dishService.DishExistsAsync(detail.DishId.Value))
							{
								errors["dish"] = "Món ăn không tồn tại.";
							}
						}

						if (detail.ComboId.HasValue && detail.ComboId.Value != 0)
						{
							if (!await _comboService.ComboExistsAsync(detail.ComboId.Value))
							{
								errors["combo"] = "Combo không tồn tại.";
							}
						}

						if (detail.UnitPrice <= 0)
						{
							errors["OrderDetails"] = "Giá của món ăn hoặc combo phải lớn hơn 0.";
						}

						if (detail.Quantity <= 0)
						{
							errors["OrderDetails"] = "Số lượng phải lớn hơn 0.";
						}
					}

				}
			}
			if (errors.Any())
			{
				return BadRequest(errors);
			}
			try
			{
				await _cartService.Checkout(checkoutDTO);
				 _cartService.ClearCart();
				return Ok(new { message = "Đặt hàng thành công." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Đã xảy ra sự cố khi xử lý yêu cầu của bạn.", details = ex.Message });
			}
		}




		[HttpGet("checkoutsuccess/{guestPhone}")]
		public async Task<IActionResult> GetCheckoutSuccess(string guestPhone)
		{
			var checkoutSuccessInfo = await _cartService.GetCheckoutSuccessInfoAsync(guestPhone);

			if (checkoutSuccessInfo == null || string.IsNullOrWhiteSpace(checkoutSuccessInfo.GuestPhone))
			{
				return NotFound("Không tìm thấy thông tin đặt hàng cho số điện thoại của khách hàng.");
			}

			return Ok(checkoutSuccessInfo);
		}

		[Authorize(Roles = "OrderStaff,Cashier")]
		[HttpPost("AddNewOrder")]
        public async Task<IActionResult> AddNewOder([FromBody] TakeOutDTO checkoutDTO)
        {
			var errors = new Dictionary<string, string>();

			if (checkoutDTO == null)
			{
				errors["checkoutData"] = "Dữ liệu thanh toán là bắt buộc.";
			}

			if (checkoutDTO.OrderDetails == null || !checkoutDTO.OrderDetails.Any())
			{
				errors["cartItems"] = "Giỏ hàng không được để trống.";
			}


			if (errors.Any())
			{
				return BadRequest(errors);
			}

			try
			{
				await _cartService.TakeOut(checkoutDTO);
				_cartService.ClearCart();
				return Ok(new { message = "Tạo đơn hàng thành công." });
			}
			catch (Exception ex)
			{
				return BadRequest(new Dictionary<string, string>
				{
					["error"] = ex.Message
				});
			}
        }
        [Authorize(Roles = "OrderStaff,Cashier")]
        [HttpPost("AddNewOrderTakeAway")]
        public async Task<IActionResult> AddNewOder2([FromBody] TakeOutDTO takeoutDTO)
        {
            var errors = new Dictionary<string, string>();

            if (errors.Any())
            {
                return BadRequest(errors);
            }

            try
            {
                int orderId = await _cartService.TakeOut(takeoutDTO);
                _cartService.ClearCart();
                return Ok(new
                {
                    message = "Tạo đơn hàng thành công.",
					orderId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Dictionary<string, string>
                {
                    ["error"] = ex.Message
                });
            }
        }
        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetOrdersByAccountId(int accountId)
        {
            try
            {
                var orders = await _cartService.GetOrdersByAccountIdAsync(accountId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("GetRemainingItems")]
        public async Task<IActionResult> GetRemainingItems([FromBody] GetRemainingItemsRequestDTO request)
        {
            if ((request.ComboIds == null || !request.ComboIds.Any()) && (request.DishIds == null || !request.DishIds.Any()))
            {
                return BadRequest("ComboIds or DishIds must be provided.");
            }

            var result = await _orderDetailService.GetRemainingItemsAsync(request.ComboIds, request.DishIds);
            return Ok(result);
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
