using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Zenkoi.BLL.Helpers.Validations
{
	public class PasswordValidationAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var password = value as string;

			if (string.IsNullOrEmpty(password))
			{
				return new ValidationResult("Mật khẩu không được để trống.");
			}

			// Kiểm tra độ dài
			if (password.Length < 6)
			{
				return new ValidationResult("Mật khẩu phải có ít nhất 6 ký tự.");
			}

			if (!password.Any(char.IsDigit))
			{
				return new ValidationResult("Mật khẩu phải có ít nhất 1 ký tự số.");
			}

			// Kiểm tra chứa ít nhất một chữ cái viết hoa
			if (!Regex.IsMatch(password, @"[A-Z]"))
			{
				return new ValidationResult("Mật khẩu phải có ít nhất 1 chữ cái viết hoa.");
			}

			// Kiểm tra chứa ít nhất một chữ cái viết thường
			if (!Regex.IsMatch(password, @"[a-z]"))
			{
				return new ValidationResult("Mật khẩu phải có ít nhất một chữ cái viết thường.");
			}

			// Kiểm tra chứa ít nhất một ký tự đặc biệt
			if (!Regex.IsMatch(password, @"[\W_]")) // \W là ký tự đặc biệt và _ là dấu gạch dưới
			{
				return new ValidationResult("Mật khẩu phải chứa ít nhất một kí tự đặc biệt.");
			}

			// Nếu tất cả các điều kiện đều đúng
			return ValidationResult.Success;
		}
	}
}
