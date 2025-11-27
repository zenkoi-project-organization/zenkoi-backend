using Net.payOS;
using Net.payOS.Types;
using Zenkoi.BLL.DTOs.OrderDTOs;
using Zenkoi.BLL.DTOs.PayOSDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.UnitOfWork;

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

				var paymentTransaction = await _unitOfWork.PaymentTransactions.GetSingleAsync(
					new QueryBuilder<PaymentTransaction>()
					.WithPredicate(pt => pt.OrderId == webhookData.orderCode.ToString())
					.WithTracking(true)
					.Build());

				if (paymentTransaction == null)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"[PayOS Webhook] Transaction not found for OrderId: {webhookData.orderCode}");
					Console.ResetColor();
					return new PayOSWebhookResponse(-1, "fail", "Transaction not found");
				}

				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"[PayOS Webhook] Found transaction ID: {paymentTransaction.Id}, Status: {paymentTransaction.Status}");
				Console.ResetColor();

				// Nếu đã xử lý thành công rồi, trả về OK
				if (paymentTransaction.Status == "Success")
				{
					return new PayOSWebhookResponse(0, "Ok", null);
				}

				// Get actual Order ID
				var actualOrderId = paymentTransaction.ActualOrderId;
				if (actualOrderId == null)
				{
					return new PayOSWebhookResponse(-1, "fail", "Order ID not found");
				}

				var order = await _orderService.GetOrderByIdAsync(actualOrderId.Value);
				if (order == null)
				{
					return new PayOSWebhookResponse(-1, "fail", "Order not found");
				}

				// Bắt đầu transaction để đảm bảo tính nhất quán
				await _unitOfWork.BeginTransactionAsync();
				try
				{
					paymentTransaction.Status = "Success";
					paymentTransaction.TransactionId = webhookData.orderCode.ToString();
					paymentTransaction.ResponseData = System.Text.Json.JsonSerializer.Serialize(webhookData);
					paymentTransaction.UpdatedAt = DateTime.UtcNow;

					// Update Order status
					await _orderService.UpdateOrderStatusAsync(
						actualOrderId.Value,
						new UpdateOrderStatusDTO { Status = OrderStatus.Processing }
					);

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
				catch (Exception)
				{
					await _unitOfWork.RollBackAsync();
					throw;
				}
			}
			catch (Exception ex)
			{
				return new PayOSWebhookResponse(-1, "fail", ex.Message);
			}
		}

		public async Task ProcessPaymentReturnAsync(int orderCode)
		{
			try
			{

				var paymentTransaction = await _unitOfWork.PaymentTransactions.GetSingleAsync(
					new QueryBuilder<PaymentTransaction>()
					.WithPredicate(pt => pt.Id == orderCode && pt.PaymentMethod == "PayOS")
					.WithTracking(true)
					.Build());

				if (paymentTransaction == null)
				{
					throw new Exception($"PaymentTransaction not found for orderCode: {orderCode}");
				}

				if (paymentTransaction.Status == "Success")
				{
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine($"[PayOS Return] Payment already processed");
					Console.ResetColor();
					return;
				}

				var actualOrderId = paymentTransaction.ActualOrderId;
				if (actualOrderId == null)
				{
					throw new Exception("Order ID not found in PaymentTransaction");
				}

				var order = await _orderService.GetOrderByIdAsync(actualOrderId.Value);
				if (order == null)
				{
					throw new Exception($"Order not found: {actualOrderId}");
				}

				var paymentInfo = await GetPaymentLinkInformationAsync(orderCode);
				if (paymentInfo.status != "PAID")
				{
					throw new Exception($"Payment not confirmed by PayOS. Status: {paymentInfo.status}");
				}

				await _unitOfWork.BeginTransactionAsync();
				try
				{
					paymentTransaction.Status = "Success";
					paymentTransaction.TransactionId = orderCode.ToString();
					paymentTransaction.ResponseData = System.Text.Json.JsonSerializer.Serialize(paymentInfo);
					paymentTransaction.UpdatedAt = DateTime.UtcNow;

					await _orderService.UpdateOrderStatusAsync(
						actualOrderId.Value,
						new UpdateOrderStatusDTO { Status = OrderStatus.Processing }
					);

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
	
				}
				catch (Exception)
				{
					await _unitOfWork.RollBackAsync();
					throw;
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}
	}
}
