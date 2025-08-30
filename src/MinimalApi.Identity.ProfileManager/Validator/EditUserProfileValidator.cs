using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.ProfileManager.Models;

namespace MinimalApi.Identity.ProfileManager.Validator;

public class EditUserProfileValidator : AbstractValidator<EditUserProfileModel>
{
    public EditUserProfileValidator(IOptions<ValidationOptions> options)
    {
        var validationOptions = options.Value;

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .GreaterThan(0).WithMessage("UserId must be an integer greater than zero");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(validationOptions.MinLength).WithMessage($"First name must be at least {validationOptions.MinLength} characters")
            .MaximumLength(validationOptions.MaxLength).WithMessage($"First name must not exceed {validationOptions.MaxLength} characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(validationOptions.MinLength).WithMessage($"Last name must be at least {validationOptions.MinLength} characters")
            .MaximumLength(validationOptions.MaxLength).WithMessage($"Last name must not exceed {validationOptions.MaxLength} characters");
    }
}
