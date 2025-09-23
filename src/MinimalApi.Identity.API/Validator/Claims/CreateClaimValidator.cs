using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.Core.Settings;

namespace MinimalApi.Identity.API.Validator.Claims;

public class CreateClaimValidator : AbstractValidator<CreateClaimModel>
{
    public CreateClaimValidator(IOptions<AppSettings> options)
    {
        var vOptions = options.Value;

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required")
            .MinimumLength(vOptions.ValidateMinLength).WithMessage($"Claim must be at least {vOptions.ValidateMinLength} characters")
            .MaximumLength(vOptions.ValidateMaxLength).WithMessage($"Claim must not exceed {vOptions.ValidateMaxLength} characters");
    }
}