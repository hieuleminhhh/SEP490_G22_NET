using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.DTOs.InvoiceDTO;
using EHM_API.DTOs.OrderDTO.Manager;

namespace EHM_API.Services
{
	public interface IInvoiceService
	{
		Task<InvoiceDetailDTO> GetInvoiceDetailAsync(int invoiceId);
		Task<int> CreateInvoiceForOrderAsync(int orderId, CreateInvoiceForOrderDTO createInvoiceDto);
		Task<int> CreateInvoiceForOrder(int orderId, CreateInvoiceForOrder2DTO createInvoiceDto);
		Task UpdateInvoiceAndOrderAsync(int orderId, UpdateInvoiceSuccessPaymentDTO dto);
		Task UpdateOrderStatusAsync(int orderId, UpdateStatusOrderDTO dto);
		Task UpdateInvoiceAndCreateGuestAsync(int invoiceId, UpdateInvoiceDTO dto);
		Task<InvoiceDetailDTO> GetInvoiceByOrderIdAsync(int orderId);
		Task<int> UpdateDepositAndCreateInvoiceAsync(int orderId, PrepaymentDTO dto);
		Task UpdateOrderAndInvoiceAsync(int orderId, InvoiceOfSitting dto);
		Task<IEnumerable<GetInvoiceAndOrderInfo>> GetAllInvoicesAndOrdersAsync();
		Task<IEnumerable<GetOrderCancelInfo>> GetOrdersWithStatusAndDepositAsync(int status, decimal minDeposit);

		Task<IEnumerable<GetOrderUnpaidOfShipDTO>> GetOrdersUnpaidForShipAsync();
		Task<bool> UpdatePaymentStatusAsync(int orderId, UpdatePaymentStatusDTO dto);
        Task<UpdateAmountInvoiceDTO?> UpdateInvoiceAsync(UpdateAmountInvoiceDTO updateAmountInvoiceDTO);
    }
}
