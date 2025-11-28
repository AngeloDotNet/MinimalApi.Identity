using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.Core.Settings;

namespace MinimalApi.Identity.API.Validator;

public class RegisterValidator : AbstractValidator<RegisterModel>
{
    private static int requiredUniqueChars;

    public RegisterValidator(IOptions<JwtOptions> iOptions, IOptions<AppSettings> pOptions)
    {
        var identityOptions = iOptions.Value;
        var vOptions = pOptions.Value;
        requiredUniqueChars = identityOptions.RequiredUniqueChars;

        RuleFor(x => x.Firstname)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(vOptions.ValidateMinLength).WithMessage($"First name must be at least {vOptions.ValidateMinLength} characters")
            .MaximumLength(vOptions.ValidateMaxLength).WithMessage($"First name must not exceed {vOptions.ValidateMaxLength} characters");

        RuleFor(x => x.Lastname)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(vOptions.ValidateMinLength).WithMessage($"Last name must be at least {vOptions.ValidateMinLength} characters")
            .MaximumLength(vOptions.ValidateMaxLength).WithMessage($"Last name must not exceed {vOptions.ValidateMaxLength} characters");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(vOptions.ValidateMinLength).WithMessage($"Username must be at least {vOptions.ValidateMinLength} characters")
            .MaximumLength(vOptions.ValidateMaxLength).WithMessage($"Username must not exceed {vOptions.ValidateMaxLength} characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(identityOptions.RequiredLength).WithMessage($"Password must be at least {identityOptions.RequiredLength} characters")
            .Must(ContainAtLeastTwoUppercaseLetters).WithMessage("Password must contain at least one uppercase letter.")
            .Must(ContainAtLeastOneLowercaseLetter).WithMessage("Password must contain at least one lowercase letter.")
            .Must(ContainAtLeastOneNonAlphanumericCharacter).WithMessage("Password must contain at least one non-alphanumeric character.")
            .Must(ContainAtLeastUniqueCharacters).WithMessage($"Password must contain at least {requiredUniqueChars} unique characters.");
    }

    private static bool ContainAtLeastTwoUppercaseLetters(string password) => password.Any(char.IsUpper);

    private static bool ContainAtLeastOneLowercaseLetter(string password) => password.Any(char.IsLower);

    private static bool ContainAtLeastOneNonAlphanumericCharacter(string password) => password.Any(ch => !char.IsLetterOrDigit(ch));

    private static bool ContainAtLeastUniqueCharacters(string password) => password.Distinct().Count() >= requiredUniqueChars;
}