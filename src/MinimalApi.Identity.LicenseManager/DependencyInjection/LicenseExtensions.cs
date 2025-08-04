using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.LicenseManager.Services.Interfaces;
using MinimalApi.Identity.LicenseManager.Validator;

namespace MinimalApi.Identity.LicenseManager.DependencyInjection;

public static class LicenseExtensions
{
    public const string License = nameof(ClaimsType.License);

    public const string EndpointsCreateLicense = "/create-license";
    public const string EndpointsAssignLicense = "/assign-license";
    public const string EndpointsRevokeLicense = "/revoke-license";
    public const string EndpointsDeleteLicense = "/delete-license";

    public static IServiceCollection LicenseRegistrationService(this IServiceCollection services)
    {
        services
            .AddRegisterServices(options =>
            {
                options.Interfaces = [typeof(ILicenseService)];
                options.StringEndsWith = "Service";
                options.Lifetime = ServiceLifetime.Transient;
            })
            .ConfigureFluentValidation<AssignLicenseValidator>();

        return services;
    }
}
