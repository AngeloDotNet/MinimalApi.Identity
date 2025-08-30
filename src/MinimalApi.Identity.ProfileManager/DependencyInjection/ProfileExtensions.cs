using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.ProfileManager.Services;
using MinimalApi.Identity.ProfileManager.Validator;

namespace MinimalApi.Identity.ProfileManager.DependencyInjection;

public static class ProfileExtensions
{
    public const string FullName = nameof(ClaimsType.FullName);

    public const string EndpointsGetProfile = "/{userId}";
    public const string EndpointsEditProfile = "/edit-profile";
    public const string EndpointsChangeEnableProfile = "/change-enable-profile";

    public static IServiceCollection ProfileManagerRegistrationService(this IServiceCollection services)
    {
        services
            .AddRegisterServices(options =>
            {
                options.Interfaces = [typeof(IProfileService)];
                options.StringEndsWith = "Service";
                options.Lifetime = ServiceLifetime.Transient;
            })
            .ConfigureFluentValidation<CreateUserProfileValidator>();

        return services;
    }
}
