using FluentValidation;
using MinimalApi.Identity.AccountManager.Models;

namespace MinimalApi.Identity.AccountManager.Validator;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailModel>
{
    public ConfirmEmailValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required.");
    }
}
