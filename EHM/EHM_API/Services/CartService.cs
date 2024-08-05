using AutoMapper;
using EHM_API.DTOs.CartDTO.Guest;
using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.Models;
using EHM_API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace EHM_API.Services
{
	public class CartService : ICartService
	{
		private readonly ICartRepository _cartRepository;
		private readonly IComboRepository _comboRepository;
		private readonly IMapper _mapper;

		public CartService(ICartRepository cartRepository, IComboRepository comboRepository, IMapper mapper)
		{
			_cartRepository = cartRepository;
			_comboRepository = comboRepository;
			_mapper = mapper;
		}

		public List<Cart2DTO> GetCart()
		{
			return _cartRepository.GetCart();
		}

		public async Task AddToCart(Cart2DTO orderDTO)
		{
			await _cartRepository.AddToCart(orderDTO);
		}

		public async Task UpdateCart(UpdateCartDTO updateCartDTO)
		{
			await _cartRepository.UpdateCart(updateCartDTO);
		}

		public void ClearCart()
		{
			_cartRepository.ClearCart();
		}

		public async Task Checkout(CheckoutDTO checkoutDTO)
		{
			Guest guest = null;
			Address address = null;

			if (!string.IsNullOrWhiteSpace(checkoutDTO.GuestPhone) && !string.IsNullOrWhiteSpace(checkoutDTO.Email))
			{
				guest = await _cartRepository.GetOrCreateGuest(checkoutDTO);
			}

			if (!string.IsNullOrWhiteSpace(checkoutDTO.GuestAddress) && !string.IsNullOrWhiteSpace(checkoutDTO.ConsigneeName))
			{
				address = await _cartRepository.GetOrCreateAddress(checkoutDTO);
				checkoutDTO.AddressId = address?.AddressId;
			}

			decimal totalAmount = 0;

			var orderDetails = new List<OrderDetail>();

			foreach (var item in checkoutDTO.OrderDetails)
			{
				if ((item.DishId.HasValue && item.DishId.Value > 0) && (item.ComboId.HasValue && item.ComboId.Value > 0) ||
					(!item.DishId.HasValue || item.DishId.Value <= 0) && (!item.ComboId.HasValue || item.ComboId.Value <= 0))
				{
					throw new InvalidOperationException("Mỗi giỏ hàng phải có một món hoặc Combo, không được có cả hai hoặc không có món nào.");
				}

				Dish dish = null;
				Combo combo = null;

				if (item.DishId.HasValue && item.DishId.Value > 0)
				{
					dish = await _cartRepository.GetDishByIdAsync(item.DishId.Value);
					if (dish == null)
					{
						throw new KeyNotFoundException($"Món ăn với ID {item.DishId} không tồn tại.");
					}
				}

				if (item.ComboId.HasValue && item.ComboId.Value > 0)
				{
					combo = await _comboRepository.GetComboByIdAsync(item.ComboId.Value);
					if (combo == null)
					{
						throw new KeyNotFoundException($"Combo với ID {item.ComboId} không tồn tại.");
					}
				}


				var existingOrderDetail = orderDetails.FirstOrDefault(od =>
					(dish != null && od.DishId == dish.DishId) ||
					(combo != null && od.ComboId == combo.ComboId));

				if (existingOrderDetail != null)
				{

					existingOrderDetail.Quantity += item.Quantity;
					existingOrderDetail.UnitPrice += item.UnitPrice;
					totalAmount += (item.UnitPrice ?? 0m);
				}
				else
				{

					var orderDetail = new OrderDetail
					{
						DishId = dish != null ? (int?)dish.DishId : null,
						ComboId = combo != null ? (int?)combo.ComboId : null,
						Quantity = item.Quantity,
						UnitPrice = item.UnitPrice,
						DishesServed = 0,
						Note = item.Note,
						OrderTime = item.OrderTime
					};

					orderDetails.Add(orderDetail);
					totalAmount += (item.UnitPrice ?? 0m);
				}
			}
			var order = new Order
			{
				OrderDate = DateTime.Now,
				Status = checkoutDTO.Status ?? 0,
				RecevingOrder = checkoutDTO.RecevingOrder,
				GuestPhone = guest?.GuestPhone,
				TotalAmount = totalAmount,
				OrderDetails = orderDetails,
				Deposits = checkoutDTO.Deposits,
				AddressId = address?.AddressId,
				Note = checkoutDTO.Note,
				Type = checkoutDTO.Type,
				DiscountId = checkoutDTO.DiscountId
			};


			await _cartRepository.CreateOrder(order);
		}


		public async Task<CheckoutSuccessDTO> GetCheckoutSuccessInfoAsync(string guestPhone)
		{
			var order = await _cartRepository.GetOrderByGuestPhoneAsync(guestPhone);

			if (order == null)
			{
				return null;
			}

			return _mapper.Map<CheckoutSuccessDTO>(order);
		}

        public async Task<int> TakeOut(TakeOutDTO takeOutDTO)
        {
            return await _cartRepository.TakeOut(takeOutDTO);
        }


    }
}