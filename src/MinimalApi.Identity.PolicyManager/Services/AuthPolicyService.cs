using System.Linq.Expressions;
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
using MinimalApi.Identity.PolicyManager.DependencyInjection;
using MinimalApi.Identity.PolicyManager.Models;
using MinimalApi.Identity.PolicyManager.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class AuthPolicyService(MinimalApiAuthDbContext dbContext, ILogger<AuthPolicyService> logger,
    IServiceProvider serviceProvider) : IAuthPolicyService
{
    public async Task<List<PolicyResponseModel>> GetAllPoliciesAsync(CancellationToken cancellationToken)
    {
        var query = await PolicyExtensions.GetPolicies(dbContext).ToListAsync(cancellationToken);
        //var query = await dbContext.Set<AuthPolicy>().AsNoTracking().ToListAsync(cancellationToken);

        if (query.Count == 0)
        {
            throw new NotFoundException(PolicyExtensions.PolicyNotFound);
        }

        return query.Select(p => new PolicyResponseModel(p.Id, p.PolicyName, p.PolicyDescription, p.PolicyPermissions)).ToList();
    }

    public async Task<string> CreatePolicyAsync(CreatePolicyModel model, CancellationToken cancellationToken)
    {
        if (await CheckPolicyExistAsync(model))
        {
            throw new ConflictException(PolicyExtensions.PolicyAlreadyExist);
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

        return PolicyExtensions.PolicyCreated;
    }

    public async Task<string> DeletePolicyAsync(DeletePolicyModel model, CancellationToken cancellationToken)
    {
        //var authPolicy = await dbContext.Set<AuthPolicy>().AsNoTracking()
        //.Where(x => x.Id == model.Id && x.PolicyName == model.PolicyName)
        var authPolicy = await PolicyExtensions.GetPolicies(dbContext, x => x.Id == model.Id && x.PolicyName == model.PolicyName)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(PolicyExtensions.PolicyNotFound);

        if (authPolicy.IsDefault)
        {
            throw new BadRequestException(PolicyExtensions.PolicyNotDeleted);
        }

        dbContext.Set<AuthPolicy>().Remove(authPolicy);
        await dbContext.SaveChangesAsync(cancellationToken);

        return PolicyExtensions.PolicyDeleted;
    }

    public async Task<List<AuthPolicy>> GetAllAuthPoliciesAsync(Expression<Func<AuthPolicy, bool>> filter = null!)
    {
        //var query = dbContext.Set<AuthPolicy>().AsNoTracking();
        var query = PolicyExtensions.GetPolicies(dbContext);

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.OrderBy(x => x.Id).ToListAsync();
    }

    public async Task<bool> GenerateAuthPoliciesAsync()
    {
        try
        {
            using var scope = serviceProvider.CreateScope();

            //var authorizationPolicyService = scope.ServiceProvider.GetRequiredService<IAuthPolicyService>();
            //var listPolicy = await authorizationPolicyService.GetAllAuthPoliciesAsync(x => x.IsActive);
            var listPolicy = await GetAllAuthPoliciesAsync(x => x.IsActive);
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

            //var authorizationPolicyService = scope.ServiceProvider.GetRequiredService<IAuthPolicyService>();
            //var listPolicy = await authorizationPolicyService.GetAllAuthPoliciesAsync(x => x.IsActive);
            var listPolicy = await GetAllAuthPoliciesAsync(x => x.IsActive);
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

    private async Task<bool> CheckPolicyExistAsync(CreatePolicyModel model)
    {
        return await dbContext.Set<AuthPolicy>().AsNoTracking()
            .AnyAsync(x => x.PolicyName.Equals(model.PolicyName, StringComparison.InvariantCultureIgnoreCase)
            || x.PolicyPermissions.SequenceEqual(model.PolicyPermissions));
    }
}