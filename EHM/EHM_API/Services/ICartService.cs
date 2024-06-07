using EHM_API.DTOs.CartDTO;

namespace EHM_API.Services
{
	public interface ICartService
	{
		List<Cart2DTO> GetCart();
		Task AddToCart(Cart2DTO orderDTO);
		void ClearCart();

		Task UpdateCart(UpdateCartDTO updateCartDTO);

		Task Checkout(CheckoutDTO checkoutDTO);

		Task<CheckoutSuccessDTO> GetCheckoutSuccessInfoAsync(string guestPhone);
	}
}
