using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.VnPayDTOs
{
	public class VnPayRequestDTO
	{
		public long? OrderId { get; set; }
		public string FullName { get; set; }
		public string Description { get; set; }
		[Required(ErrorMessage = "Amount không được để trống.")]
		public double Amount { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}
