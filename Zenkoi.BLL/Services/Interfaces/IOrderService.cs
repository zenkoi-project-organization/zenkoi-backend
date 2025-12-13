using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.OrderDTOs;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDTO> GetOrderByIdAsync(int id);
        Task<OrderResponseDTO> GetOrderByOrderNumberAsync(string orderNumber);
        Task<PaginatedList<OrderResponseDTO>> GetOrdersByCustomerIdAsync(int customerId, OrderFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10);
        Task<PaginatedList<OrderResponseDTO>> GetAllOrdersAsync(OrderFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10);
        Task<OrderResponseDTO> UpdateOrderStatusAsync(int id, UpdateOrderStatusDTO updateOrderStatusDTO);
        Task<bool> DeleteOrderAsync(int id);
        Task UpdateInventoryAfterPaymentSuccessAsync(int orderId);
        Task ValidateOrderItemsAvailabilityAsync(OrderResponseDTO order);
        Task ValidateCartItemsAvailabilityAsync(IEnumerable<CartItem> cartItems);
        Task CancelOrderAndReleaseInventoryAsync(int orderId);
        Task RestockOrderPacketFishAsync(int orderId);
    }
}
