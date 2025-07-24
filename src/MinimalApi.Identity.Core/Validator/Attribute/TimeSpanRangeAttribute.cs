using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MinimalApi.Identity.Core.Validator.Attribute;

public class TimeSpanRangeAttribute(string min, string max) : ValidationAttribute
{
    private readonly TimeSpan min = TimeSpan.Parse(min, CultureInfo.InvariantCulture);
    private readonly TimeSpan max = TimeSpan.Parse(max, CultureInfo.InvariantCulture);

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is TimeSpan timeSpanValue && (timeSpanValue < min || timeSpanValue > max))
        {
            return new ValidationResult(ErrorMessage ?? $"The field {validationContext.DisplayName} must be between {min} and {max}.");
        }

        return ValidationResult.Success!;
    }
}