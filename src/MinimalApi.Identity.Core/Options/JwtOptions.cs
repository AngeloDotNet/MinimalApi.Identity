using Microsoft.IdentityModel.Tokens;

namespace MinimalApi.Identity.Core.Options;

/// <summary>
/// Represents configuration options for JSON Web Token (JWT) authentication and identity management.
/// </summary>
/// <remarks>
/// This class is used to configure JWT token generation and validation, as well as user password and lockout policies.
/// For more information, see:
/// <see href="https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn"/>
/// </remarks>
public class JwtOptions
{
    /// <summary>
    /// Gets or sets the expected issuer of the JWT.
    /// </summary>
    /// <remarks>
    /// Used to validate the <c>iss</c> claim in the token.
    /// </remarks>
    public string Issuer { get; set; } = null!;

    /// <summary>
    /// Gets or sets the expected audience for the JWT.
    /// </summary>
    /// <remarks>
    /// Used to validate the <c>aud</c> claim in the token.
    /// </remarks>
    public string Audience { get; set; } = null!;

    /// <summary>
    /// Gets or sets the symmetric security key used to sign and validate JWTs.
    /// </summary>
    /// <remarks>
    /// This should be a strong, secret value.
    /// </remarks>
    public string SecurityKey { get; set; } = null!;

    /// <summary>
    /// Gets or sets the allowed clock skew when validating token expiration and not-before times.
    /// </summary>
    /// <remarks>
    /// Default is <see cref="TokenValidationParameters.DefaultClockSkew"/>.
    /// </remarks>
    public TimeSpan ClockSkew { get; set; } = TokenValidationParameters.DefaultClockSkew;

    /// <summary>
    /// Gets or sets the expiration time in minutes for access tokens.
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; }

    /// <summary>
    /// Gets or sets the expiration time in minutes for refresh tokens.
    /// </summary>
    public int RefreshTokenExpirationMinutes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether each user must have a unique email address.
    /// </summary>
    public bool RequireUniqueEmail { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether passwords must contain at least one digit.
    /// </summary>
    public bool RequireDigit { get; set; }

    /// <summary>
    /// Gets or sets the minimum required length for user passwords.
    /// </summary>
    public int RequiredLength { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether passwords must contain at least one uppercase letter.
    /// </summary>
    public bool RequireUppercase { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether passwords must contain at least one lowercase letter.
    /// </summary>
    public bool RequireLowercase { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether passwords must contain at least one non-alphanumeric character.
    /// </summary>
    public bool RequireNonAlphanumeric { get; set; }

    /// <summary>
    /// Gets or sets the minimum number of unique characters required in a password.
    /// </summary>
    public int RequiredUniqueChars { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether users must confirm their email address before being allowed to sign in.
    /// </summary>
    public bool RequireConfirmedEmail { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of failed access attempts before a user is locked out.
    /// </summary>
    public int MaxFailedAccessAttempts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether lockout is enabled for new users.
    /// </summary>
    public bool AllowedForNewUsers { get; set; }

    /// <summary>
    /// Gets or sets the default lockout time span for a user after reaching the maximum failed access attempts.
    /// </summary>
    public TimeSpan DefaultLockoutTimeSpan { get; set; }
}