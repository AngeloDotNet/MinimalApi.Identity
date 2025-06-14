using FluentValidation;
using MinimalApi.Identity.Licenses.Models;

namespace MinimalApi.Identity.Licenses.Validator;

public class AssignLicenseValidator : AbstractValidator<AssignLicenseModel>
{
    public AssignLicenseValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .GreaterThan(0).WithMessage("UserId must be an integer greater than zero");

        RuleFor(x => x.LicenseId)
            .NotEmpty().WithMessage("LicenseId is required")
            .GreaterThan(0).WithMessage("LicenseId must be an integer greater than zero");
    }
}
