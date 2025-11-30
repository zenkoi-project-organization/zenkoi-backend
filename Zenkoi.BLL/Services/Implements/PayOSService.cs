using Net.payOS;
using Net.payOS.Types;
using Zenkoi.BLL.DTOs.OrderDTOs;
using Zenkoi.BLL.DTOs.PayOSDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.UnitOfWork;
using System.Data;
using Microsoft.EntityFrameworkCore;
using static Zenkoi.DAL.Enums.PaymentTransactionStatus;

namespace Zenkoi.BLL.Services.Implements
{
	public class PayOSService : IPayOSService
	{
		private readonly PayOS _payOS;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IOrderService _orderService;

		public PayOSService(PayOS payOS, IUnitOfWork unitOfWork, IOrderService orderService)
		{
			_payOS = payOS;
			_unitOfWork = unitOfWork;
			_orderService = orderService;
		}

		public async Task<PaymentLinkInformation> CancelPaymentLinkAsync(long orderId)
		{
			PaymentLinkInformation response = await _payOS.cancelPaymentLink(orderId);
			return response;
		}

		public async Task ConfirmWebhookAsync(string webhookUrl)
		{
			await _payOS.confirmWebhook(webhookUrl);
		}

		public async Task<CreatePaymentResult> CreatePaymentLinkAsync(PaymentData data)
		{
			CreatePaymentResult response = await _payOS.createPaymentLink(data);
			if (response == null) throw new Exception("Không thể tạo link thanh toán PayOS");

			return response;
		}

		public async Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(long orderId)
		{
			PaymentLinkInformation response = await _payOS.getPaymentLinkInformation(orderId);
			if (response == null) throw new Exception($"Không tìm thấy thông tin thanh toán cho đơn hàng: #{orderId}");

			return response;
		}

		public WebhookData VerifyPaymentWebhookData(WebhookType data)
		{
			WebhookData response = _payOS.verifyPaymentWebhookData(data);

			if (data.desc == "Ma giao dich thu nghiem" || data.desc == "VQRIO123")
			{
				return response;
			}

			return response;
		}

		public async Task<PayOSWebhookResponse> ProcessWebhookAsync(WebhookType payload)
		{
			try
			{
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine($"[PayOS Webhook] Received webhook");
				Console.ResetColor();

				var webhookData = VerifyPaymentWebhookData(payload);
				if (webhookData == null)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"[PayOS Webhook] Webhook verification failed");
					Console.ResetColor();
					return new PayOSWebhookResponse(-1, "fail", "Webhook verification failed");
				}

				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine($"[PayOS Webhook] OrderCode: {webhookData.orderCode}, Amount: {webhookData.amount}");
				Console.ResetColor();

				// Begin transaction with Serializable isolation level to prevent race conditions
				await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);

				try
				{
					var paymentTransaction = await _unitOfWork.PaymentTransactions.GetSingleAsync(
						new QueryBuilder<PaymentTransaction>()
						.WithPredicate(pt => pt.OrderId == webhookData.orderCode.ToString())
						.WithTracking(true)
						.Build());

					if (paymentTransaction == null)
					{
						await _unitOfWork.RollBackAsync();
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine($"[PayOS Webhook] Transaction not found for OrderId: {webhookData.orderCode}");
						Console.ResetColor();
						return new PayOSWebhookResponse(-1, "fail", "Transaction not found");
					}

					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"[PayOS Webhook] Found transaction ID: {paymentTransaction.Id}, Status: {paymentTransaction.Status}");
					Console.ResetColor();

					// Idempotency check - if already processed, return OK
					if (paymentTransaction.Status == Success)
					{
						await _unitOfWork.RollBackAsync();
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.WriteLine($"[PayOS Webhook] Payment already processed, returning OK");
						Console.ResetColor();
						return new PayOSWebhookResponse(0, "Ok", null);
					}

					// Check for duplicate using IdempotencyKey
					var idempotencyKey = $"{paymentTransaction.ActualOrderId}_{webhookData.orderCode}";
					var existingSuccessPayment = await _unitOfWork.PaymentTransactions.GetSingleAsync(
						new QueryBuilder<PaymentTransaction>()
						.WithPredicate(pt => pt.IdempotencyKey == idempotencyKey && pt.Status == Success)
						.Build());

					if (existingSuccessPayment != null)
					{
						await _unitOfWork.RollBackAsync();
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.WriteLine($"[PayOS Webhook] Payment with idempotency key {idempotencyKey} already exists");
						Console.ResetColor();
						return new PayOSWebhookResponse(0, "Ok", "Already processed");
					}

					// Get actual Order ID
					var actualOrderId = paymentTransaction.ActualOrderId;
					if (actualOrderId == null)
					{
						await _unitOfWork.RollBackAsync();
						return new PayOSWebhookResponse(-1, "fail", "Order ID not found");
					}

					var order = await _orderService.GetOrderByIdAsync(actualOrderId.Value);
					if (order == null)
					{
						await _unitOfWork.RollBackAsync();
						return new PayOSWebhookResponse(-1, "fail", "Order not found");
					}

					// Update payment transaction
					paymentTransaction.Status = Success;
					paymentTransaction.IdempotencyKey = idempotencyKey;
					paymentTransaction.TransactionId = webhookData.orderCode.ToString();
					paymentTransaction.ResponseData = System.Text.Json.JsonSerializer.Serialize(webhookData);
					paymentTransaction.UpdatedAt = DateTime.UtcNow;

