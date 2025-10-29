using Zenkoi.BLL.DTOs.CartDTOs;
using Zenkoi.BLL.DTOs.OrderDTOs;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDTO> GetCartByCustomerIdAsync(int customerId);
        Task<CartItemResponseDTO> AddCartItemAsync(AddCartItemDTO addCartItemDTO, int customerId);
        Task<CartItemResponseDTO> UpdateCartItemAsync(int cartItemId, UpdateCartItemDTO updateCartItemDTO, int customerId);
        Task<bool> RemoveCartItemAsync(int cartItemId, int customerId);
        Task<bool> ClearCartAsync(int customerId);
        Task<CartResponseDTO> GetOrCreateCartForCustomerAsync(int customerId);
        Task<OrderResponseDTO> ConvertCartToOrderAsync(ConvertCartToOrderDTO convertCartToOrderDTO, int customerId);
    }
}

