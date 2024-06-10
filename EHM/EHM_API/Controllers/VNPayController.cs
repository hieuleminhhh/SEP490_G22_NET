using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using EHM_API.Models;
using EHM_API.DTOs.VNPayDTO;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace EHM_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VnpayController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly EHMDBContext _context;

		public VnpayController(IConfiguration configuration, EHMDBContext context)
		{
			_configuration = configuration;
			_context = context;
		}

		[HttpGet("simulate-payment")]
		public IActionResult SimulatePayment([FromQuery] VNPayResponseDTO vnpayData)
		{
			if (vnpayData == null || string.IsNullOrEmpty(vnpayData.vnp_TxnRef))
			{
				return BadRequest(new { RspCode = "99", Message = "Input data required" });
			}

			// Giả lập thông tin thanh toán
			long orderId = Convert.ToInt64(vnpayData.vnp_TxnRef);
			decimal vnp_Amount = Convert.ToDecimal(vnpayData.vnp_Amount) / 100;
			long vnpayTranId = Convert.ToInt64(vnpayData.vnp_TransactionNo);
			string vnp_ResponseCode = vnpayData.vnp_ResponseCode ?? "00"; // Giả lập thanh toán thành công
			string vnp_TransactionStatus = vnpayData.vnp_TransactionStatus ?? "00"; // Giả lập trạng thái giao dịch thành công

			var order = _context.Orders.SingleOrDefault(o => o.OrderId == orderId);
			if (order != null)
			{
				if (order.Deposits == vnp_Amount)
				{
					if (order.Status == 0) // Trạng thái chờ thanh toán
					{
						if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
						{
							order.Deposits = (decimal)order.TotalAmount;
							order.Status = 1; // Đã thanh toán
							_context.SaveChanges();

							// Tạo QR code
							var qrCodeUrl = GenerateQRCodeUrl(orderId, vnp_Amount);
							var qrCodeImage = GenerateQRCodeImage(qrCodeUrl);

							// Trả về hình ảnh QR code
							return File(qrCodeImage, "image/png");
						}
						else
						{
							return Ok(new { RspCode = "01", Message = "Transaction Failed" });
						}
					}
					else
					{
						return Ok(new { RspCode = "02", Message = "Order already confirmed" });
					}
				}
				else
				{
					return Ok(new
					{
						RspCode = "04",
						Message = "Invalid amount",
						ExpectedAmount = order.TotalAmount,
						ReceivedAmount = vnp_Amount
					});
				}
			}
			else
			{
				return Ok(new { RspCode = "01", Message = "Order not found" });
			}
		}

		private string GenerateQRCodeUrl(long orderId, decimal amount)
		{
			// Tạo URL chứa thông tin thanh toán cho QR code
			var url = $"{Request.Scheme}://{Request.Host}/api/vnpay/simulate-payment?vnp_TxnRef={orderId}&vnp_Amount={amount}&vnp_ResponseCode=00&vnp_TransactionStatus=00";
			return url;
		}


		private byte[] GenerateQRCodeImage(string qrCodeUrl)
		{
			QRCodeGenerator qrGenerator = new QRCodeGenerator();
			QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeUrl, QRCodeGenerator.ECCLevel.Q);
			Base64QRCode qrCode = new Base64QRCode(qrCodeData);
			string qrCodeImageAsBase64 = qrCode.GetGraphic(6);

			// Chuyển đổi chuỗi base64 thành byte array
			byte[] qrCodeImageBytes = Convert.FromBase64String(qrCodeImageAsBase64);

			return qrCodeImageBytes;
		}
	}
}
