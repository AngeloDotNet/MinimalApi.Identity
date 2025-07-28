using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.PolicyManager.Models;

namespace MinimalApi.Identity.PolicyManager.Validator;

public class CreatePolicyValidator : AbstractValidator<CreatePolicyModel>
{
    public CreatePolicyValidator(IOptions<ValidationOptions> options)
    {
        var validationOptions = options.Value;

        RuleFor(x => x.PolicyName)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(validationOptions.MinLength).WithMessage($"Name must be at least {validationOptions.MinLength} characters")
            .MaximumLength(validationOptions.MaxLength).WithMessage($"Name must not exceed {validationOptions.MaxLength} characters");

        RuleFor(x => x.PolicyDescription)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(validationOptions.MinLengthDescription).WithMessage($"Description must be at least {validationOptions.MinLengthDescription} characters")
            .MaximumLength(validationOptions.MaxLengthDescription).WithMessage($"Name must not exceed {validationOptions.MaxLengthDescription} characters");

        RuleFor(x => x.PolicyPermissions)
            .NotEmpty().WithMessage("Permissions are required.")
            .Must(permissions => permissions != null && permissions.Length != 0).WithMessage("Permissions array must contain at least one element.");
    }
}
