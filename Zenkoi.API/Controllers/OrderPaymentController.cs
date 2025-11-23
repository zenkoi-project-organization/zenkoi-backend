using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderPaymentController : BaseAPIController
    {
        private readonly IPaymentService _paymentService;

        public OrderPaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Tạo payment link từ Order (PayOS hoặc VnPay)
        /// </summary>
        /// <param name="orderId">ID đơn hàng</param>
        /// <param name="method">Phương thức thanh toán (PayOS, VnPay)</param>
        /// <returns>URL thanh toán</returns>
        [HttpPost("create-payment/{orderId:int}")]
        public async Task<IActionResult> CreatePaymentFromOrder(int orderId, [FromQuery] string method = "VnPay")
        {
            try
            {
                var result = await _paymentService.CreatePaymentLinkAsync(orderId, method);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Error creating payment: {ex.Message}");
            }
        }

        /// <summary>
        /// Xử lý callback sau khi thanh toán thành công
        /// </summary>
        [HttpPost("payment-callback/{orderId:int}")]
        public async Task<IActionResult> PaymentCallback(int orderId, [FromQuery] string status)
        {
            try
            {
                var success = await _paymentService.ProcessPaymentCallbackAsync(orderId, status);

                if (success)
                {
                    return GetSuccess(new { message = "Payment completed successfully" });
                }

                return GetError("Payment not completed or already processed");
            }
            catch (Exception ex)
            {
                return GetError($"Error processing callback: {ex.Message}");
            }
        }
    }
}

