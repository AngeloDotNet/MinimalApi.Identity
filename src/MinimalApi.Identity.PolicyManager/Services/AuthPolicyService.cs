using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.Core.Authorization;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Utility.Messages;
using MinimalApi.Identity.PolicyManager.DependencyInjection;
using MinimalApi.Identity.PolicyManager.Mapping;
using MinimalApi.Identity.PolicyManager.Models;
using MinimalApi.Identity.PolicyManager.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class AuthPolicyService(MinimalApiAuthDbContext dbContext, ILogger<AuthPolicyService> logger,
    IServiceProvider serviceProvider) : IAuthPolicyService
{
    public async Task<List<PolicyResponseModel>> GetAllPoliciesAsync(CancellationToken cancellationToken)
    {
        var query = await PolicyQuery.GetPoliciesAsync(dbContext, null!, cancellationToken);

        if (query.Count == 0)
        {
            throw new NotFoundException(MessagesAPI.PolicyNotFound);
        }

        return query.ToList();
    }

    public async Task<string> CreatePolicyAsync(CreatePolicyModel model, CancellationToken cancellationToken)
    {
        if (await CheckPolicyExistAsync(model.PolicyName))
        {
            throw new ConflictException(MessagesAPI.PolicyAlreadyExist);
        }

        var authPolicy = new AuthPolicy
        {
            PolicyName = model.PolicyName,
            PolicyDescription = model.PolicyDescription,
            PolicyPermissions = model.PolicyPermissions,
            IsDefault = false,
            IsActive = true
        };

        dbContext.Set<AuthPolicy>().Add(authPolicy);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MessagesAPI.PolicyCreated;
    }

    public async Task<string> DeletePolicyAsync(DeletePolicyModel model, CancellationToken cancellationToken)
    {
        var authPolicy = await PolicyQuery.GetPolicyAsync(dbContext, x => x.Id == model.Id && x.PolicyName == model.PolicyName, cancellationToken)
            ?? throw new NotFoundException(MessagesAPI.PolicyNotFound);

        if (authPolicy.IsDefault)
        {
            throw new BadRequestException(MessagesAPI.PolicyNotDeleted);
        }

        var entityPolicy = authPolicy.ToEntityModel();

        dbContext.Set<AuthPolicy>().Remove(entityPolicy);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MessagesAPI.PolicyDeleted;
    }

    public async Task<bool> GenerateAuthPoliciesAsync()
    {
        try
        {
            using var scope = serviceProvider.CreateScope();

            var listPolicy = await PolicyQuery.GetPoliciesAsync(dbContext, p => p.IsActive);
            var authorizationOptions = serviceProvider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

            if (listPolicy.Count == 0)
            {
                logger.LogWarning(ConstantsConfiguration.NoActivePoliciesFound);
                throw new NotFoundException();
            }

            logger.LogInformation("Found {PolicyCount} active policies in the database.", listPolicy.Count);

            foreach (var policy in listPolicy)
            {
                if (authorizationOptions.GetPolicy(policy.PolicyName) != null)
                {
                    logger.LogInformation("Policy {PolicyName} already exists.", policy.PolicyName);
                    continue;
                }

                var permissionRequirement = new PermissionRequirement(policy.PolicyPermissions);
                logger.LogInformation("Adding policy: {PolicyName} with permissions: {PolicyPermissions}", policy.PolicyName, string.Join(", ", policy.PolicyPermissions));
                authorizationOptions.AddPolicy(policy.PolicyName, policy => policy.Requirements.Add(permissionRequirement));
            }

            return true;
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(ex, ConstantsConfiguration.NoActivePoliciesFound);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while generating authorization policies.");
            return false;
        }
    }

    public async Task<bool> UpdateAuthPoliciesAsync()
    {
        try
        {
            using var scope = serviceProvider.CreateScope();

            var listPolicy = await PolicyQuery.GetPoliciesAsync(dbContext, p => p.IsActive);
            var authorizationOptions = serviceProvider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

            if (listPolicy.Count == 0)
            {
                logger.LogWarning(ConstantsConfiguration.NoActivePoliciesFound);
                throw new NotFoundException();
            }

            logger.LogInformation("Found {PolicyCount} active policies in the database.", listPolicy.Count);
            foreach (var policy in listPolicy)
            {
                if (authorizationOptions.GetPolicy(policy.PolicyName) != null)
                {
                    logger.LogInformation("Policy {PolicyName} already exists.", policy.PolicyName);
                    continue;
                }

                var permissionRequirement = new PermissionRequirement(policy.PolicyPermissions);
                logger.LogInformation("Adding policy: {PolicyName} with permissions: {PolicyPermissions}", policy.PolicyName, string.Join(", ", policy.PolicyPermissions));
                authorizationOptions.AddPolicy(policy.PolicyName, policy => policy.Requirements.Add(permissionRequirement));
            }

            return true;
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(ex, ConstantsConfiguration.NoActivePoliciesFound);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while generating authorization policies.");
            return false;
        }
    }

    private async Task<bool> CheckPolicyExistAsync(string policyName)
        => await dbContext.Set<AuthPolicy>().AsNoTracking()
                    .AnyAsync(x => x.PolicyName.Equals(policyName, StringComparison.InvariantCultureIgnoreCase));
}