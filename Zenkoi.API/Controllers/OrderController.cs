using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.OrderDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : BaseAPIController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Manager,SaleStaff")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var result = await _orderService.GetOrderByIdAsync(id);
                return GetSuccess(result);
            }
            catch (ArgumentException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy đơn hàng: {ex.Message}");
            }
        }

        [HttpGet("by-order-number/{orderNumber}")]
        [Authorize(Roles = "Manager,SaleStaff")]
        public async Task<IActionResult> GetOrderByOrderNumber(string orderNumber)
        {
            try
            {
                var result = await _orderService.GetOrderByOrderNumberAsync(orderNumber);
                return GetSuccess(result);
            }
            catch (ArgumentException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy đơn hàng: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetOrdersByCustomerId(
            int customerId,
            [FromQuery] OrderFilterRequestDTO filter,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            if (customerId != UserId && !User.IsInRole("Admin") && !User.IsInRole("Manager"))
                return Forbid();

            try
            {
                var result = await _orderService.GetOrdersByCustomerIdAsync(customerId, filter, pageIndex, pageSize);
                return GetPagedSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy danh sách đơn hàng: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("customer/me")]
        public async Task<IActionResult> GetOrdersByCurrentCustomerId(
            [FromQuery] OrderFilterRequestDTO filter,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _orderService.GetOrdersByCustomerIdAsync(UserId, filter, pageIndex, pageSize);
                return GetPagedSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy danh sách đơn hàng: {ex.Message}");
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Manager,SaleStaff")]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] OrderFilterRequestDTO filter,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _orderService.GetAllOrdersAsync(filter, pageIndex, pageSize);
                return GetPagedSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy danh sách đơn hàng: {ex.Message}");
            }
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Manager,SaleStaff")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDTO updateOrderStatusDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _orderService.UpdateOrderStatusAsync(id, updateOrderStatusDTO);
                return SaveSuccess(result, "Cập nhật trạng thái đơn hàng thành công");
            }
            catch (ArgumentException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi cập nhật trạng thái đơn hàng: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var result = await _orderService.DeleteOrderAsync(id);
                if (result)
                {
                    return SaveSuccess(new { message = "Xóa đơn hàng thành công" }, "Xóa đơn hàng thành công");
                }
                return GetNotFound("Không tìm thấy đơn hàng để xóa");
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi xóa đơn hàng: {ex.Message}");
            }
        }

        [HttpPost("{id:int}/restock-packetfish")]
        [Authorize(Roles = "Manager,FarmStaff")]
        public async Task<IActionResult> RestockPacketFish(int id)
        {
            try
            {
                var result = await _orderService.RestockOrderPacketFishAsync(id);
                return SaveSuccess(result, "Restock gói cá thành công");
            }
            catch (InvalidOperationException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi restock gói cá: {ex.Message}");
            }
        }

    }
}
