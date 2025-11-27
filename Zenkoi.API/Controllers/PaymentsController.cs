using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Zenkoi.BLL.DTOs.PayOSDTOs;
using Zenkoi.BLL.DTOs.VnPayDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    public class PaymentsController : BaseAPIController
    {
        private readonly IPayOSService _payOSService;
        private readonly IConfiguration _configuration;
        private readonly IVnPayService _vnPayService;
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPayOSService payOSService, IConfiguration configuration, IVnPayService vnPayService, IPaymentService paymentService)
        {
            _payOSService = payOSService;
            _configuration = configuration;
            _vnPayService = vnPayService;
            _paymentService = paymentService;
        }

        [HttpGet("vnpay-return")]
        [AllowAnonymous]
        public async Task<IActionResult> VnPayReturn()
        {
            var feURL = _configuration["FrontendURL"];
            var result = await _vnPayService.ProcessVnPayReturnAsync(Request.Query);

            if (result.IsSuccess)
            {
                return Redirect($"{feURL}/payment-success/{result.OrderId}?method=VnPay&amount={result.Amount}");
            }
            else
            {
                var errorMessage = string.IsNullOrEmpty(result.ErrorMessage) ? "Payment+failed" : result.ErrorMessage.Replace(" ", "+");
                return Redirect($"{feURL}/payment-failure/{result.OrderId}?method=VnPay&code={result.ErrorCode}&message={errorMessage}");
            }
        }

        [HttpGet("payos-return")]
        [AllowAnonymous]
        public async Task<IActionResult> PayOSReturn([FromQuery] int orderCode, [FromQuery] string? status, [FromQuery] bool? cancel)
        {
            var feURL = _configuration["FrontendURL"];

            try
            {
                // Nếu user cancel thanh toán
                if (cancel == true || status == "CANCELLED")
                {
                    var cancelResult = await _paymentService.CheckPaymentStatusByOrderCodeAsync(orderCode);
                    return Redirect($"{feURL}/payment-failure/{cancelResult.OrderId ?? 0}?method=PayOS&code=CANCELLED&message=Payment+cancelled");
                }

                if (status == "PAID")
                {            
                    await _payOSService.ProcessPaymentReturnAsync(orderCode);
                }

                var result = await _paymentService.CheckPaymentStatusByOrderCodeAsync(orderCode);

                if (result.IsSuccess && result.OrderId.HasValue)
                {
                    return Redirect($"{feURL}/payment-success/{result.OrderId}?method=PayOS&amount={result.Amount}");
                }
                else if (result.Status == "Pending")
                {
                    return Redirect($"{feURL}/payment-pending/{result.OrderId ?? 0}?method=PayOS&orderCode={orderCode}");
                }
                else
                {
                    return Redirect($"{feURL}/payment-failure/{result.OrderId ?? 0}?method=PayOS&code=FAILED&message=Payment+failed");
                }
            }
            catch (Exception ex)
            {
                return Redirect($"{feURL}/payment-failure/0?method=PayOS&code=ERROR&message={ex.Message.Replace(" ", "+")}");
            }
        }

        [HttpPost("payos/create-payment-link")]
        public async Task<IActionResult> CreatePayOSPaymentLink([FromBody] PayOSPaymentRequestDTO request)
        {
            if (!ModelState.IsValid) return ModelInvalid();

            try
            {
                var paymentUrl = await _paymentService.CreatePayOSPaymentLinkAsync(UserId, request);
                return GetSuccess(paymentUrl);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet("payos/get-payment-link-infomation/{orderId}")]
        public async Task<IActionResult> GetPaymentLinkInformation(long orderId)
        {
            try
            {
                var response = await _payOSService.GetPaymentLinkInformationAsync(orderId);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost("payos/webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> HandleWebhook([FromBody] WebhookType payload)
        {
            var result = await _payOSService.ProcessWebhookAsync(payload);
            return Ok(result);
        }

        [HttpPost("vnpay/create-payment-url")]
        public async Task<IActionResult> CreateVnPayPaymentUrl([FromBody] VnPayRequestDTO request)
        {
            if (!ModelState.IsValid) return ModelInvalid();

            try
            {
                var paymentUrl = await _paymentService.CreateVnPayPaymentUrlAsync(UserId, request);
                return GetSuccess(paymentUrl);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet("history")]
        [Authorize]
        public async Task<IActionResult> GetPaymentHistory()
        {
            try
            {
                var paymentHistory = await _paymentService.GetPaymentHistoryAsync(UserId);

                var result = paymentHistory.Select(pt => new
                {
                    pt.Id,
                    pt.PaymentMethod,
                    pt.OrderId,
                    pt.Amount,
                    pt.Description,
                    pt.Status,
                    pt.CreatedAt,
                    pt.UpdatedAt
                });

                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet("status/{orderId}")]
        public async Task<IActionResult> GetPaymentStatus(string orderId)
        {
            try
            {
                var paymentTransaction = await _paymentService.GetPaymentStatusAsync(orderId);

                if (paymentTransaction == null)
                {
                    return GetNotFound("Không tìm thấy giao dịch thanh toán.");
                }

                return GetSuccess(new
                {
                    paymentTransaction.Status,
                    paymentTransaction.PaymentMethod,
                    paymentTransaction.Amount,
                    paymentTransaction.CreatedAt,
                    paymentTransaction.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet("check-status/{orderCode:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckPaymentStatusByOrderCode(int orderCode)
        {
            try
            {
                var result = await _paymentService.CheckPaymentStatusByOrderCodeAsync(orderCode);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

    }
}
