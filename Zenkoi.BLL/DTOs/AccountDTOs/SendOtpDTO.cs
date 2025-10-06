using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.AccountDTOs
{
	public class SendOtpDTO
	{
		[Required(ErrorMessage = "Email không được để trống")]
		public string Email { get; set; } = null!;
	}
}
