using MinimalApi.Identity.Core.Database;
using MinimalApi.Identity.PolicyManager.DependencyInjection;
using MinimalApi.Identity.PolicyManager.Models;
using MinimalApi.Identity.PolicyManager.Services;

namespace MinimalApi.Identity.API.Services;

public class AuthPolicyService(MinimalApiAuthDbContext dbContext) : IAuthPolicyService
{
    public async Task<List<PolicyResponseModel>> GetAllPoliciesAsync(CancellationToken cancellationToken)
        => await PolicyQuery.GetPoliciesAsync(dbContext, null, cancellationToken);

    public async Task<string> CreatePolicyAsync(CreatePolicyModel model, CancellationToken cancellationToken)
        => await PolicyQuery.CreatePolicyAsync(model, dbContext, cancellationToken);

    public async Task<string> DeletePolicyAsync(DeletePolicyModel model, CancellationToken cancellationToken)
        => await PolicyQuery.DeletePolicyAsync(model, dbContext, cancellationToken);
}