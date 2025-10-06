using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.AccountDTOs
{
	public class AccountCreateRequestDTO
	{
		[Required(ErrorMessage = "Email không được bỏ trống")]
		[EmailAddress(ErrorMessage = "Email sai định dạng. Định dạng đúng: example@gmail.com")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Tên đăng nhập không được để trống")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Mật khẩu không được để trống")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Nhắc lại mật khẩu không được để trống")]
		public string ConfirmPassword { get; set; }

		[Required(ErrorMessage = "Họ và tên không được để trống")]
		public string FullName { get; set; }

		[Required(ErrorMessage = "Số điện thoại không được để trống")]
		[Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
		public string PhoneNumber { get; set; }

		[Required(ErrorMessage = "Role không được để trống")]
		public Role Role { get; set; }
	
	}
}
