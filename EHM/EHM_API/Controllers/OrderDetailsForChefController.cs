using EHM_API.DTOs.OrderDetailDTO.Manager;
using EHM_API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsForChefController : ControllerBase
    {
        private readonly IOrderDetailService _service;

        public OrderDetailsForChefController(IOrderDetailService service)
        {
            _service = service;
        }

        [HttpPut("{orderDetailId}/quantity")]
        public async Task<IActionResult> UpdateOrderDetailQuantity(int orderDetailId, [FromBody] int quantity)
        {
            if (quantity <= 0)
            {
                return BadRequest("Số lượng phải lớn hơn không.");
            }

            var result = await _service.UpdateOrderDetailQuantityAsync(orderDetailId, quantity);
            if (!result)
            {
                return NotFound("Không tìm được OrderDetailID.");
            }

            return NoContent();
        }
        [HttpGet("1-4")]
        public async Task<IActionResult> GetOrderDetails()
        {
            try
            {
                var orderDetails = await _service.GetOrderDetailsAsync();

                if (orderDetails == null || !orderDetails.Any())
                {
                    return NotFound(new { message = "Không tìm thấy chi tiết đơn hàng." });
                }

                return Ok(new
                {
                    message = "Lấy thông tin chi tiết đơn hàng thành công.",
                    data = orderDetails
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Đã xảy ra lỗi khi lấy thông tin chi tiết đơn hàng. Lỗi: {ex.Message}" });
            }
        }

        [HttpGet("2-3")]
        public async Task<IActionResult> GetOrderDetails1()
        {
            try
            {
                var orderDetails = await _service.GetOrderDetails1Async();

                if (orderDetails == null || !orderDetails.Any())
                {
                    return NotFound(new { message = "Không tìm thấy chi tiết đơn hàng." });
                }

                return Ok(new
                {
                    message = "Lấy thông tin chi tiết đơn hàng thành công.",
                    data = orderDetails
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Đã xảy ra lỗi khi lấy thông tin chi tiết đơn hàng. Lỗi: {ex.Message}" });
            }
        }

        [HttpGet("summary")]
        public async Task<ActionResult<IEnumerable<OrderDetailForChefDTO>>> GetOrderDetailSummary()
        {
            var summary = await _service.GetOrderDetailSummaryAsync();
            return Ok(summary);
        }

        [HttpPut("update-dishes-served")]
        public async Task<IActionResult> UpdateDishesServed([FromBody] UpdateDishesServedDTO updateDishesServedDto)
        {
            if (updateDishesServedDto == null || updateDishesServedDto.OrderDetailId == 0)
            {
                return BadRequest(new { message = "OrderDetailId không hợp lệ." });
            }

            try
            {
                await _service.UpdateDishesServedAsync(updateDishesServedDto.OrderDetailId, updateDishesServedDto.DishesServed);
                return Ok(new { message = "Cập nhật DishesServed thành công." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Đã xảy ra lỗi khi cập nhật DishesServed. Lỗi: {ex.Message}" });
            }
        }
    

    [HttpGet("getByDishesServed")]
        public async Task<IActionResult> GetOrderDetailsByDishesServed([FromQuery] int? dishesServed)
        {
            try
            {
                var orderDetails = await _service.GetOrderDetailsByDishesServedAsync(dishesServed);
                if (orderDetails == null || !orderDetails.Any())
                {
                    return NotFound("Không tìm thấy chi tiết đơn hàng");
                }
                return Ok(orderDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi khi tìm kiếm chi tiết đơn hàng: {ex.Message}");
            }
        }

        [HttpGet("searchforstaff")]
        public async Task<ActionResult<IEnumerable<OrderDetailForStaff>>> SearchByDishOrComboNameAsync(string keyword)
        {
            var results = await _service.SearchByDishOrComboNameAsync(keyword);
            return Ok(results);
        }

        [HttpGet("stafftype1")]
        public async Task<ActionResult<IEnumerable<OrderDetailForStaffType1>>> GetOrderDetailsForStaffType1Async()
        {
            var results = await _service.GetOrderDetailsForStaffType1Async();
            return Ok(results);
        }
    }
}
