using Microsoft.Extensions.Configuration;
using Net.payOS.Types;
using Zenkoi.BLL.DTOs.OrderDTOs;
using Zenkoi.BLL.DTOs.PaymentDTOs;
using Zenkoi.BLL.DTOs.PayOSDTOs;
using Zenkoi.BLL.DTOs.VnPayDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.UnitOfWork;
using Microsoft.AspNetCore.Http;
using static Zenkoi.DAL.Enums.PaymentTransactionStatus;

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
            var feURL = _configuration["FrontendURL"];

            // Begin transaction with Serializable isolation to prevent retry payment race conditions
            await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            try
            {
                var existingTransactions = await _unitOfWork.PaymentTransactions.GetAllAsync(
                    new QueryBuilder<PaymentTransaction>()
                    .WithPredicate(pt => pt.ActualOrderId == order.Id &&
                                        pt.PaymentMethod == "PayOS" &&
                                        (pt.Status == Pending || pt.Status == Failed))
                    .WithTracking(true)
                    .Build());

                bool isRetry = existingTransactions.Any();

                foreach (var existingTxn in existingTransactions)
                {
                    existingTxn.Status = Cancelled;
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
                    Status = Pending,
                    PaymentUrl = "",
                    ResponseData = null,
                    TransactionId = null,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.PaymentTransactions.CreateAsync(paymentTransaction);
                await _unitOfWork.SaveChangesAsync();

                var orderCode = paymentTransaction.Id;

                var description = $"DH {order.OrderNumber}";
                if (description.Length > 25)
                {
                    description = description.Substring(0, 25);
                }

                var beURL = _configuration["BackendURL"] ?? "https://localhost:7087";

                // Build items list from order details
                var items = order.OrderDetails.Select(od => new ItemData(
                    name: od.KoiFish?.RFID ?? od.PacketFish?.Name ?? "Product",
                    quantity: od.Quantity,
                    price: (int)od.UnitPrice
                )).ToList();

                // Add shipping fee as separate item if exists
                if (order.ShippingFee > 0)
                {
                    items.Add(new ItemData(
                        name: "Phí vận chuyển",
                        quantity: 1,
                        price: (int)order.ShippingFee
                    ));
                }

                var paymentData = new PaymentData(
                    orderCode: orderCode,
                    amount: (int)order.TotalAmount,
                    description: description,
                    items: items,
                    cancelUrl: $"{feURL}/checkout/failure",
                    returnUrl: $"{beURL}/api/Payments/payos-return?orderCode={orderCode}"
                );

                var createPayment = await _payOSService.CreatePaymentLinkAsync(paymentData);

                paymentTransaction.PaymentUrl = createPayment.checkoutUrl;
                paymentTransaction.OrderId = orderCode.ToString();
                await _unitOfWork.PaymentTransactions.UpdateAsync(paymentTransaction);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return new PaymentResponseDTO
                {
                    PaymentUrl = createPayment.checkoutUrl,
                    OrderId = order.Id,
                    IsRetry = isRetry
                };
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        private async Task<PaymentResponseDTO> CreateVnPayPaymentAsync(OrderResponseDTO order, Customer? customer)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new Exception("HTTP context not available");
            }

            // Begin transaction with Serializable isolation to prevent retry payment race conditions
            await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            try
            {
                var existingTransactions = await _unitOfWork.PaymentTransactions.GetAllAsync(
                    new QueryBuilder<PaymentTransaction>()
                    .WithPredicate(pt => pt.ActualOrderId == order.Id &&
                                        pt.PaymentMethod == "VnPay" &&
                                        (pt.Status == Pending || pt.Status == Failed))
                    .WithTracking(true)
                    .Build());

                bool isRetry = existingTransactions.Any();

                foreach (var existingTxn in existingTransactions)
                {
                    existingTxn.Status = Cancelled;
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
                    Status = Pending,
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

                await _unitOfWork.CommitTransactionAsync();

                return new PaymentResponseDTO
                {
                    PaymentUrl = paymentUrl,
                    OrderId = order.Id,
                    IsRetry = isRetry
                };
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
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

            if (status == "success" && paymentTransaction.Status == Success)
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

                await _orderService.UpdateInventoryAfterPaymentSuccessAsync(orderId);

                await _unitOfWork.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<string> CreatePayOSPaymentLinkAsync(int userId, PayOSPaymentRequestDTO request)
        {
            var feURL = _configuration["FrontendURL"];
            var beURL = _configuration["BaseURL"] ?? "https://localhost:7087";
          
            var paymentTransaction = new PaymentTransaction
            {
                UserId = userId,
                PaymentMethod = "PayOS",
                OrderId = "", 
                ActualOrderId = request.ActualOrderId,
                Amount = request.Amount,
                Description = request.Description,
                Status = Pending,
                PaymentUrl = "",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PaymentTransactions.CreateAsync(paymentTransaction);
            await _unitOfWork.SaveChangesAsync();

            var orderCode = paymentTransaction.Id;

            PaymentData paymentData = new PaymentData(
                orderCode: orderCode,
                amount: request.Amount,
                description: request.Description,
                items: request.Items,
                cancelUrl: $"{feURL}/checkout/failure",
                returnUrl: $"{beURL}/api/Payments/payos-return?orderCode={orderCode}"
            );

            CreatePaymentResult createPayment = await _payOSService.CreatePaymentLinkAsync(paymentData);

            paymentTransaction.OrderId = orderCode.ToString();
            paymentTransaction.PaymentUrl = createPayment.checkoutUrl;
            await _unitOfWork.PaymentTransactions.UpdateAsync(paymentTransaction);
            await _unitOfWork.SaveChangesAsync();

            return createPayment.checkoutUrl;
        }

        public async Task<string> CreateVnPayPaymentUrlAsync(int userId, VnPayRequestDTO request)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new Exception("HTTP context not available");
            }

            var paymentTransaction = new PaymentTransaction
            {
                UserId = userId,
                PaymentMethod = "VnPay",
                OrderId = "",
                ActualOrderId = request.OrderId.HasValue ? (int)request.OrderId.Value : null,
                Amount = (decimal)request.Amount,
                Description = request.Description,
                Status = Pending,
                PaymentUrl = "",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PaymentTransactions.CreateAsync(paymentTransaction);
            await _unitOfWork.SaveChangesAsync();

            var txnRef = paymentTransaction.Id;
            request.OrderId = txnRef;

            var paymentUrl = await _vnPayService.CreatePaymentUrlAsync(userId, httpContext, request);

            paymentTransaction.OrderId = txnRef.ToString();
            paymentTransaction.PaymentUrl = paymentUrl;
            await _unitOfWork.PaymentTransactions.UpdateAsync(paymentTransaction);
            await _unitOfWork.SaveChangesAsync();

            return paymentUrl;
        }

        public async Task<IEnumerable<PaymentTransaction>> GetPaymentHistoryAsync(int userId)
        {
            var paymentHistory = await _unitOfWork.PaymentTransactions.GetAllAsync(
                new QueryBuilder<PaymentTransaction>()
                .WithPredicate(pt => pt.UserId == userId)
                .WithOrderBy(q => q.OrderByDescending(pt => pt.CreatedAt))
                .Build());

            return paymentHistory;
        }

        public async Task<PaymentTransaction?> GetPaymentStatusAsync(string orderId)
        {
            return await _unitOfWork.PaymentTransactions.GetSingleAsync(
                new QueryBuilder<PaymentTransaction>()
                .WithPredicate(pt => pt.OrderId == orderId)
                .Build());
        }

        public async Task<PaymentStatusResponseDTO> CheckPaymentStatusByOrderCodeAsync(int orderCode)
        {
            var paymentTransaction = await _unitOfWork.PaymentTransactions.GetSingleAsync(
                new QueryBuilder<PaymentTransaction>()
                .WithPredicate(pt => pt.Id == orderCode)
                .Build());

            if (paymentTransaction == null)
            {
                throw new Exception("Payment transaction not found");
            }

            return new PaymentStatusResponseDTO
            {
                IsSuccess = paymentTransaction.Status == Success,
                Status = paymentTransaction.Status.ToString(),
                OrderId = paymentTransaction.ActualOrderId,
                Amount = paymentTransaction.Amount,
                PaymentMethod = paymentTransaction.PaymentMethod,
                TransactionId = paymentTransaction.TransactionId,
                CreatedAt = paymentTransaction.CreatedAt,
                UpdatedAt = paymentTransaction.UpdatedAt
            };
        }
    }
}
