using MinimalApi.Identity.PolicyManager.Models;

namespace MinimalApi.Identity.PolicyManager.Services;

public interface IAuthPolicyService
{
    Task<List<PolicyResponseModel>> GetAllPoliciesAsync(CancellationToken cancellationToken);
    Task<string> CreatePolicyAsync(CreatePolicyModel model, CancellationToken cancellationToken);
    Task<string> DeletePolicyAsync(DeletePolicyModel model, CancellationToken cancellationToken);
    Task<bool> GenerateAuthPoliciesAsync();
    Task<bool> UpdateAuthPoliciesAsync();
}