using FluentValidation;
using MinimalApi.Identity.RolesManager.Models;

namespace MinimalApi.Identity.RolesManager.Validator;

public class AssignRoleValidator : AbstractValidator<AssignRoleModel>
{
    public AssignRoleValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required");
    }
}