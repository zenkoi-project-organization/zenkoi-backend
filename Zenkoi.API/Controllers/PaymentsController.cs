using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net.payOS.Types;
using Zenkoi.BLL.DTOs.OrderDTOs;
using Zenkoi.BLL.DTOs.PayOSDTOs;
using Zenkoi.BLL.DTOs.VnPayDTOs;
using Zenkoi.BLL.Services.Implements;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
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
        private readonly IOrderService _orderService;

        public PaymentsController(IHttpContextAccessor httpContextAccessor, IPayOSService payOSService, IConfiguration configuration, IVnPayService vnPayService, IUnitOfWork unitOfWork, IOrderService orderService)
        {
            _httpContextAccessor = httpContextAccessor;
            _payOSService = payOSService;
            _configuration = configuration;
            _vnPayService = vnPayService;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
        }

        //[HttpGet("response-payment")]
        //public async Task<IActionResult> PaymentResponse()
        //{
        //    try
        //    {
        //        var vnpayRes = _vnPayService.PaymentExcute(Request.Query);

        //        if (!int.TryParse(vnpayRes.OrderId, out var transactionId))
        //        {
        //            throw new Exception("OrderId từ VnPay không hợp lệ.");
        //        }

        //        // Tìm và cập nhật trạng thái thanh toán
        //        var queryOptions = new QueryOptions<PaymentTransaction>
        //        {
        //            Predicate = pt => pt.OrderId == vnpayRes.OrderId
        //        };
        //        var paymentTransaction = await _unitOfWork.PaymentTransactions.GetSingleAsync(queryOptions);

        //        if (paymentTransaction != null)
        //        {
        //            paymentTransaction.Status = vnpayRes.IsSuccess ? "Success" : "Failed";
        //            paymentTransaction.TransactionId = vnpayRes.TransactionId.ToString();
        //            paymentTransaction.ResponseData = System.Text.Json.JsonSerializer.Serialize(vnpayRes);
        //            paymentTransaction.UpdatedAt = DateTime.UtcNow;

        //            await _unitOfWork.SaveChangesAsync();
        //        }

        //        if (!vnpayRes.IsSuccess)
        //        {
        //            // Thanh toán VnPay thất bại: trả thông tin lỗi
        //            return SaveError("Thanh toán không thành công: " + vnpayRes.VnPayResponseCode);
        //        }

        //        return SaveSuccess(vnpayRes);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Red;
        //        Console.WriteLine(ex.Message);
        //        Console.ResetColor();
        //        return Error($"Lỗi xử lý thanh toán: {ex.Message}");
        //    }
        //}

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn()
        {
            try
            {
                var vnpayRes = _vnPayService.PaymentExcute(Request.Query);
                var feURL = _configuration["FrontendURL"];         

                if (!vnpayRes.IsSuccess)
                {
                    return Redirect($"{feURL}/checkout/failure?code=97&message=Invalid+signature");
                }

                if (!int.TryParse(vnpayRes.OrderId, out var orderId))
                {
                    return Redirect($"{feURL}/checkout/failure?code=01&message=Invalid+order");
                }

                var paymentTransaction = await _unitOfWork.PaymentTransactions.GetSingleAsync(
                    new QueryBuilder<PaymentTransaction>()
                    .WithPredicate(pt => pt.OrderId == vnpayRes.OrderId || pt.ActualOrderId == orderId)
                    .Build());

                if (paymentTransaction == null)
                {               
                    return Redirect($"{feURL}/checkout/failure?code=01&message=Transaction+not+found");
                }

                if (paymentTransaction.Status == "Success")
                {

                    return Redirect($"{feURL}/checkout/success?orderId={orderId}&amount={vnpayRes.Amount}");
                }

                if (paymentTransaction.Status == "Failed")
                {
                    return Redirect($"{feURL}/checkout/failure?code={vnpayRes.VnPayResponseCode}&orderId={orderId}");
                }

                var order = await _orderService.GetOrderByIdAsync(paymentTransaction.ActualOrderId ?? orderId);
                if (order == null)
                {
                    return Redirect($"{feURL}/checkout/failure?code=01&message=Order+not+found");
                }

                if (Math.Abs(order.TotalAmount - (decimal)vnpayRes.Amount) > 0.01m)
                {                 

                    paymentTransaction.Status = "Failed";
                    paymentTransaction.ResponseData = System.Text.Json.JsonSerializer.Serialize(vnpayRes);
                    paymentTransaction.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.SaveChangesAsync();

                    return Redirect($"{feURL}/checkout/failure?code=04&message=Invalid+amount");
                }

                if (vnpayRes.VnPayResponseCode == "00")
                {
                    paymentTransaction.Status = "Success";
                    paymentTransaction.TransactionId = vnpayRes.TransactionId.ToString();
                    paymentTransaction.ResponseData = System.Text.Json.JsonSerializer.Serialize(vnpayRes);
                    paymentTransaction.UpdatedAt = DateTime.UtcNow;

                    await _orderService.UpdateOrderStatusAsync(
                        paymentTransaction.ActualOrderId ?? orderId,
                        new UpdateOrderStatusDTO { Status = OrderStatus.Paid }
                    );

                    var payment = new Payment
                    {
                        OrderId = paymentTransaction.ActualOrderId ?? orderId,
                        Method = PaymentMethod.VNPAY,
                        Amount = order.TotalAmount,
                        PaidAt = DateTime.UtcNow,
                        TransactionId = vnpayRes.TransactionId.ToString(),
                        Gateway = "VnPay",
                        UserId = paymentTransaction.UserId
                    };
                    await _unitOfWork.GetRepo<Payment>().CreateAsync(payment);

                    await _unitOfWork.SaveChangesAsync();

                    return Redirect($"{feURL}/checkout/success?orderId={orderId}&amount={vnpayRes.Amount}");
                }
                else
                {
                    paymentTransaction.Status = "Failed";
                    paymentTransaction.TransactionId = vnpayRes.TransactionId.ToString();
                    paymentTransaction.ResponseData = System.Text.Json.JsonSerializer.Serialize(vnpayRes);
                    paymentTransaction.UpdatedAt = DateTime.UtcNow;

                    await _orderService.UpdateOrderStatusAsync(
                        paymentTransaction.ActualOrderId ?? orderId,
                        new UpdateOrderStatusDTO { Status = OrderStatus.Created }
                    );

                    await _unitOfWork.SaveChangesAsync();

                    return Redirect($"{feURL}/checkout/failure?code={vnpayRes.VnPayResponseCode}&orderId={orderId}");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"VnPay Return Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                Console.ResetColor();

                var feURL = _configuration["FrontendURL"];
                return Redirect($"{feURL}/checkout/failure?message={ex.Message}");
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
                var userId = UserId;
                
                var paymentUrl = await _vnPayService.CreatePaymentUrlAsync(userId, HttpContext, request);

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
