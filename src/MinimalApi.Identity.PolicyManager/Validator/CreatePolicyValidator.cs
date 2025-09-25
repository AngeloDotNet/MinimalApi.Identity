using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.Core.Settings;
using MinimalApi.Identity.PolicyManager.Models;

namespace MinimalApi.Identity.PolicyManager.Validator;

public class CreatePolicyValidator : AbstractValidator<CreatePolicyModel>
{
    public CreatePolicyValidator(IOptions<AppSettings> options)
    {
        var vOptions = options.Value;

        RuleFor(x => x.PolicyName)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(vOptions.ValidateMinLength).WithMessage($"Name must be at least {vOptions.ValidateMinLength} characters")
            .MaximumLength(vOptions.ValidateMaxLength).WithMessage($"Name must not exceed {vOptions.ValidateMaxLength} characters");

        RuleFor(x => x.PolicyDescription)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(vOptions.ValidateMinLengthDescription).WithMessage($"Description must be at least {vOptions.ValidateMinLengthDescription} characters")
            .MaximumLength(vOptions.ValidateMaxLengthDescription).WithMessage($"Description must not exceed {vOptions.ValidateMaxLengthDescription} characters");

        RuleFor(x => x.PolicyPermissions)
            .NotEmpty().WithMessage("Permissions are required.")
            .Must(permissions => permissions != null && permissions.Length != 0).WithMessage("Permissions array must contain at least one element.");
    }
}
