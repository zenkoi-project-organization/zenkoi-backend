using System;

namespace Zenkoi.BLL.DTOs.PaymentTransactionDTOs
{
    public class PaymentTransactionResponseDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public int? ActualOrderId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PaymentUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
