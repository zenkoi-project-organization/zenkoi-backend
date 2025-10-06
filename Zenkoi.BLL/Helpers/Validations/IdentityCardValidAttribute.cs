using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Zenkoi.BLL.Helpers.Validations
{
	public class IdentityCardValidAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var identityCard = value as string;

			string pattern = @"^0(0[1-9]|[1-8][0-9]|9[0-6])[0-3]([0-9][0-9])[0-9]{6}$";
			if (!Regex.IsMatch(identityCard, pattern))
			{
				return new ValidationResult("Giấy tờ tùy thân không hợp lệ.");
			}
			return ValidationResult.Success;
		}
	}
}
