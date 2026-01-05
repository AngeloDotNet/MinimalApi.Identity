using FluentValidation;
using MinimalApi.Identity.RolesManager.Models;

namespace MinimalApi.Identity.RolesManager.Validator;

public class DeleteRoleValidator : AbstractValidator<DeleteRoleModel>
{
    public DeleteRoleValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required");
    }
}