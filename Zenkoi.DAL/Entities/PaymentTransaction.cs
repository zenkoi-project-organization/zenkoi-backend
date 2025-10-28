using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [StringLength(20)]
        public string Status { get; set; } = "Pending"; 

        [StringLength(2048)]
        public string? PaymentUrl { get; set; }

        public string? ResponseData { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
