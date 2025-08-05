using Microsoft.IdentityModel.Tokens;

namespace MinimalApi.Identity.API.Options;

public class JwtOptions
{
    public string SchemaName { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string SecurityKey { get; set; } = null!;
    public TimeSpan ClockSkew { get; set; } = TokenValidationParameters.DefaultClockSkew;

    public int AccessTokenExpirationMinutes { get; set; }
    public int RefreshTokenExpirationMinutes { get; set; }
    public bool RequireUniqueEmail { get; set; }
    public bool RequireDigit { get; set; }
    public int RequiredLength { get; set; }
    public bool RequireUppercase { get; set; }
    public bool RequireLowercase { get; set; }
    public bool RequireNonAlphanumeric { get; set; }
    public int RequiredUniqueChars { get; set; }
    public bool RequireConfirmedEmail { get; set; }
    public int MaxFailedAccessAttempts { get; set; }
    public bool AllowedForNewUsers { get; set; }
    public TimeSpan DefaultLockoutTimeSpan { get; set; }
}