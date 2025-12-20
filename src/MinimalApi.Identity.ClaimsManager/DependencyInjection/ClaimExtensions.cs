using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Identity.ClaimsManager.Services;
using MinimalApi.Identity.ClaimsManager.Validator;
using MinimalApi.Identity.Core.DependencyInjection;

namespace MinimalApi.Identity.ClaimsManager.DependencyInjection;

public static class ClaimExtensions
{
    public const string EndpointsCreateClaim = "/create-claim";
    public const string EndpointsAssignClaim = "/assign-claim";
    public const string EndpointsRevokeClaim = "/revoke-claim";
    public const string EndpointsDeleteClaim = "/delete-claim";

    public static IServiceCollection ClaimsManagerRegistrationService(this IServiceCollection services)
    {
        services
            .AddRegisterServices(options =>
            {
                options.Interfaces = [typeof(IClaimsService)];
                options.StringEndsWith = "Service";
                options.Lifetime = ServiceLifetime.Transient;
            })
            .ConfigureFluentValidation<AssignClaimValidator>();

        return services;
    }
}