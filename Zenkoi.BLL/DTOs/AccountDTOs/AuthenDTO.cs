using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.AccountDTOs
{
	public class AuthenDTO
	{
		[Required(ErrorMessage = "Tên đăng nhập không được để trống.")]
		[Display(Name = "Tên đăng nhập hoặc Email")]
		public string UserNameOrEmail { get; set; } = null!;

		[Required(ErrorMessage = "Mật khẩu không được để trống")]
		[DataType(DataType.Password)]
		public string Password { get; set; } = null!;
	}
}
