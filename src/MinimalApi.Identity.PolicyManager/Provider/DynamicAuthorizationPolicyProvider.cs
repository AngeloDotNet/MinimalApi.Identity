using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.Core.Authorization;
using MinimalApi.Identity.PolicyManager.Services;

namespace MinimalApi.Identity.PolicyManager.Provider;

public class DynamicAuthorizationPolicyProvider(IServiceProvider serviceProvider, IOptions<AuthorizationOptions> authorizationOptions) : IAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions authorizationOptions = authorizationOptions.Value;

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        using var scope = serviceProvider.CreateScope();
        var policyService = scope.ServiceProvider.GetRequiredService<IAuthPolicyService>();

        // You can pass a real cancellation token if needed
        var policies = await policyService.GetAllPoliciesAsync(CancellationToken.None);
        var policyModel = policies.Where(p => p.PolicyName == policyName).FirstOrDefault();

        if (policyModel != null)
        {
            var permissionRequirement = new PermissionRequirement(policyModel.PolicyPermissions);
            var policy = new AuthorizationPolicyBuilder().AddRequirements(permissionRequirement).Build();

            return policy;
        }

        // Fallback to static policies
        return authorizationOptions.GetPolicy(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => Task.FromResult(authorizationOptions.DefaultPolicy);

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => Task.FromResult(authorizationOptions.FallbackPolicy);
}