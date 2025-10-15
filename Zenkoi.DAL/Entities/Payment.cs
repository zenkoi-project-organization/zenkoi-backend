using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
	public class Payment
	{
		[Key]
		public int Id { get; set; }
		public int OrderId { get; set; }
		public Order Order { get; set; }

		public PaymentMethod Method { get; set; }
		public decimal Amount { get; set; }
		public DateTime PaidAt { get; set; } = DateTime.UtcNow;
		public string? TransactionId { get; set; }
		public string? Gateway { get; set; } // VNPAY, PayOS, PayPal ...

		// Keep existing fields for backward compatibility
		public int? UserId { get; set; }
		public string? PaymentInfo { get; set; }
		public string? BankName { get; set; }
		public bool IsDefault { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser? User { get; set; }
	}
}
