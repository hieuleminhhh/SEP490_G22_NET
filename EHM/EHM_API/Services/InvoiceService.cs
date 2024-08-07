﻿using AutoMapper;
using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.Models;
using EHM_API.Repositories;
using System.Numerics;

namespace EHM_API.Services
{
	public class InvoiceService : IInvoiceService
	{
		private readonly IInvoiceRepository _invoiceRepository;
		private readonly IOrderRepository _orderRepository;
		private readonly IMapper _mapper;

		public InvoiceService(IInvoiceRepository invoiceRepository, IMapper mapper, IOrderRepository orderRepository)
		{
			_invoiceRepository = invoiceRepository;
			_mapper = mapper;
			_orderRepository = orderRepository;
		}

		public async Task<InvoiceDetailDTO> GetInvoiceDetailAsync(int invoiceId)
		{
			var invoiceDetail = await _invoiceRepository.GetInvoiceDetailAsync(invoiceId);
			if (invoiceDetail == null)
			{
				return null;
			}

			invoiceDetail.ItemInvoice = CombineInvoiceItems(invoiceDetail.ItemInvoice);
			return invoiceDetail;
		}

		private IEnumerable<ItemInvoiceDTO> CombineInvoiceItems(IEnumerable<ItemInvoiceDTO> items)
		{
			return items
				.GroupBy(item => new { item.DishId, item.ComboId })
				.Select(g =>
				{
					var first = g.First();
					return new ItemInvoiceDTO
					{
						DishId = first.DishId,
						ItemName = first.ItemName,
						ComboId = first.ComboId,
						NameCombo = first.NameCombo,
						Price = first.Price,
						UnitPrice = g.Sum(item => item.UnitPrice),
						Quantity = g.Sum(item => item.Quantity)
					};
				})
				.ToList();
		}
		public async Task<int> CreateInvoiceForOrderAsync(int orderId, CreateInvoiceForOrderDTO createInvoiceDto)
        {
            return await _invoiceRepository.CreateInvoiceForOrderAsync(orderId, createInvoiceDto);
        }

		public async Task<int> CreateInvoiceForOrder(int orderId, CreateInvoiceForOrder2DTO createInvoiceDto)
		{
			return await _invoiceRepository.CreateInvoiceForOrder(orderId, createInvoiceDto);
		}

		public async Task UpdateInvoiceAndCreateGuestAsync(int invoiceId, UpdateInvoiceDTO dto)
		{
			await _invoiceRepository.UpdateInvoiceAndCreateGuestAsync(invoiceId, dto);
		}

		public async Task UpdateInvoiceAndOrderAsync(int orderId, UpdateInvoiceSuccessPaymentDTO dto)
		{
			var order = await _orderRepository.GetByIdAsync(orderId);
			if (order == null)
			{
				throw new KeyNotFoundException($"Không tìm thấy đơn hàng với ID {orderId}.");
			}

			var invoice = await _invoiceRepository.GetInvoiceByIdAsync(order.InvoiceId.Value);
			if (invoice == null)
			{
				throw new KeyNotFoundException($"Không tìm thấy hóa đơn với ID {order.InvoiceId}.");
			}

			invoice.PaymentStatus = dto.PaymentStatus;

			order.Status = dto.Status;

			invoice.AmountReceived = order.TotalAmount;

			await _invoiceRepository.UpdateInvoiceAsync(invoice);
			await _orderRepository.UpdateOrderAsync(order);
		}

		public async Task UpdateOrderStatusAsync(int orderId, UpdateStatusOrderDTO dto)
		{
			var order = await _orderRepository.GetByIdAsync(orderId);
			if (order == null)
			{
				throw new KeyNotFoundException($"Không tìm thấy đơn hàng với ID {orderId}.");
			}

			order.Status = dto.Status;
			await _orderRepository.UpdateOrderAsync(order);
		}

		public async Task<InvoiceDetailDTO> GetInvoiceByOrderIdAsync(int orderId)
		{
			var invoice = await _invoiceRepository.GetInvoiceByOrderIdAsync(orderId);
			if (invoice == null)
			{
				throw new KeyNotFoundException($"Không tìm thấy hóa đơn cho đơn hàng {orderId}");
			}

			var invoiceDetail = await _invoiceRepository.GetInvoiceDetailAsync(invoice.InvoiceId);
			if (invoiceDetail == null)
			{
				throw new KeyNotFoundException($"Không tìm thấy chi tiết hóa đơn cho hóa đơn {invoice.InvoiceId}");
			}

			invoiceDetail.ItemInvoice = CombineInvoiceItems(invoiceDetail.ItemInvoice);
			return invoiceDetail;
		}

	}
}
