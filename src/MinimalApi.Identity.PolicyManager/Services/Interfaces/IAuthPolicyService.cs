using System.Linq.Expressions;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.PolicyManager.Models;

namespace MinimalApi.Identity.PolicyManager.Services.Interfaces;

public interface IAuthPolicyService
{
    Task<List<PolicyResponseModel>> GetAllPoliciesAsync();
    Task<string> CreatePolicyAsync(CreatePolicyModel model);
    Task<string> DeletePolicyAsync(DeletePolicyModel model);
    Task<bool> GenerateAuthPoliciesAsync();
    Task<bool> UpdateAuthPoliciesAsync();
    Task<List<AuthPolicy>> GetAllAuthPoliciesAsync(Expression<Func<AuthPolicy, bool>> filter = null!);
}