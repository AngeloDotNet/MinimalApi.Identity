using FluentValidation;
using MinimalApi.Identity.PolicyManager.Models;

namespace MinimalApi.Identity.PolicyManager.Validator;

public class DeletePolicyValidator : AbstractValidator<DeletePolicyModel>
{
    public DeletePolicyValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required")
            .GreaterThan(0).WithMessage("Id must be an integer greater than zero");

        RuleFor(x => x.PolicyName)
            .NotEmpty().WithMessage("Name is required.");
    }
}