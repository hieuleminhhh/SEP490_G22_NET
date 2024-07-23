using EHM_API.DTOs.CartDTO.Guest;
using EHM_API.DTOs.CartDTO.OrderStaff;
using System.Threading;

namespace EHM_API.Services
{
    public interface ICartService
	{
		List<Cart2DTO> GetCart();
		Task AddToCart(Cart2DTO orderDTO);
		void ClearCart();

		Task UpdateCart(UpdateCartDTO updateCartDTO);

		Task Checkout(CheckoutDTO checkoutDTO);

		Task<int> TakeOut(TakeOutDTO takeOutDTO);


        Task<CheckoutSuccessDTO> GetCheckoutSuccessInfoAsync(string guestPhone);
	}
}
