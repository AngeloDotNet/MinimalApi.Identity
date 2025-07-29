using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.Core.Options;

namespace MinimalApi.Identity.API.Validator.Modules;

public class CreateModuleValidator : AbstractValidator<CreateModuleModel>
{
    public CreateModuleValidator(IOptions<ValidationOptions> options)
    {
        var validationOptions = options.Value;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(validationOptions.MinLength).WithMessage($"Name must be at least {validationOptions.MinLength} characters")
            .MaximumLength(validationOptions.MaxLength).WithMessage($"Name must not exceed {validationOptions.MaxLength} characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(validationOptions.MinLengthDescription).WithMessage($"Name must be at least {validationOptions.MinLengthDescription} characters")
            .MaximumLength(validationOptions.MaxLengthDescription).WithMessage($"Name must not exceed {validationOptions.MaxLengthDescription} characters");
    }
}
