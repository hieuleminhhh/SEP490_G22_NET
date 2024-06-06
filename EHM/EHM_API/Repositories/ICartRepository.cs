using EHM_API.DTOs.CartDTO;
using EHM_API.Models;

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
}