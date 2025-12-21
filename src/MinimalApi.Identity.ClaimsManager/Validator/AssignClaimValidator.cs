using FluentValidation;
using MinimalApi.Identity.ClaimsManager.Models;

namespace MinimalApi.Identity.ClaimsManager.Validator;

public class AssignClaimValidator : AbstractValidator<AssignClaimModel>
{
    public AssignClaimValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .GreaterThan(0).WithMessage("UserId must be an integer greater than zero");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required");
    }
}