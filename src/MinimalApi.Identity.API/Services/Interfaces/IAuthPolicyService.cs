﻿using System.Linq.Expressions;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.Core.Entities;

namespace MinimalApi.Identity.API.Services.Interfaces;

public interface IAuthPolicyService
{
    Task<List<PolicyResponseModel>> GetAllPoliciesAsync();
    Task<string> CreatePolicyAsync(CreatePolicyModel model);
    Task<string> DeletePolicyAsync(DeletePolicyModel model);
    Task<bool> GenerateAuthPoliciesAsync();
    Task<bool> UpdateAuthPoliciesAsync();
    Task<List<AuthPolicy>> GetAllAuthPoliciesAsync(Expression<Func<AuthPolicy, bool>> filter = null!);
}