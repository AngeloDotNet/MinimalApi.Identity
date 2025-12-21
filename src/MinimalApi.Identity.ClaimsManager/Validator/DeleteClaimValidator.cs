using FluentValidation;
using MinimalApi.Identity.ClaimsManager.Models;

namespace MinimalApi.Identity.ClaimsManager.Validator;

public class DeleteClaimValidator : AbstractValidator<DeleteClaimModel>
{
    public DeleteClaimValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required");
    }
}