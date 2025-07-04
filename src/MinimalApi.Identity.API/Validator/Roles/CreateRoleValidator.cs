﻿using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.Core.Options;

namespace MinimalApi.Identity.API.Validator.Roles;

public class CreateRoleValidator : AbstractValidator<CreateRoleModel>
{
    public CreateRoleValidator(IOptions<ApiValidationOptions> options)
    {
        var applicationOptions = options.Value;

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .MinimumLength(applicationOptions.MinLengthRoleName).WithMessage($"Role name must be at least {applicationOptions.MinLengthRoleName} characters")
            .MaximumLength(applicationOptions.MaxLengthRoleName).WithMessage($"Role name must not exceed {applicationOptions.MaxLengthRoleName} characters");
    }
}
