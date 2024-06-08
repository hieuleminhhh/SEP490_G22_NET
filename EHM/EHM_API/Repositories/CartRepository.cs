using EHM_API.Controllers;
using EHM_API.DTOs.CartDTO;
using EHM_API.Models;
using EHM_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHM_API.Repositories
{
	public class CartRepository : ICartRepository
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IDishService _dishService;
		private readonly IComboService _comboService;
		private readonly EHMDBContext _context;

		public CartRepository(IHttpContextAccessor httpContextAccessor, IDishService dishService, EHMDBContext context)
		{
			_httpContextAccessor = httpContextAccessor;
			_dishService = dishService;
			_context = context;
		}

		public List<Cart2DTO> GetCart()
		{
			if (_httpContextAccessor.HttpContext.Session.Keys.Contains("Cart2"))
			{
				return _httpContextAccessor.HttpContext.Session.Get<List<Cart2DTO>>("Cart2") ?? new List<Cart2DTO>();
			}
			return new List<Cart2DTO>();
		}

		public async Task AddToCart(Cart2DTO orderDTO)
		{
			var dish = await _dishService.GetDishByIdAsync(orderDTO.DishId);
			//	var combo = await _comboService.GetComboWithDishesAsync()

			if (dish == null)
			{
				throw new KeyNotFoundException("Invalid dish ID.");
			}

			orderDTO.ItemName = dish.ItemName;
			orderDTO.ItemDescription = dish.ItemDescription;
			orderDTO.Price = dish.Price;
			orderDTO.UnitPrice = dish.Price;
			orderDTO.TotalAmount = dish.Price * orderDTO.Quantity;
			orderDTO.ImageUrl = dish.ImageUrl;
			orderDTO.CategoryId = dish.CategoryId;

			var cart = GetCart();

			var existingOrder = cart.FirstOrDefault(x => x.DishId == orderDTO.DishId);
			if (existingOrder != null)
			{
				existingOrder.Quantity += orderDTO.Quantity;
				existingOrder.TotalAmount += orderDTO.TotalAmount;
			}
			else
			{
				cart.Add(orderDTO);
			}

			_httpContextAccessor.HttpContext.Session.Set("Cart2", cart);
		}

		public async Task UpdateCart(UpdateCartDTO updateCartDTO)
		{
			var cart = _httpContextAccessor.HttpContext.Session.Get<List<Cart2DTO>>("Cart2");
			if (cart == null)
			{
				throw new KeyNotFoundException("Cart not found.");
			}

			var item = cart.FirstOrDefault(x => x.DishId == updateCartDTO.DishId);
			if (item == null)
			{
				throw new KeyNotFoundException("Item not found in cart.");
			}

			if (updateCartDTO.Quantity < 0)
			{
				item.Quantity += updateCartDTO.Quantity;
				item.TotalAmount = item.Price * item.Quantity;
			}
			else
			{
				item.Quantity = updateCartDTO.Quantity;
				item.TotalAmount = item.Price * item.Quantity;
			}

			if (item.Quantity <= 0)
			{
				cart.Remove(item);
			}

			_httpContextAccessor.HttpContext.Session.Set("Cart2", cart);
		}


		//checkout Order
		public async Task CreateOrder(Order order)
		{
			await _context.Orders.AddAsync(order);
			await _context.SaveChangesAsync();
		}

		public void ClearCart()
		{
			if (_httpContextAccessor.HttpContext.Session.Keys.Contains("Cart2"))
			{
				_httpContextAccessor.HttpContext.Session.Remove("Cart2");
			}
		}
		public async Task<Guest> GetOrCreateGuest(CheckoutDTO checkoutDTO)
		{
			var guest = await _context.Guests
				.FirstOrDefaultAsync(g => g.GuestPhone == checkoutDTO.GuestPhone);

			if (guest != null)
			{
				// Cập nhật thông tin Guest nếu cần
				guest.Email = checkoutDTO.Email;
			}
			else
			{
				// Tạo mới Guest
				guest = new Guest
				{
					GuestPhone = checkoutDTO.GuestPhone,
					Email = checkoutDTO.Email
				};
				await _context.Guests.AddAsync(guest);
			}

			await _context.SaveChangesAsync();
			return guest;
		}


		public async Task<Address> GetOrCreateAddress(CheckoutDTO checkoutDTO)
		{
			var address = await _context.Addresses
				.FirstOrDefaultAsync(a =>
					a.GuestAddress == checkoutDTO.GuestAddress &&
					a.ConsigneeName == checkoutDTO.ConsigneeName &&
					a.GuestPhone == checkoutDTO.GuestPhone);

			if (address == null)
			{
				// Tạo mới Address
				address = new Address
				{
					GuestAddress = checkoutDTO.GuestAddress,
					ConsigneeName = checkoutDTO.ConsigneeName,
					GuestPhone = checkoutDTO.GuestPhone
				};
				await _context.Addresses.AddAsync(address);
				await _context.SaveChangesAsync();
			}

			return address;
		}

		public async Task<Dish> GetDishByIdAsync(int? dishId)
		{
			if (dishId == null)
			{
				return null;
			}

			return await _context.Dishes.FirstOrDefaultAsync(d => d.DishId == dishId);
		}


		public async Task<Order> GetOrderByGuestPhoneAsync(string guestPhone)
		{
			if (string.IsNullOrWhiteSpace(guestPhone))
			{
				return null;
			}

			return await _context.Orders
	   .Where(o => o.GuestPhone == guestPhone)
	   .Include(o => o.GuestPhoneNavigation)
	   .ThenInclude(g => g.Addresses)
	   .Include(o => o.OrderDetails)
	   .ThenInclude(od => od.Dish)
	   .ThenInclude(d => d.Category)
	   .Include(o => o.OrderDetails)
	   .ThenInclude(od => od.Combo)
	   .OrderByDescending(o => o.OrderId)
	   .FirstOrDefaultAsync();
		}

	}
}