					// Update Order status
					await _orderService.UpdateOrderStatusAsync(
						actualOrderId.Value,
						new UpdateOrderStatusDTO { Status = OrderStatus.Processing }
					);

					await _orderService.UpdateInventoryAfterPaymentSuccessAsync(actualOrderId.Value);

					// Create Payment record
					var payment = new Payment
					{
						OrderId = actualOrderId.Value,
						Method = PaymentMethod.PayOS,
						Amount = order.TotalAmount,
						PaidAt = DateTime.UtcNow,
						TransactionId = webhookData.orderCode.ToString(),
						Gateway = "PayOS",
						UserId = paymentTransaction.UserId
					};
					await _unitOfWork.GetRepo<Payment>().CreateAsync(payment);

					await _unitOfWork.SaveChangesAsync();
					await _unitOfWork.CommitTransactionAsync();

					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"[PayOS Webhook] Payment processed successfully for Order #{actualOrderId}");
					Console.ResetColor();

					return new PayOSWebhookResponse(0, "Ok", null);
				}
				catch (DbUpdateConcurrencyException)
				{
					await _unitOfWork.RollBackAsync();
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine($"[PayOS Webhook] Concurrency conflict - payment already processed by another request");
					Console.ResetColor();
					return new PayOSWebhookResponse(0, "Ok", "Already processed by another request");
				}
				catch (Exception)
				{
					await _unitOfWork.RollBackAsync();
					throw;
				}
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"[PayOS Webhook] Error: {ex.Message}");
				Console.ResetColor();
				return new PayOSWebhookResponse(-1, "fail", ex.Message);
			}
		}

		public async Task ProcessPaymentReturnAsync(int orderCode)
		{
			try
			{
				// Begin transaction with Serializable isolation level
				await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);

				try
				{
					var paymentTransaction = await _unitOfWork.PaymentTransactions.GetSingleAsync(
						new QueryBuilder<PaymentTransaction>()
						.WithPredicate(pt => pt.Id == orderCode && pt.PaymentMethod == "PayOS")
						.WithTracking(true)
						.Build());

					if (paymentTransaction == null)
					{
						await _unitOfWork.RollBackAsync();
						throw new Exception($"PaymentTransaction not found for orderCode: {orderCode}");
					}

					// Idempotency check
					if (paymentTransaction.Status == Success)
					{
						await _unitOfWork.RollBackAsync();
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.WriteLine($"[PayOS Return] Payment already processed");
						Console.ResetColor();
						return;
					}

					var actualOrderId = paymentTransaction.ActualOrderId;
					if (actualOrderId == null)
					{
						await _unitOfWork.RollBackAsync();
						throw new Exception("Order ID not found in PaymentTransaction");
					}

					// Check IdempotencyKey
					var idempotencyKey = $"{actualOrderId}_{orderCode}";
					var existingSuccessPayment = await _unitOfWork.PaymentTransactions.GetSingleAsync(
						new QueryBuilder<PaymentTransaction>()
						.WithPredicate(pt => pt.IdempotencyKey == idempotencyKey && pt.Status == Success)
						.Build());

					if (existingSuccessPayment != null)
					{
						await _unitOfWork.RollBackAsync();
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.WriteLine($"[PayOS Return] Payment already processed via webhook");
						Console.ResetColor();
						return;
					}

					var order = await _orderService.GetOrderByIdAsync(actualOrderId.Value);
					if (order == null)
					{
						await _unitOfWork.RollBackAsync();
						throw new Exception($"Order not found: {actualOrderId}");
					}

					var paymentInfo = await GetPaymentLinkInformationAsync(orderCode);
					if (paymentInfo.status != "PAID")
					{
						await _unitOfWork.RollBackAsync();
						throw new Exception($"Payment not confirmed by PayOS. Status: {paymentInfo.status}");
					}

					paymentTransaction.Status = Success;
					paymentTransaction.IdempotencyKey = idempotencyKey;
					paymentTransaction.TransactionId = orderCode.ToString();
					paymentTransaction.ResponseData = System.Text.Json.JsonSerializer.Serialize(paymentInfo);
					paymentTransaction.UpdatedAt = DateTime.UtcNow;

					await _orderService.UpdateOrderStatusAsync(
						actualOrderId.Value,
						new UpdateOrderStatusDTO { Status = OrderStatus.Processing }
					);

					// Update inventory (KoiFish SaleStatus, PacketFish stock)
					await _orderService.UpdateInventoryAfterPaymentSuccessAsync(actualOrderId.Value);

					var payment = new Payment
					{
						OrderId = actualOrderId.Value,
						Method = PaymentMethod.PayOS,
						Amount = order.TotalAmount,
						PaidAt = DateTime.UtcNow,
						TransactionId = orderCode.ToString(),
						Gateway = "PayOS",
						UserId = paymentTransaction.UserId
					};
					await _unitOfWork.GetRepo<Payment>().CreateAsync(payment);

					await _unitOfWork.SaveChangesAsync();
					await _unitOfWork.CommitTransactionAsync();

					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"[PayOS Return] Payment processed successfully");
					Console.ResetColor();
				}
				catch (DbUpdateConcurrencyException)
				{
					await _unitOfWork.RollBackAsync();
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine($"[PayOS Return] Concurrency conflict - payment already processed");
					Console.ResetColor();
					return;
				}
				catch (Exception)
				{
					await _unitOfWork.RollBackAsync();
					throw;
				}
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"[PayOS Return] Error: {ex.Message}");
				Console.ResetColor();
				throw;
			}
		}
	}
}
