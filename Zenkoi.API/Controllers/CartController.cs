using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.CartDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.BLL.DTOs.OrderDTOs;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : BaseAPIController
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCartByCustomerId()
        {
            try
            {
                var result = await _cartService.GetCartByCustomerIdAsync(UserId);
                return GetSuccess(result);
            }
            catch (ArgumentException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy giỏ hàng: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetOrCreateCart()
        {
            try
            {
                var result = await _cartService.GetOrCreateCartForCustomerAsync(UserId);
                return GetSuccess(result);
            }
            catch (ArgumentException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi tạo/lấy giỏ hàng: {ex.Message}");
            }
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddCartItem([FromBody] AddCartItemDTO addCartItemDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _cartService.AddCartItemAsync(addCartItemDTO, UserId);
                return SaveSuccess(result, "Thêm sản phẩm vào giỏ hàng thành công");
            }
            catch (ArgumentException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi thêm sản phẩm vào giỏ hàng: {ex.Message}");
            }
        }

        [HttpPut("items/{cartItemId:int}")]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemDTO updateCartItemDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _cartService.UpdateCartItemAsync(cartItemId, updateCartItemDTO, UserId);
                return SaveSuccess(result, "Cập nhật giỏ hàng thành công");
            }
            catch (ArgumentException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi cập nhật giỏ hàng: {ex.Message}");
            }
        }

        [HttpDelete("items/{cartItemId:int}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            try
            {
                var result = await _cartService.RemoveCartItemAsync(cartItemId, UserId);
                if (result)
                {
                    return SaveSuccess("Xóa sản phẩm khỏi giỏ hàng thành công");
                }
                return GetError("Không thể xóa sản phẩm khỏi giỏ hàng");
            }
            catch (ArgumentException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi xóa sản phẩm khỏi giỏ hàng: {ex.Message}");
            }
        }

        [HttpDelete("customer/{customerId:int}")]
        public async Task<IActionResult> ClearCart(int customerId)
        {      
            if (customerId != UserId)
                return Forbid();

            try
            {
                var result = await _cartService.ClearCartAsync(customerId);
                if (result)
                {
                    return SaveSuccess("Xóa giỏ hàng thành công");
                }
                return GetError("Không thể xóa giỏ hàng");
            }
            catch (ArgumentException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi xóa giỏ hàng: {ex.Message}");
            }
        }

        [HttpPost("convert-to-order")]
        public async Task<IActionResult> ConvertCartToOrder([FromBody] ConvertCartToOrderDTO convertCartToOrderDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _cartService.ConvertCartToOrderAsync(convertCartToOrderDTO, UserId);
                return SaveSuccess(result, "Chuyển đổi giỏ hàng thành đơn hàng thành công");
            }
            catch (ArgumentException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi chuyển đổi giỏ hàng: {ex.Message}");
            }
        }
    }
}

