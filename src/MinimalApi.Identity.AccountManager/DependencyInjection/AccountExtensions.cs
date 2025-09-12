using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Identity.AccountManager.Services;
using MinimalApi.Identity.AccountManager.Validator;
using MinimalApi.Identity.Core.DependencyInjection;

namespace MinimalApi.Identity.AccountManager.DependencyInjection;

public static class AccountExtensions
{
    public const string EndpointChangeEmail = "/change-email";

    public static IServiceCollection AccountManagerRegistrationService(this IServiceCollection services)
    {
        services
            .AddRegisterServices(options =>
            {
                options.Interfaces = [typeof(IAccountService)];
                options.StringEndsWith = "Service";
                options.Lifetime = ServiceLifetime.Transient;
            })
            .ConfigureFluentValidation<ChangeEmailValidator>();

        return services;
    }
}
