using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Utility.Messages;
using MinimalApi.Identity.PolicyManager.Mapping;
using MinimalApi.Identity.PolicyManager.Models;

namespace MinimalApi.Identity.PolicyManager.DependencyInjection;

public static class PolicyQuery
{
    public static async Task<List<PolicyResponseModel>> GetPoliciesAsync(MinimalApiAuthDbContext dbContext,
        Expression<Func<AuthPolicy, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        IQueryable<AuthPolicy> query = dbContext.Set<AuthPolicy>();

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        return await query.ToModel().ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public static async Task<string> CreatePolicyAsync(CreatePolicyModel model, MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await CheckPolicyExistAsync(model.PolicyName, dbContext))
        {
            throw new ConflictException(MessagesApi.PolicyAlreadyExist);
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

        return MessagesApi.PolicyCreated;
    }

    public static async Task<string> DeletePolicyAsync(DeletePolicyModel model, MinimalApiAuthDbContext dbContext, CancellationToken cancellationToken)
    {
        var policyToDelete = await dbContext.Set<AuthPolicy>().Where(x => x.Id == model.Id)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(MessagesApi.PolicyNotFound);

        if (policyToDelete.IsDefault)
        {
            throw new BadRequestException(MessagesApi.PolicyNotDeleted);
        }

        dbContext.Set<AuthPolicy>().Remove(policyToDelete);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MessagesApi.PolicyDeleted;
    }

    private static async Task<bool> CheckPolicyExistAsync(string policyName, MinimalApiAuthDbContext dbContext)
        => await dbContext.Set<AuthPolicy>().AnyAsync(x => x.PolicyName.Equals(policyName, StringComparison.InvariantCultureIgnoreCase));
}