using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net.payOS.Types;
using Zenkoi.BLL.DTOs.PayOSDTOs;
using Zenkoi.BLL.DTOs.VnPayDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    public class PaymentsController : BaseAPIController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPayOSService _payOSService;
        private readonly IConfiguration _configuration;
        private readonly IVnPayService _vnPayService;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentsController(IHttpContextAccessor httpContextAccessor, IPayOSService payOSService, IConfiguration configuration, IVnPayService vnPayService, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _payOSService = payOSService;
            _configuration = configuration;
            _vnPayService = vnPayService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("response-payment")]
        public async Task<IActionResult> PaymentResponse()
        {
            try
            {
                var vnpayRes = _vnPayService.PaymentExcute(Request.Query);

                if (!int.TryParse(vnpayRes.OrderId, out var transactionId))
                {
                    throw new Exception("OrderId từ VnPay không hợp lệ.");
                }

                // Tìm và cập nhật trạng thái thanh toán
                var queryOptions = new QueryOptions<PaymentTransaction>
                {
                    Predicate = pt => pt.OrderId == vnpayRes.OrderId
                };
                var paymentTransaction = await _unitOfWork.PaymentTransactions.GetSingleAsync(queryOptions);

                if (paymentTransaction != null)
                {
                    paymentTransaction.Status = vnpayRes.IsSuccess ? "Success" : "Failed";
                    paymentTransaction.TransactionId = vnpayRes.TransactionId.ToString();
                    paymentTransaction.ResponseData = System.Text.Json.JsonSerializer.Serialize(vnpayRes);
                    paymentTransaction.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.SaveChangesAsync();
                }

                if (!vnpayRes.IsSuccess)
                {
                    // Thanh toán VnPay thất bại: trả thông tin lỗi
                    return SaveError("Thanh toán không thành công: " + vnpayRes.VnPayResponseCode);
                }

                return SaveSuccess(vnpayRes);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error($"Lỗi xử lý thanh toán: {ex.Message}");
            }
        }

        [HttpPost("payos/create-payment-link")]
        public async Task<IActionResult> CreatePayOSPaymentLink([FromBody] PayOSPaymentRequestDTO request)
        {
            if (!ModelState.IsValid) return ModelInvalid();

            try
            {              
             
                var feURL = _configuration["FronendURL"];

                PaymentData paymentData = new PaymentData(
                    orderCode: request.OrderCode,
                    amount: request.Amount,
                    description: request.Description,
                    items: request.Items,
                    cancelUrl: $"{feURL}/payment-cancel",
                    returnUrl: $"{feURL}/payment-success"
                );

                CreatePaymentResult createPayment = await _payOSService.CreatePaymentLinkAsync(paymentData);

                // Lưu thông tin thanh toán vào database
                var paymentTransaction = new PaymentTransaction
                {
                    UserId = UserId,
                    PaymentMethod = "PayOS",
                    OrderId = request.OrderCode.ToString(),
                    Amount = request.Amount,
                    Description = request.Description,
                    Status = "Pending",
                    PaymentUrl = createPayment.checkoutUrl,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.PaymentTransactions.CreateAsync(paymentTransaction);
                await _unitOfWork.SaveChangesAsync();

                return GetSuccess(createPayment.checkoutUrl);
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
        public async Task<IActionResult> HandleWebhook([FromBody] WebhookType payload)
        {
            try
            {
                var webhookData = _payOSService.VerifyPaymentWebhookData(payload);
                if (webhookData == null)
                {
                    return Ok(new PayOSWebhookResponse(-1, "fail", null));
                }

                // Tìm và cập nhật trạng thái thanh toán
                var webhookQueryOptions = new QueryOptions<PaymentTransaction>
                {
                    Predicate = pt => pt.OrderId == webhookData.orderCode.ToString()
                };
                var paymentTransaction = await _unitOfWork.PaymentTransactions.GetSingleAsync(webhookQueryOptions);

                if (paymentTransaction != null)
                {                  
                    paymentTransaction.Status = "Success";
                    paymentTransaction.TransactionId = webhookData.orderCode.ToString();
                    paymentTransaction.ResponseData = System.Text.Json.JsonSerializer.Serialize(webhookData);
                    paymentTransaction.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.SaveChangesAsync();
                }

                return Ok(new PayOSWebhookResponse(0, "Ok", null));
            }
            catch (Exception)
            {
                return Ok(new PayOSWebhookResponse(-1, "fail", null));
            }
        }

        [HttpPost("vnpay/create-payment-url")]
        public async Task<IActionResult> CreateVnPayPaymentUrl([FromBody] VnPayRequestDTO request)
        {
            if (!ModelState.IsValid) return ModelInvalid();

            try
            {
                // Lấy userId từ token
                var userId = UserId;
                
                var paymentUrl = await _vnPayService.CreatePaymentUrlAsync(userId, HttpContext, request);

                // Lưu thông tin thanh toán vào database
                var paymentTransaction = new PaymentTransaction
                {
                    UserId = userId,
                    PaymentMethod = "VnPay",
                    OrderId = request.OrderId?.ToString() ?? Guid.NewGuid().ToString(),
                    Amount = (decimal)request.Amount,
                    Description = request.Description,
                    Status = "Pending",
                    PaymentUrl = paymentUrl,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.PaymentTransactions.CreateAsync(paymentTransaction);
                await _unitOfWork.SaveChangesAsync();

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
                var userId = UserId;
                var queryOptions = new QueryOptions<PaymentTransaction>
                {
                    Predicate = pt => pt.UserId == userId
                };
                
                var paymentHistory = await _unitOfWork.PaymentTransactions.GetAllAsync(queryOptions);
                
                var result = paymentHistory
                    .OrderByDescending(pt => pt.CreatedAt)
                    .Select(pt => new
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
                var statusQueryOptions = new QueryOptions<PaymentTransaction>
                {
                    Predicate = pt => pt.OrderId == orderId
                };
                var paymentTransaction = await _unitOfWork.PaymentTransactions.GetSingleAsync(statusQueryOptions);

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

    }
}
