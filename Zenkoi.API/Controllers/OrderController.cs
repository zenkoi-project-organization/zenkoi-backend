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

        /// <summary>
        /// Tạo đơn hàng mới
        /// </summary>
        /// <param name="createOrderDTO">Thông tin đơn hàng</param>
        /// <returns>Đơn hàng đã tạo</returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO createOrderDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _orderService.CreateOrderAsync(createOrderDTO, UserId);
                return SaveSuccess(result, "Tạo đơn hàng thành công");
            }
            catch (ArgumentException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi tạo đơn hàng: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy đơn hàng theo ID
        /// </summary>
        /// <param name="id">ID đơn hàng</param>
        /// <returns>Thông tin đơn hàng</returns>
        [HttpGet("{id:int}")]
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

        /// <summary>
        /// Lấy đơn hàng theo mã đơn hàng
        /// </summary>
        /// <param name="orderNumber">Mã đơn hàng</param>
        /// <returns>Thông tin đơn hàng</returns>
        [HttpGet("by-order-number/{orderNumber}")]
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

        /// <summary>
        /// Lấy danh sách đơn hàng của khách hàng
        /// </summary>
        /// <param name="customerId">ID khách hàng</param>
        /// <param name="filter">Bộ lọc đơn hàng</param>
        /// <param name="pageIndex">Trang hiện tại (mặc định: 1)</param>
        /// <param name="pageSize">Số lượng mục trên mỗi trang (mặc định: 10)</param>
        /// <returns>Danh sách đơn hàng đã phân trang</returns>
        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetOrdersByCustomerId(
            int customerId,
            [FromQuery] OrderFilterRequestDTO filter,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
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

        /// <summary>
        /// Lấy danh sách đơn hàng của user hiện tại
        /// </summary>
        /// <param name="filter">Bộ lọc đơn hàng</param>
        /// <param name="pageIndex">Trang hiện tại (mặc định: 1)</param>
        /// <param name="pageSize">Số lượng mục trên mỗi trang (mặc định: 10)</param>
        /// <returns>Danh sách đơn hàng đã phân trang</returns>
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

        /// <summary>
        /// Lấy tất cả đơn hàng với bộ lọc và phân trang (Admin only)
        /// </summary>
        /// <param name="filter">Bộ lọc đơn hàng</param>
        /// <param name="pageIndex">Trang hiện tại (mặc định: 1)</param>
        /// <param name="pageSize">Số lượng mục trên mỗi trang (mặc định: 10)</param>
        /// <returns>Danh sách đơn hàng đã phân trang</returns>
        [HttpGet("all")]
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

        /// <summary>
        /// Cập nhật trạng thái đơn hàng
        /// </summary>
        /// <param name="id">ID đơn hàng</param>
        /// <param name="updateOrderStatusDTO">Thông tin cập nhật trạng thái</param>
        /// <returns>Đơn hàng đã cập nhật</returns>
        [HttpPut("{id:int}/status")]
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

        /// <summary>
        /// Xóa đơn hàng
        /// </summary>
        /// <param name="id">ID đơn hàng</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id:int}")]
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

    }
}
