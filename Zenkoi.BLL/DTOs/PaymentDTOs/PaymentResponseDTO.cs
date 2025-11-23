namespace Zenkoi.BLL.DTOs.PaymentDTOs
{
    public class PaymentResponseDTO
    {
        public string PaymentUrl { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public bool IsRetry { get; set; }
    }
}
