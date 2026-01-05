using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.Core.Settings;
using MinimalApi.Identity.LicenseManager.Models;

namespace MinimalApi.Identity.LicenseManager.Validator;

public class CreateLicenseValidator : AbstractValidator<CreateLicenseModel>
{
    public CreateLicenseValidator(IOptions<AppSettings> options)
    {
        var vOptions = options.Value;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(vOptions.ValidateMinLength).WithMessage($"Name must be at least {vOptions.ValidateMinLength} characters")
            .MaximumLength(vOptions.ValidateMaxLength).WithMessage($"Name must not exceed {vOptions.ValidateMaxLength} characters");

        RuleFor(x => x.ExpirationDate)
            .NotEmpty().WithMessage("Expiration date is required")
            .Must(x => x > DateOnly.FromDateTime(DateTime.Now)).WithMessage("Expiration date must be greater than today");
    }
}