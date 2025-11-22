using Microsoft.Extensions.Configuration;
using Net.payOS.Types;
using Zenkoi.BLL.DTOs.OrderDTOs;
using Zenkoi.BLL.DTOs.PaymentDTOs;
using Zenkoi.BLL.DTOs.VnPayDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.UnitOfWork;
using Microsoft.AspNetCore.Http;

namespace Zenkoi.BLL.Services.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IOrderService _orderService;
        private readonly IPayOSService _payOSService;
        private readonly IVnPayService _vnPayService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentService(
            IOrderService orderService,
            IPayOSService payOSService,
            IVnPayService vnPayService,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _orderService = orderService;
            _payOSService = payOSService;
            _vnPayService = vnPayService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaymentResponseDTO> CreatePaymentLinkAsync(int orderId, string paymentMethod)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            if (order.Status != OrderStatus.Pending)
            {
                throw new Exception("Order status is not valid for payment. Order must be in Pending status.");
            }
            var customer = await _unitOfWork.GetRepo<Customer>().GetSingleAsync(
                new QueryBuilder<Customer>()
                .WithPredicate(c => c.Id == order.CustomerId)
                .WithInclude(c => c.ApplicationUser)
                .Build());

            if (paymentMethod.ToLower() == "payos")
            {
                return await CreatePayOSPaymentAsync(order, customer);
            }
            else
            {
                return await CreateVnPayPaymentAsync(order, customer);
            }
        }

        private async Task<PaymentResponseDTO> CreatePayOSPaymentAsync(OrderResponseDTO order, Customer? customer)
        {
            var feURL = _configuration["FronendURL"];

            var existingTransactions = await _unitOfWork.PaymentTransactions.GetAllAsync(
                new QueryBuilder<PaymentTransaction>()
                .WithPredicate(pt => pt.ActualOrderId == order.Id &&
                                    pt.PaymentMethod == "PayOS" &&
                                    (pt.Status == "Pending" || pt.Status == "Failed"))
                .WithTracking(true)
                .Build());

            bool isRetry = existingTransactions.Any();

            foreach (var existingTxn in existingTransactions)
            {
                existingTxn.Status = "Cancelled";
                existingTxn.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.PaymentTransactions.UpdateAsync(existingTxn);
            }

            var paymentTransaction = new PaymentTransaction
            {
                UserId = customer?.ApplicationUser?.Id ?? 0,
                PaymentMethod = "PayOS",
                OrderId = "",
                ActualOrderId = order.Id,
                Amount = order.TotalAmount,
                Description = $"Thanh toán đơn hàng {order.OrderNumber}",
                Status = "Pending",
                PaymentUrl = "",
                ResponseData = null,
                TransactionId = null,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PaymentTransactions.CreateAsync(paymentTransaction);
            await _unitOfWork.SaveChangesAsync();

            var orderCode = paymentTransaction.Id;

            var paymentData = new PaymentData(
                orderCode: orderCode,
                amount: (int)order.TotalAmount,
                description: $"Thanh toán đơn hàng {order.OrderNumber}",
                items: order.OrderDetails.Select(od => new ItemData(
                    name: od.KoiFish?.RFID ?? od.PacketFish?.Name ?? "Product",
                    quantity: od.Quantity,
                    price: (int)od.UnitPrice
                )).ToList(),
                cancelUrl: $"{feURL}/payment-cancel",
                returnUrl: $"{feURL}/payment-success/{order.Id}"
            );

            var createPayment = await _payOSService.CreatePaymentLinkAsync(paymentData);

            paymentTransaction.PaymentUrl = createPayment.checkoutUrl;
            paymentTransaction.OrderId = orderCode.ToString();
            await _unitOfWork.PaymentTransactions.UpdateAsync(paymentTransaction);
            await _unitOfWork.SaveChangesAsync();

            return new PaymentResponseDTO
            {
                PaymentUrl = createPayment.checkoutUrl,
                OrderId = order.Id,
                IsRetry = isRetry
            };
        }

        private async Task<PaymentResponseDTO> CreateVnPayPaymentAsync(OrderResponseDTO order, Customer? customer)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new Exception("HTTP context not available");
            }

            var existingTransactions = await _unitOfWork.PaymentTransactions.GetAllAsync(
                new QueryBuilder<PaymentTransaction>()
                .WithPredicate(pt => pt.ActualOrderId == order.Id &&
                                    pt.PaymentMethod == "VnPay" &&
                                    (pt.Status == "Pending" || pt.Status == "Failed"))
                .WithTracking(true)
                .Build());

            bool isRetry = existingTransactions.Any();

            foreach (var existingTxn in existingTransactions)
            {
                existingTxn.Status = "Cancelled";
                existingTxn.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.PaymentTransactions.UpdateAsync(existingTxn);
            }

            var paymentTransaction = new PaymentTransaction
            {
                UserId = customer?.ApplicationUser?.Id ?? 0,
                PaymentMethod = "VnPay",
                OrderId = "", 
                ActualOrderId = order.Id,
                Amount = order.TotalAmount,
                Description = $"Thanh toán đơn hàng {order.OrderNumber}",
                Status = "Pending",
                PaymentUrl = "",
                ResponseData = null,
                TransactionId = null,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PaymentTransactions.CreateAsync(paymentTransaction);
            await _unitOfWork.SaveChangesAsync();

            var txnRef = paymentTransaction.Id;

            var paymentUrl = await _vnPayService.CreatePaymentUrlAsync(
                customer?.ApplicationUser?.Id ?? 0,
                httpContext,
                new VnPayRequestDTO
                {
                    OrderId = txnRef,
                    Amount = (double)order.TotalAmount,
                    Description = $"Thanh toán đơn hàng {order.OrderNumber}",
                    FullName = customer?.ApplicationUser?.FullName ?? "Customer",
                    CreatedDate = DateTime.Now
                }
            );

            paymentTransaction.PaymentUrl = paymentUrl;
            paymentTransaction.OrderId = txnRef.ToString();
            await _unitOfWork.PaymentTransactions.UpdateAsync(paymentTransaction);
            await _unitOfWork.SaveChangesAsync();

            return new PaymentResponseDTO
            {
                PaymentUrl = paymentUrl,
                OrderId = order.Id,
                IsRetry = isRetry
            };
        }

        public async Task<bool> ProcessPaymentCallbackAsync(int orderId, string status)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            var paymentTransaction = await _unitOfWork.PaymentTransactions.GetSingleAsync(
                new QueryBuilder<PaymentTransaction>()
                .WithPredicate(pt => pt.ActualOrderId == orderId)
                .Build());

            if (paymentTransaction == null)
            {
                throw new Exception("Payment transaction not found");
            }

            if (status == "success" && paymentTransaction.Status == "Success")
            {
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

                await _orderService.UpdateOrderStatusAsync(orderId, new Zenkoi.BLL.DTOs.OrderDTOs.UpdateOrderStatusDTO
                {
                    Status = OrderStatus.Processing
                });

                await _unitOfWork.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }
}
