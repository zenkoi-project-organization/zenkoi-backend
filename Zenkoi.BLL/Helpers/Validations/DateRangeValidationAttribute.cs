using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.Helpers.Validations;

public class DateRangeValidationAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    public DateRangeValidationAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var currentValue = value as DateOnly?;

        var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
        if (property == null)
        {
            return new ValidationResult($"Unknown property: {_comparisonProperty}");
        }

        var comparisonValue = property.GetValue(validationContext.ObjectInstance) as DateOnly?;

        if (currentValue.HasValue && comparisonValue.HasValue)
        {
            if (currentValue.Value > comparisonValue.Value)
            {
                return new ValidationResult($"ScheduledDateFrom must be less than or equal to ScheduledDateTo");
            }
        }

        return ValidationResult.Success;
    }
}
