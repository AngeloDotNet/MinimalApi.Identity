using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.ModuleManager.Services;
using MinimalApi.Identity.ModuleManager.Validator;

namespace MinimalApi.Identity.ModuleManager.DependencyInjection;

public static class ModuliExtensions
{
    public const string Module = nameof(ClaimsType.Module);

    public const string EndpointsCreateModule = "/create-module";
    public const string EndpointsAssignModule = "/assign-module";
    public const string EndpointsRevokeModule = "/revoke-module";
    public const string EndpointsDeleteModule = "/delete-module";

    public static IServiceCollection ModuleManagerRegistrationService(this IServiceCollection services)
    {
        services
            .AddRegisterServices(options =>
            {
                options.Interfaces = [typeof(IModuleService)];
                options.StringEndsWith = "Service";
                options.Lifetime = ServiceLifetime.Transient;
            })
            .ConfigureFluentValidation<AssignModuleValidator>();

        return services;
    }
}
