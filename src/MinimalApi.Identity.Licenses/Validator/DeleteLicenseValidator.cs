using FluentValidation;
using MinimalApi.Identity.Licenses.Models;

namespace MinimalApi.Identity.Licenses.Validator;

public class DeleteLicenseValidator : AbstractValidator<DeleteLicenseModel>
{
    public DeleteLicenseValidator()
    {
        RuleFor(x => x.LicenseId)
            .NotEmpty().WithMessage("LicenseId is required")
            .GreaterThan(0).WithMessage("LicenseId must be an integer greater than zero");
    }
}
