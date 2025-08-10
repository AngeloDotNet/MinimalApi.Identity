using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Identity.API.Options;

namespace MinimalApi.Identity.API.Extensions;

public static class ManageOptionsExtensions
{
    private static FeatureFlagsOptions? featureFlagsOptions;
    private static JwtOptions? jwtOptions;

    public static FeatureFlagsOptions GetFeatureFlagsOptions(IConfiguration configuration)
    {
        var configOptions = configuration.GetSection(nameof(FeatureFlagsOptions)).Get<FeatureFlagsOptions>();

        if (featureFlagsOptions is null)
        {
            featureFlagsOptions = configOptions ?? new FeatureFlagsOptions
            {
                EnabledFeatureLicense = false,
                EnabledFeatureModule = false
            };
        }
        else if (configOptions is not null)
        {
            featureFlagsOptions.EnabledFeatureLicense = configOptions.EnabledFeatureLicense;
            featureFlagsOptions.EnabledFeatureModule = configOptions.EnabledFeatureModule;
        }

        return featureFlagsOptions;
    }

    public static JwtOptions GetJwtOptions(IConfiguration configuration)
    {
        var configOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();

        if (jwtOptions is null)
        {
            jwtOptions = configOptions ?? new JwtOptions
            {
                SchemaName = "Bearer",
                Issuer = string.Empty,
                Audience = string.Empty,
                SecurityKey = string.Empty,
                ClockSkew = TokenValidationParameters.DefaultClockSkew,

                AccessTokenExpirationMinutes = 60,
                RefreshTokenExpirationMinutes = 60,
                RequireUniqueEmail = true,
                RequireDigit = true,
                RequiredLength = 8,
                RequireUppercase = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequiredUniqueChars = 4,
                RequireConfirmedEmail = true,
                MaxFailedAccessAttempts = 3,
                AllowedForNewUsers = true,
                DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5)
            };
        }
        else if (configOptions is not null)
        {
            jwtOptions.SchemaName = configOptions.SchemaName;
            jwtOptions.Issuer = configOptions.Issuer;
            jwtOptions.Audience = configOptions.Audience;
            jwtOptions.SecurityKey = configOptions.SecurityKey;
            jwtOptions.ClockSkew = configOptions.ClockSkew;

            jwtOptions.AccessTokenExpirationMinutes = configOptions.AccessTokenExpirationMinutes;
            jwtOptions.RefreshTokenExpirationMinutes = configOptions.RefreshTokenExpirationMinutes;
            jwtOptions.RequireUniqueEmail = configOptions.RequireUniqueEmail;
            jwtOptions.RequireDigit = configOptions.RequireDigit;
            jwtOptions.RequiredLength = configOptions.RequiredLength;
            jwtOptions.RequireUppercase = configOptions.RequireUppercase;
            jwtOptions.RequireLowercase = configOptions.RequireLowercase;
            jwtOptions.RequireNonAlphanumeric = configOptions.RequireNonAlphanumeric;
            jwtOptions.RequiredUniqueChars = configOptions.RequiredUniqueChars;
            jwtOptions.RequireConfirmedEmail = configOptions.RequireConfirmedEmail;
            jwtOptions.MaxFailedAccessAttempts = configOptions.MaxFailedAccessAttempts;
            jwtOptions.AllowedForNewUsers = configOptions.AllowedForNewUsers;
            jwtOptions.DefaultLockoutTimeSpan = configOptions.DefaultLockoutTimeSpan;
        }

        return jwtOptions;
    }
}