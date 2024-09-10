using EHM_API.DTOs.CartDTO.Guest;
using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface ICartRepository
	{
		List<Cart2DTO> GetCart();
		Task AddToCart(Cart2DTO orderDTO);
		void ClearCart();

		Task UpdateCart(UpdateCartDTO updateCartDTO);

		Task CreateOrder(Order order);
		Task<Guest> GetOrCreateGuest(CheckoutDTO checkoutDTO);
		Task<Address> GetOrCreateAddress(CheckoutDTO checkoutDTO);
		Task<Dish> GetDishByIdAsync(int? dishId);

		Task<Order> GetOrderByGuestPhoneAsync(string guestPhone);

		Task CreateOrderTakeOut(Order order);
		Task<Guest> GetOrCreateGuestTakeOut(TakeOutDTO takeOutDTO);
		Task<Address?> GetOrCreateAddressTakeOut(TakeOutDTO takeOutDTO);

		Task<int> TakeOut(TakeOutDTO takeOutDTO);
		Task<List<Order>> GetOrdersByAccountIdAsync(int accountId);
    }
}
