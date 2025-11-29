using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class PaymentTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } // "PayOS" hoặc "VnPay"

        [Required]
        [StringLength(100)]
        public string OrderId { get; set; }

        public int? ActualOrderId { get; set; }
        public Order? ActualOrder { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(100)]
        public string? TransactionId { get; set; }

        [Required]
        public PaymentTransactionStatus Status { get; set; } = PaymentTransactionStatus.Pending;

        /// Idempotency key để tránh xử lý duplicate payment
        /// Format: {ActualOrderId}_{TransactionId}
        [StringLength(200)]
        public string? IdempotencyKey { get; set; }

        [StringLength(2048)]
        public string? PaymentUrl { get; set; }

        public string? ResponseData { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Row version for optimistic concurrency control
        /// </summary>
        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
