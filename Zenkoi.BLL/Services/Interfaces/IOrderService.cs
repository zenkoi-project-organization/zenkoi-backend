using Zenkoi.BLL.DTOs.OrderDTOs;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Queries;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDTO> CreateOrderAsync(CreateOrderDTO createOrderDTO);
        Task<OrderResponseDTO> GetOrderByIdAsync(int id);
        Task<OrderResponseDTO> GetOrderByOrderNumberAsync(string orderNumber);
        Task<IEnumerable<OrderResponseDTO>> GetOrdersByCustomerIdAsync(int customerId);
        Task<IEnumerable<OrderResponseDTO>> GetAllOrdersAsync(QueryOptions<Order>? queryOptions = null);
        Task<OrderResponseDTO> UpdateOrderStatusAsync(int id, UpdateOrderStatusDTO updateOrderStatusDTO);
        Task<bool> DeleteOrderAsync(int id);
        Task<decimal> CalculateOrderTotalAsync(CreateOrderDTO createOrderDTO);
    }
}
