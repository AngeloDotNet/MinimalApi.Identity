using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.PolicyManager.BackgroundServices;
using MinimalApi.Identity.PolicyManager.HostedServices;
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
            .AddSingleton<IHostedService, AuthorizationPolicyGeneration>()
            .AddHostedService<AuthorizationPolicyUpdater>();

        return services;
    }
}