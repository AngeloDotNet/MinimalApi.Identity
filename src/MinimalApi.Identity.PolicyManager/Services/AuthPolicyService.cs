using Microsoft.Extensions.Logging;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.PolicyManager.DependencyInjection;
using MinimalApi.Identity.PolicyManager.Models;
using MinimalApi.Identity.PolicyManager.Services;

namespace MinimalApi.Identity.API.Services;

public class AuthPolicyService(MinimalApiAuthDbContext dbContext, ILogger<AuthPolicyService> logger, IServiceProvider serviceProvider) : IAuthPolicyService
{
    public async Task<List<PolicyResponseModel>> GetAllPoliciesAsync(CancellationToken cancellationToken)
        => await PolicyQuery.GetPoliciesAsync(dbContext, null, cancellationToken);

    public async Task<string> CreatePolicyAsync(CreatePolicyModel model, CancellationToken cancellationToken)
        => await PolicyQuery.CreatePolicyAsync(model, dbContext, cancellationToken);

    public async Task<string> DeletePolicyAsync(DeletePolicyModel model, CancellationToken cancellationToken)
        => await PolicyQuery.DeletePolicyAsync(model, dbContext, cancellationToken);

    //public async Task<bool> GenerateAuthPoliciesAsync()
    //{
    //    try
    //    {
    //        using var scope = serviceProvider.CreateScope();

    //        var listPolicy = await PolicyQuery.GetPoliciesAsync(dbContext, p => p.IsActive);
    //        var authorizationOptions = serviceProvider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

    //        if (listPolicy.Count == 0)
    //        {
    //            logger.LogWarning(ConstantsConfiguration.NoActivePoliciesFound);
    //            throw new NotFoundException();
    //        }

    //        logger.LogInformation("Found {PolicyCount} active policies in the database.", listPolicy.Count);

    //        foreach (var policy in listPolicy)
    //        {
    //            if (authorizationOptions.GetPolicy(policy.PolicyName) != null)
    //            {
    //                logger.LogInformation("Policy {PolicyName} already exists.", policy.PolicyName);
    //                continue;
    //            }

    //            var permissionRequirement = new PermissionRequirement(policy.PolicyPermissions);
    //            logger.LogInformation("Adding policy: {PolicyName} with permissions: {PolicyPermissions}", policy.PolicyName, string.Join(", ", policy.PolicyPermissions));
    //            authorizationOptions.AddPolicy(policy.PolicyName, policy => policy.Requirements.Add(permissionRequirement));
    //        }

    //        return true;
    //    }
    //    catch (NotFoundException ex)
    //    {
    //        logger.LogWarning(ex, ConstantsConfiguration.NoActivePoliciesFound);
    //        return false;
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, "An error occurred while generating authorization policies.");
    //        return false;
    //    }
    //}

    //public async Task<bool> UpdateAuthPoliciesAsync()
    //{
    //    try
    //    {
    //        using var scope = serviceProvider.CreateScope();

    //        var listPolicy = await PolicyQuery.GetPoliciesAsync(dbContext, p => p.IsActive);
    //        var authorizationOptions = serviceProvider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

    //        if (listPolicy.Count == 0)
    //        {
    //            logger.LogWarning(ConstantsConfiguration.NoActivePoliciesFound);
    //            throw new NotFoundException();
    //        }

    //        logger.LogInformation("Found {PolicyCount} active policies in the database.", listPolicy.Count);
    //        foreach (var policy in listPolicy)
    //        {
    //            if (authorizationOptions.GetPolicy(policy.PolicyName) != null)
    //            {
    //                logger.LogInformation("Policy {PolicyName} already exists.", policy.PolicyName);
    //                continue;
    //            }

    //            var permissionRequirement = new PermissionRequirement(policy.PolicyPermissions);
    //            logger.LogInformation("Adding policy: {PolicyName} with permissions: {PolicyPermissions}", policy.PolicyName, string.Join(", ", policy.PolicyPermissions));
    //            authorizationOptions.AddPolicy(policy.PolicyName, policy => policy.Requirements.Add(permissionRequirement));
    //        }

    //        return true;
    //    }
    //    catch (NotFoundException ex)
    //    {
    //        logger.LogWarning(ex, ConstantsConfiguration.NoActivePoliciesFound);
    //        return false;
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, "An error occurred while generating authorization policies.");
    //        return false;
    //    }
    //}
}