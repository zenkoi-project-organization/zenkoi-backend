using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zenkoi.DAL.Entities
{
	public class Payment
	{
		[Key]
		public int Id { get; set; }

		public int UserId { get; set; }
		public string PaymentInfo { get; set; }
		public string? BankName { get; set; }
		public bool IsDefault { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }
	}
}
