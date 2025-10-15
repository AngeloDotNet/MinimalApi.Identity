using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.RolesManager.Services;
using MinimalApi.Identity.RolesManager.Validator;

namespace MinimalApi.Identity.RolesManager.DependencyInjection;

public static class RolesExtensions
{
    public const string EndpointsCreateRole = "/create-role";
    public const string EndpointsAssignRole = "/assign-role";
    public const string EndpointsRevokeRole = "/revoke-role";
    public const string EndpointsDeleteRole = "/delete-role";

    public static IServiceCollection RolesManagerRegistrationService(this IServiceCollection services)
    {
        services
            .AddRegisterServices(options =>
            {
                options.Interfaces = [typeof(IRoleService)];
                options.StringEndsWith = "Service";
                options.Lifetime = ServiceLifetime.Transient;
            })
            .ConfigureFluentValidation<CreateRoleValidator>();

        return services;
    }
}