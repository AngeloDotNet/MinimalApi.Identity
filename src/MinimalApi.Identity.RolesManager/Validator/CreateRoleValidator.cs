using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.Core.Settings;
using MinimalApi.Identity.RolesManager.Models;

namespace MinimalApi.Identity.RolesManager.Validator;

public class CreateRoleValidator : AbstractValidator<CreateRoleModel>
{
    public CreateRoleValidator(IOptions<AppSettings> options)
    {
        var vOptions = options.Value;

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .MinimumLength(vOptions.ValidateMinLength).WithMessage($"Role name must be at least {vOptions.ValidateMinLength} characters")
            .MaximumLength(vOptions.ValidateMaxLength).WithMessage($"Role name must not exceed {vOptions.ValidateMaxLength} characters");
    }
}