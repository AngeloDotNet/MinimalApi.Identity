using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.PolicyManager.Mapping;
using MinimalApi.Identity.PolicyManager.Models;

namespace MinimalApi.Identity.PolicyManager.DependencyInjection;

public static class PolicyQuery
{
    public static async Task<List<PolicyResponseModel>> GetPoliciesAsync(MinimalApiAuthDbContext dbContext,
    Expression<Func<AuthPolicy, bool>> filter = null!, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Set<AuthPolicy>().AsNoTracking();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.ToModel().ToListAsync(cancellationToken);
    }

    public static async Task<PolicyDetailsResponseModel?> GetPolicyAsync(MinimalApiAuthDbContext dbContext,
        Expression<Func<AuthPolicy, bool>> filter = null!, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Set<AuthPolicy>().AsNoTracking();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.ToDetailsModel().FirstOrDefaultAsync(cancellationToken);
    }
}
