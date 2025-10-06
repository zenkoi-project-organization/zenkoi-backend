using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.AccountDTOs
{
	public class AccountResetpassDTO
	{
		[Required(ErrorMessage = "Email không được để trống.")]
		[EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
		public string Email { get; set; } = null!;
		[Required(ErrorMessage = "Mật khẩu mới không đúng định dạng.")]
		public string NewPassword { get; set; } = null!;

		[Required(ErrorMessage = "Nhắc lại mật khẩu không được để trống.")]
		[Compare("NewPassword", ErrorMessage = "Nhắc lại mật khẩu không khớp với mật khẩu.")]
		public string ConfirmedNewPassword { get; set; } = null!;

		[Required(ErrorMessage = "Token không được để trống.")]
		public string Token { get; set; } = null!;
	}
}
