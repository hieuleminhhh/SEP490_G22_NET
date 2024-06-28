﻿using EHM_API.DTOs.CartDTO.Guest;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.Models;
using EHM_API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public class CartService : ICartService
	{
		private readonly ICartRepository _cartRepository;
		private readonly IComboRepository _comboRepository;

		public CartService(ICartRepository cartRepository, IComboRepository comboRepository)
		{
			_cartRepository = cartRepository;
			_comboRepository = comboRepository;
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
				Type = checkoutDTO.Type
				
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

			var guestAddress = order.GuestPhoneNavigation?.Addresses?.FirstOrDefault();

			var orderDetails = order.OrderDetails
				.Select(od => new OrderDetailsDTO
				{
					NameCombo = od.Combo?.NameCombo,
					ItemName = od.Dish?.ItemName,
					DishId = od.DishId ?? 0,
					ComboId = od.ComboId ?? 0,
					Price = od.Dish?.Price ?? od.Combo?.Price,
					DiscountedPrice = od.Dish?.Discount != null ? (od.Dish.Price - (od.Dish.Price * od.Dish.Discount.DiscountAmount / 100)) : od.Dish?.Price,
					UnitPrice = od.UnitPrice,
					Quantity = od.Quantity,
		
					ImageUrl = od.Dish?.ImageUrl ?? od.Combo?.ImageUrl
				})
				.ToList(); 

			var totalAmount = orderDetails.Sum(od => (od.UnitPrice ?? 0));

			var checkoutSuccessDTO = new CheckoutSuccessDTO
			{
				GuestPhone = order.GuestPhone,
				Email = order.GuestPhoneNavigation?.Email,
				AddressId = order?.Address.AddressId ?? 0,
				GuestAddress = order?.Address.GuestAddress,
				ConsigneeName = order?.Address.ConsigneeName,
				OrderId = order.OrderId,
				OrderDate = order.OrderDate,
				Status = order.Status ?? 0,
				ReceivingTime = order.RecevingOrder,
				TotalAmount = totalAmount,
				Note = order.Note,
				Deposits = order.Deposits,
				OrderDetails = orderDetails,
			};

			return checkoutSuccessDTO;
		}
	}
}
