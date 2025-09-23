using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.Core.Settings;

namespace MinimalApi.Identity.API.Validator.Modules;

public class CreateModuleValidator : AbstractValidator<CreateModuleModel>
{
    public CreateModuleValidator(IOptions<AppSettings> options)
    {
        var vOptions = options.Value;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(vOptions.ValidateMinLength).WithMessage($"Name must be at least {vOptions.ValidateMinLength} characters")
            .MaximumLength(vOptions.ValidateMaxLength).WithMessage($"Name must not exceed {vOptions.ValidateMaxLength} characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(vOptions.ValidateMinLengthDescription).WithMessage($"Name must be at least {vOptions.ValidateMinLengthDescription} characters")
            .MaximumLength(vOptions.ValidateMaxLengthDescription).WithMessage($"Name must not exceed {vOptions.ValidateMaxLengthDescription} characters");
    }
}