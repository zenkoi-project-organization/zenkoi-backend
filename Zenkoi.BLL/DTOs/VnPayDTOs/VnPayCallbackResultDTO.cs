namespace Zenkoi.BLL.DTOs.VnPayDTOs
{
	public class VnPayCallbackResultDTO
	{
		public bool IsSuccess { get; set; }
		public int? OrderId { get; set; }
		public decimal Amount { get; set; }
		public string ErrorCode { get; set; } = string.Empty;
		public string ErrorMessage { get; set; } = string.Empty;
	}
}
