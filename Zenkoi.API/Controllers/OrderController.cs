using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.OrderDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
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

                var result = await _orderService.CreateOrderAsync(createOrderDTO);
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
        /// <returns>Danh sách đơn hàng</returns>
        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetOrdersByCustomerId(int customerId)
        {
            try
            {
                var result = await _orderService.GetOrdersByCustomerIdAsync(customerId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy danh sách đơn hàng: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy tất cả đơn hàng (Admin only)
        /// </summary>
        /// <param name="status">Trạng thái đơn hàng (optional)</param>
        /// <param name="customerId">ID khách hàng (optional)</param>
        /// <param name="startDate">Ngày bắt đầu (optional)</param>
        /// <param name="endDate">Ngày kết thúc (optional)</param>
        /// <returns>Danh sách đơn hàng</returns>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] string? status = null,
            [FromQuery] int? customerId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var queryOptions = new QueryOptions<Order>();

                // Apply filters
                if (!string.IsNullOrEmpty(status))
                {
                    if (Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
                    {
                        queryOptions.Predicate = o => o.Status == orderStatus;
                    }
                }

                if (customerId.HasValue)
                {
                    if (queryOptions.Predicate != null)
                    {
                        var existingPredicate = queryOptions.Predicate;
                        queryOptions.Predicate = o => existingPredicate.Compile()(o) && o.CustomerId == customerId.Value;
                    }
                    else
                    {
                        queryOptions.Predicate = o => o.CustomerId == customerId.Value;
                    }
                }

                if (startDate.HasValue || endDate.HasValue)
                {
                    var start = startDate ?? DateTime.MinValue;
                    var end = endDate ?? DateTime.MaxValue;

                    if (queryOptions.Predicate != null)
                    {
                        var existingPredicate = queryOptions.Predicate;
                        queryOptions.Predicate = o => existingPredicate.Compile()(o) && o.CreatedAt >= start && o.CreatedAt <= end;
                    }
                    else
                    {
                        queryOptions.Predicate = o => o.CreatedAt >= start && o.CreatedAt <= end;
                    }
                }

                var result = await _orderService.GetAllOrdersAsync(queryOptions);
                return GetSuccess(result);
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        /// <summary>
        /// Tính tổng tiền đơn hàng
        /// </summary>
        /// <param name="createOrderDTO">Thông tin đơn hàng</param>
        /// <returns>Tổng tiền</returns>
        [HttpPost("calculate-total")]
        public async Task<IActionResult> CalculateOrderTotal([FromBody] CreateOrderDTO createOrderDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var total = await _orderService.CalculateOrderTotalAsync(createOrderDTO);
                return GetSuccess(new { TotalAmount = total });
            }
            catch (ArgumentException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi tính tổng tiền: {ex.Message}");
            }
        }
    }
}
