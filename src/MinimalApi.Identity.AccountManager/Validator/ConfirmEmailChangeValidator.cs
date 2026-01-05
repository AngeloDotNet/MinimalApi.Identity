using FluentValidation;
using MinimalApi.Identity.AccountManager.Models;

namespace MinimalApi.Identity.AccountManager.Validator;

public class ConfirmEmailChangeValidator : AbstractValidator<ConfirmEmailChangeModel>
{
    public ConfirmEmailChangeValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");
    }
}
