using Net.payOS.Types;

namespace Zenkoi.BLL.Services.Interfaces
{
	public interface IPayOSService
	{
		Task<CreatePaymentResult> CreatePaymentLinkAsync(PaymentData data);
		Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(long orderId);
		Task ConfirmWebhookAsync(string webhookUrl);
		Task<PaymentLinkInformation> CancelPaymentLinkAsync(long orderId);
		WebhookData VerifyPaymentWebhookData(WebhookType data);
	}
}
