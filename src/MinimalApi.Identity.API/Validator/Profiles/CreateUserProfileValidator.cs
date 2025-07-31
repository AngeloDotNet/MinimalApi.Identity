using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.Core.Options;

namespace MinimalApi.Identity.API.Validator.Profiles;

public class CreateUserProfileValidator : AbstractValidator<CreateUserProfileModel>
{
    public CreateUserProfileValidator(IOptions<ValidationOptions> options)
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
