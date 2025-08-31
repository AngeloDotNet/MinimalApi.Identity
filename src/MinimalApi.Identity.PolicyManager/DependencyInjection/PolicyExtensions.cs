using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Identity.Core.Authorization;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.PolicyManager.Provider;
using MinimalApi.Identity.PolicyManager.Services;
using MinimalApi.Identity.PolicyManager.Validator;

namespace MinimalApi.Identity.PolicyManager.DependencyInjection;

public static class PolicyExtensions
{
    public const string EndpointsCreateAuthPolicy = "/create-policy";
    public const string EndpointsDeleteAuthPolicy = "/delete-policy";

    public static IServiceCollection PolicyManagerRegistrationService(this IServiceCollection services)
    {
        services
            .AddRegisterServices(options =>
            {
                options.Interfaces = [typeof(IAuthPolicyService)];
                options.StringEndsWith = "Service";
                options.Lifetime = ServiceLifetime.Transient;
            })
            .ConfigureFluentValidation<CreatePolicyValidator>()
        //.AddSingleton<IHostedService, AuthorizationPolicyGeneration>()

        //.AddSingleton<IAuthPolicyStore, AuthPolicyStore>()
        //.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>()

        //.AddHostedService<PolicyUpdateHostedService>()
        //.AddHostedService<AuthorizationPolicyUpdater>()
        ;

        services.AddSingleton<IAuthorizationPolicyProvider, DynamicAuthorizationPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionHandler>();

        return services;
    }
}