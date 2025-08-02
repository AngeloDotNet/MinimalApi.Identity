using System.ComponentModel.DataAnnotations;
using MinimalApi.Identity.Core.Validator.Attribute;

namespace MinimalApi.Identity.API.Options;

public class JwtOptions
{
    [Required, MinLength(1, ErrorMessage = "Issuer cannot be empty.")]
    public string Issuer { get; init; } = null!;

    [Required, MinLength(1, ErrorMessage = "Audience cannot be empty.")]
    public string Audience { get; init; } = null!;

    [Required, MinLength(1, ErrorMessage = "SecurityKey cannot be empty.")]
    public string SecurityKey { get; init; } = null!;

    [Required, Range(1, int.MaxValue, ErrorMessage = "AccessTokenExpirationMinutes must be greater than zero.")]
    public int AccessTokenExpirationMinutes { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "RefreshTokenExpirationMinutes must be greater than zero.")]
    public int RefreshTokenExpirationMinutes { get; init; }

    [Required]
    public bool RequireUniqueEmail { get; init; }

    [Required]
    public bool RequireDigit { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "RequiredLength must be greater than zero.")]
    public int RequiredLength { get; init; }

    [Required]
    public bool RequireUppercase { get; init; }

    [Required]
    public bool RequireLowercase { get; init; }

    [Required]
    public bool RequireNonAlphanumeric { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "RequiredUniqueChars must be greater than zero.")]
    public int RequiredUniqueChars { get; init; }

    [Required]
    public bool RequireConfirmedEmail { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MaxFailedAccessAttempts must be greater than zero.")]
    public int MaxFailedAccessAttempts { get; init; }

    [Required]
    public bool AllowedForNewUsers { get; init; }

    [Required, TimeSpanRange("00:00:01", "1.00:00:00", ErrorMessage = "DefaultLockoutTimeSpan must be between 1 second and 1 day.")]
    public TimeSpan DefaultLockoutTimeSpan { get; init; }
}