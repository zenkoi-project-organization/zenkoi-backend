namespace Zenkoi.BLL.DTOs.PayOSDTOs
{
	public record PayOSWebhookResponse(
		int error,
        string message,
		object? data
	);
}
