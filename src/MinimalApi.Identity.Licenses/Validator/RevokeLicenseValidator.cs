using FluentValidation;
using MinimalApi.Identity.Licenses.Models;

namespace MinimalApi.Identity.Licenses.Validator;

public class RevokeLicenseValidator : AbstractValidator<RevokeLicenseModel>
{
    public RevokeLicenseValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .GreaterThan(0).WithMessage("The value must be an integer greater than zero");

        RuleFor(x => x.LicenseId)
            .NotEmpty().WithMessage("LicenseId is required")
            .GreaterThan(0).WithMessage("The value must be an integer greater than zero");
    }
}