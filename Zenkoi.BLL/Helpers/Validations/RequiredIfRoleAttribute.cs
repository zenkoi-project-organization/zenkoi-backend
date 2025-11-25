using System.ComponentModel.DataAnnotations;
using Zenkoi.BLL.DTOs.AccountDTOs;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.Helpers.Validations
{
	public class RequiredIfRoleAttribute : ValidationAttribute
	{
		private readonly Role _role;
		private readonly string _propertyName;

		public RequiredIfRoleAttribute(Role role, string propertyName)
		{
			_role = role;
			_propertyName = propertyName;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			// AccountCreateRequestDTO no longer has Role property (signup is Customer only)
			// This validation is kept for backward compatibility but does nothing
			return ValidationResult.Success;
		}
	}
}
