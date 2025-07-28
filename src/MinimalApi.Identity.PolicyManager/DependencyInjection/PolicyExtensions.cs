using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.PolicyManager.Services.Interfaces;
using MinimalApi.Identity.PolicyManager.Validator;

namespace MinimalApi.Identity.PolicyManager.DependencyInjection;

public static class PolicyExtensions
{
    public const string EndpointsDefaultApi = "/api";
    public const string EndpointsStringEmpty = "";

    public const string EndpointsAuthPolicyGroup = EndpointsDefaultApi + "/policy";
    public const string EndpointsAuthPolicyTag = "Policies";

    public const string EndpointsCreateAuthPolicy = "/create-policy";
    public const string EndpointsDeleteAuthPolicy = "/delete-policy";

    public const string PolicyNotFound = "Policy not found";
    public const string PolicyAlreadyExist = "Policy already exists";
    public const string PolicyCreated = "Policy created successfully";
    public const string PolicyNotDeleted = "Policy not deleted, it is not possible to delete a policy created by default";
    public const string PolicyDeleted = "Policy deleted successfully";

    public static IServiceCollection PolicyManagerRegistrationService(this IServiceCollection services)
    {
        services
            .AddRegisterServices(options =>
            {
                options.Interfaces = [typeof(IAuthPolicyService)];
                options.StringEndsWith = "Service";
                options.Lifetime = ServiceLifetime.Transient;
            })
            .ConfigureFluentValidation<CreatePolicyValidator>();

        return services;
    }
}