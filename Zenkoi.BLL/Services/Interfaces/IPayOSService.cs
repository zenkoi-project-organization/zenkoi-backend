using Net.payOS.Types;
using Zenkoi.BLL.DTOs.PayOSDTOs;

namespace Zenkoi.BLL.Services.Interfaces
{
	public interface IPayOSService
	{
		Task<CreatePaymentResult> CreatePaymentLinkAsync(PaymentData data);
		Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(long orderId);
		Task ConfirmWebhookAsync(string webhookUrl);
		Task<PaymentLinkInformation> CancelPaymentLinkAsync(long orderId);
		WebhookData VerifyPaymentWebhookData(WebhookType data);
		Task<PayOSWebhookResponse> ProcessWebhookAsync(WebhookType payload);
		Task ProcessPaymentReturnAsync(int orderCode);
	}
}
