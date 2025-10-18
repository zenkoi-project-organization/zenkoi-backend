using Net.payOS;
using Net.payOS.Types;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.BLL.Services.Implements
{
	public class PayOSService : IPayOSService
	{
		private readonly PayOS _payOS;
		public PayOSService(PayOS payOS)
		{
			_payOS = payOS;
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

		public async Task<PaymentLinkInformation> GetPaymentLinkInformation(long orderId)
		{
			PaymentLinkInformation response = await _payOS.getPaymentLinkInformation(orderId);
			if (response == null) throw new Exception($"Không tìm thấy thông tin thanh toán cho đơn hàng: #{orderId}");

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
	}
}
