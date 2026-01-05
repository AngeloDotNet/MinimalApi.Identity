using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.Core.Settings;
using MinimalApi.Identity.ProfileManager.Models;

namespace MinimalApi.Identity.ProfileManager.Validator;

public class CreateUserProfileValidator : AbstractValidator<CreateUserProfileModel>
{
    public CreateUserProfileValidator(IOptions<AppSettings> options)
    {
        var vOptions = options.Value;

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .GreaterThan(0).WithMessage("UserId must be an integer greater than zero");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(vOptions.ValidateMinLength).WithMessage($"First name must be at least {vOptions.ValidateMinLength} characters")
            .MaximumLength(vOptions.ValidateMaxLength).WithMessage($"First name must not exceed {vOptions.ValidateMaxLength} characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(vOptions.ValidateMinLength).WithMessage($"Last name must be at least {vOptions.ValidateMinLength} characters")
            .MaximumLength(vOptions.ValidateMaxLength).WithMessage($"Last name must not exceed {vOptions.ValidateMaxLength} characters");
    }
}