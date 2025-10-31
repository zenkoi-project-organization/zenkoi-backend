using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.PayOSDTOs;
using Zenkoi.BLL.DTOs.VnPayDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.UnitOfWork;
using Net.payOS.Types;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderPaymentController : BaseAPIController
    {
        private readonly IOrderService _orderService;
        private readonly IPayOSService _payOSService;
        private readonly IVnPayService _vnPayService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public OrderPaymentController(
            IOrderService orderService,
            IPayOSService payOSService,
            IVnPayService vnPayService,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _orderService = orderService;
            _payOSService = payOSService;
            _vnPayService = vnPayService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        /// <summary>
        /// Tạo payment link từ Order (PayOS)
        /// </summary>
        /// <param name="orderId">ID đơn hàng</param>
        /// <param name="method">Phương thức thanh toán (PayOS, VnPay)</param>
        /// <returns>URL thanh toán</returns>
        [HttpPost("create-payment/{orderId:int}")]
        public async Task<IActionResult> CreatePaymentFromOrder(int orderId, [FromQuery] string method = "VnPay")
        {
            try
            {
                // Lấy Order
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    return GetNotFound("Order not found");
                }

                // Allow Created or PendingPayment (for retry)
                if (order.Status != OrderStatus.Created && order.Status != OrderStatus.PendingPayment)
                {
                    return GetError("Order status is not valid for payment. Order must be in Created or PendingPayment status.");
                }

                // Set order to PendingPayment when starting payment
                if (order.Status == OrderStatus.Created)
                {
                    await _orderService.UpdateOrderStatusAsync(orderId, new Zenkoi.BLL.DTOs.OrderDTOs.UpdateOrderStatusDTO
                    {
                        Status = OrderStatus.PendingPayment
                    });
                }

                var customer = await _unitOfWork.GetRepo<Customer>().GetSingleAsync(
                    new QueryBuilder<Customer>()
                    .WithPredicate(c => c.Id == order.CustomerId)
                    .WithInclude(c => c.ApplicationUser)
                    .Build());

                if (method.ToLower() == "payos")
                {
                    // PayOS
                    var feURL = _configuration["FronendURL"];
                    var orderCode = order.Id * 1000000 + order.Id; // Unique order code

                    var paymentData = new PaymentData(
                        orderCode: orderCode,
                        amount: (int)order.TotalAmount,
                        description: $"Thanh toán đơn hàng {order.OrderNumber}",
                        items: order.OrderDetails.Select(od => new ItemData(
                            name: od.KoiFish.RFID ?? od.PacketFish.Name ?? "Product",
                            quantity: od.Quantity,
                            price: (int)od.UnitPrice
                        )).ToList(),
                        cancelUrl: $"{feURL}/payment-cancel",
                        returnUrl: $"{feURL}/payment-success/{orderId}"
                    );

                    var createPayment = await _payOSService.CreatePaymentLinkAsync(paymentData);

                    // Check for existing PaymentTransaction (Pending or Failed) to reuse
                    var existingTransaction = await _unitOfWork.PaymentTransactions.GetSingleAsync(
                        new QueryBuilder<PaymentTransaction>()
                        .WithPredicate(pt => pt.ActualOrderId == orderId &&
                                            pt.PaymentMethod == "PayOS" &&
                                            (pt.Status == "Pending" || pt.Status == "Failed"))
                        .WithTracking(true)
                        .Build());

                    if (existingTransaction != null)
                    {
                        // Reuse existing transaction for retry
                        existingTransaction.PaymentUrl = createPayment.checkoutUrl;
                        existingTransaction.Status = "Pending";
                        existingTransaction.UpdatedAt = DateTime.UtcNow;
                        existingTransaction.Amount = order.TotalAmount;
                        existingTransaction.Description = $"Thanh toán đơn hàng {order.OrderNumber}";
                        existingTransaction.OrderId = orderCode.ToString();
                        existingTransaction.ResponseData = null;
                        existingTransaction.TransactionId = null;

                        await _unitOfWork.PaymentTransactions.UpdateAsync(existingTransaction);
                        await _unitOfWork.SaveChangesAsync();

                        return GetSuccess(new { paymentUrl = createPayment.checkoutUrl, orderId, isRetry = true });
                    }
                    else
                    {
                        // Create new PaymentTransaction
                        var paymentTransaction = new PaymentTransaction
                        {
                            UserId = customer?.ApplicationUser?.Id ?? 0,
                            PaymentMethod = "PayOS",
                            OrderId = orderCode.ToString(),
                            ActualOrderId = orderId,
                            Amount = order.TotalAmount,
                            Description = $"Thanh toán đơn hàng {order.OrderNumber}",
                            Status = "Pending",
                            PaymentUrl = createPayment.checkoutUrl,
                            ResponseData = null,
                            TransactionId = null,
                            CreatedAt = DateTime.UtcNow
                        };

                        await _unitOfWork.PaymentTransactions.CreateAsync(paymentTransaction);
                        await _unitOfWork.SaveChangesAsync();

                        return GetSuccess(new { paymentUrl = createPayment.checkoutUrl, orderId, isRetry = false });
                    }
                }
                else
                {
                    // VnPay
                    var paymentUrl = await _vnPayService.CreatePaymentUrlAsync(
                        customer?.ApplicationUser?.Id ?? 0,
                        HttpContext,
                        new VnPayRequestDTO
                        {
                            OrderId = orderId,
                            Amount = (double)order.TotalAmount,
                            Description = $"Thanh toán đơn hàng {order.OrderNumber}",
                            FullName = customer?.ApplicationUser?.FullName ?? "Customer",
                            CreatedDate = DateTime.Now
                        }
                    );

                    // Check for existing PaymentTransaction (Pending or Failed) to reuse
                    var existingTransaction = await _unitOfWork.PaymentTransactions.GetSingleAsync(
                        new QueryBuilder<PaymentTransaction>()
                        .WithPredicate(pt => pt.ActualOrderId == orderId &&
                                            pt.PaymentMethod == "VnPay" &&
                                            (pt.Status == "Pending" || pt.Status == "Failed"))
                        .WithTracking(true)
                        .Build());

                    if (existingTransaction != null)
                    {
                        // Reuse existing transaction for retry
                        existingTransaction.PaymentUrl = paymentUrl;
                        existingTransaction.Status = "Pending";
                        existingTransaction.UpdatedAt = DateTime.UtcNow;
                        existingTransaction.Amount = order.TotalAmount;
                        existingTransaction.Description = $"Thanh toán đơn hàng {order.OrderNumber}";
                        existingTransaction.ResponseData = null;
                        existingTransaction.TransactionId = null;

                        await _unitOfWork.PaymentTransactions.UpdateAsync(existingTransaction);
                        await _unitOfWork.SaveChangesAsync();

                        return GetSuccess(new { paymentUrl, orderId, isRetry = true });
                    }
                    else
                    {
                        // Create new PaymentTransaction
                        var paymentTransaction = new PaymentTransaction
                        {
                            UserId = customer?.ApplicationUser?.Id ?? 0,
                            PaymentMethod = "VnPay",
                            OrderId = order.OrderNumber,
                            ActualOrderId = orderId,
                            Amount = order.TotalAmount,
                            Description = $"Thanh toán đơn hàng {order.OrderNumber}",
                            Status = "Pending",
                            PaymentUrl = paymentUrl,
                            ResponseData = null,
                            TransactionId = null,
                            CreatedAt = DateTime.UtcNow
                        };

                        await _unitOfWork.PaymentTransactions.CreateAsync(paymentTransaction);
                        await _unitOfWork.SaveChangesAsync();

                        return GetSuccess(new { paymentUrl, orderId, isRetry = false });
                    }
                }
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
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    return GetNotFound("Order not found");
                }

                // Find payment transaction
                var paymentTransaction = await _unitOfWork.PaymentTransactions.GetSingleAsync(
                    new QueryBuilder<PaymentTransaction>()
                    .WithPredicate(pt => pt.ActualOrderId == orderId)
                    .Build());

                if (paymentTransaction == null)
                {
                    return GetError("Payment transaction not found");
                }

                if (status == "success" && paymentTransaction.Status == "Success")
                {
                    // Create Payment record
                    var payment = new Payment
                    {
                        OrderId = orderId,
                        Method = paymentTransaction.PaymentMethod == "PayOS" 
                            ? PaymentMethod.PayOS 
                            : PaymentMethod.VNPAY,
                        Amount = order.TotalAmount,
                        PaidAt = DateTime.UtcNow,
                        TransactionId = paymentTransaction.TransactionId,
                        Gateway = paymentTransaction.PaymentMethod,
                        UserId = paymentTransaction.UserId
                    };

                    await _unitOfWork.GetRepo<Payment>().CreateAsync(payment);
                    
                    // Update Order status
                    await _orderService.UpdateOrderStatusAsync(orderId, new Zenkoi.BLL.DTOs.OrderDTOs.UpdateOrderStatusDTO
                    {
                        Status = OrderStatus.Confirmed
                    });
                    
                    await _unitOfWork.SaveChangesAsync();

                    return GetSuccess(new { message = "Payment completed successfully", order });
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

