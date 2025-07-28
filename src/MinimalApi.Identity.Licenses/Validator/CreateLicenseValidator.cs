using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.Licenses.Models;

namespace MinimalApi.Identity.Licenses.Validator;

public class CreateLicenseValidator : AbstractValidator<CreateLicenseModel>
{
    public CreateLicenseValidator(IOptions<ValidationOptions> options)
    {
        var validationOptions = options.Value;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(validationOptions.MinLength).WithMessage($"Name must be at least {validationOptions.MinLength} characters")
            .MaximumLength(validationOptions.MaxLength).WithMessage($"Name must not exceed {validationOptions.MaxLength} characters");

        RuleFor(x => x.ExpirationDate)
            .NotEmpty().WithMessage("Expiration date is required")
            .Must(x => x > DateOnly.FromDateTime(DateTime.Now)).WithMessage("Expiration date must be greater than today");
    }
}