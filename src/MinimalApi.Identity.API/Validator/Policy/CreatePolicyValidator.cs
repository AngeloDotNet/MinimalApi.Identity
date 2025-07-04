﻿using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.Core.Options;

namespace MinimalApi.Identity.API.Validator.Policy;

public class CreatePolicyValidator : AbstractValidator<CreatePolicyModel>
{
    public CreatePolicyValidator(IOptions<ApiValidationOptions> options)
    {
        var validationOptions = options.Value;

        RuleFor(x => x.PolicyName)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(validationOptions.MinLengthPolicyName).WithMessage($"Name must be at least {validationOptions.MinLengthPolicyName} characters")
            .MaximumLength(validationOptions.MaxLengthPolicyName).WithMessage($"Name must not exceed {validationOptions.MaxLengthPolicyName} characters");

        RuleFor(x => x.PolicyDescription)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(validationOptions.MinLengthPolicyDescription).WithMessage($"Name must be at least {validationOptions.MinLengthPolicyDescription} characters")
            .MaximumLength(validationOptions.MaxLengthPolicyDescription).WithMessage($"Name must not exceed {validationOptions.MaxLengthPolicyDescription} characters");

        RuleFor(x => x.PolicyPermissions)
            .NotEmpty().WithMessage("Permissions are required.")
            .Must(permissions => permissions != null && permissions.Length != 0).WithMessage("Permissions array must contain at least one element.");
    }
}
