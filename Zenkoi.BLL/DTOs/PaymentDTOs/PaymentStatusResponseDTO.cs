namespace Zenkoi.BLL.DTOs.PaymentDTOs
{
    public class PaymentStatusResponseDTO
    {
        public bool IsSuccess { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
