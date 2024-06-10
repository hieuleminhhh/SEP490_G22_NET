using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EHM_API.Models;

namespace EHM_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MoMoQRController : ControllerBase
	{
		private readonly EHMDBContext _context;

		public MoMoQRController(EHMDBContext context)
		{
			_context = context;
		}

		[HttpGet("{orderId}")]
		public async Task<IActionResult> GenerateQRCode(int orderId)
		{
			// Lấy thông tin đơn hàng
			var order = await _context.Orders
				.Include(o => o.OrderDetails)
				.ThenInclude(od => od.Combo)
				.FirstOrDefaultAsync(o => o.OrderId == orderId);

			if (order == null || !order.TotalAmount.HasValue)
			{
				return NotFound("Order not found or total amount not specified.");
			}

			// Lấy combo từ OrderDetails
			var comboName = order.OrderDetails
				.FirstOrDefault(od => od.ComboId.HasValue)?.Combo?.NameCombo ?? "Order";

			// Tạo URL MoMo
			string phoneNumber = "0366116510"; // Số điện thoại nhận tiền MoMo
			decimal totalAmount = order.TotalAmount.Value; // Số tiền
			string momoUrl = $"https://qr.momo.vn/{phoneNumber}/?amount={totalAmount}&comment={Uri.EscapeDataString(comboName)}";

			// Tạo mã QR từ URL
			var qrCodeImage = GenerateQRCode(momoUrl);

			// Trả về hình ảnh QR code
			return File(qrCodeImage, "image/png");
		}

		private byte[] GenerateQRCode(string text)
		{
			using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
			{
				QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
				using (BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData))
				{
					// Lấy mảng byte chứa hình ảnh QR
					byte[] qrCodeImage = qrCode.GetGraphic(6);

					// Trả về mảng byte
					return qrCodeImage;
				}
			}
		}
	}
}
