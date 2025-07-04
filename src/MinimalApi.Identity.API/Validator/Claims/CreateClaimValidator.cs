﻿using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.Core.Options;

namespace MinimalApi.Identity.API.Validator.Claims;

public class CreateClaimValidator : AbstractValidator<CreateClaimModel>
{
    public CreateClaimValidator(IOptions<ApiValidationOptions> options)
    {
        var validationOptions = options.Value;

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required")
            .MinimumLength(validationOptions.MinLengthClaimValue).WithMessage($"Claim must be at least {validationOptions.MinLengthClaimValue} characters")
            .MaximumLength(validationOptions.MaxLengthClaimValue).WithMessage($"Claim must not exceed {validationOptions.MaxLengthClaimValue} characters");
    }
}