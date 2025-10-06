using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.AccountDTOs
{
	public class Authen2FaDTO
	{
		[Required(ErrorMessage = "Email không được để trống.")]
		[EmailAddress(ErrorMessage = "Email không đúng định dạng. Định dạng đúng: example@gmail.com")]
		public string? Email { get; set; }
		[Required(ErrorMessage = "Mã OTP không được để trống.")]
		[Display(Name = "Mã OTP")]
		public string? Code { get; set; }
	}
}
