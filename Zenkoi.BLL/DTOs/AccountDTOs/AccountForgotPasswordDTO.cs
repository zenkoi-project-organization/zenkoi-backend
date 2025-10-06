using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.AccountDTOs
{
	public class AccountForgotPasswordDTO
	{
		[Required(ErrorMessage = "Email không được để trống.")]
		[EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
		public string Email { get; set; } = null!;
	}
}
