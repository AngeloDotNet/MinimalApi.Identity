using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.PolicyManager.Models;

namespace MinimalApi.Identity.PolicyManager.Mapping;

public static class PolicyMapper
{
    public static IQueryable<PolicyResponseModel> ToModel(this IQueryable<AuthPolicy> query)
        => query.Select(p => new PolicyResponseModel(p.Id, p.PolicyName, p.PolicyDescription, p.PolicyPermissions));

    public static IQueryable<PolicyDetailsResponseModel> ToDetailsModel(this IQueryable<AuthPolicy> query)
        => query.Select(p => new PolicyDetailsResponseModel(p.Id, p.PolicyName, p.PolicyDescription, p.PolicyPermissions, p.IsDefault, p.IsActive));

    public static AuthPolicy ToEntityModel(this PolicyDetailsResponseModel model)
    {
        return new AuthPolicy
        {
            PolicyName = model.PolicyName,
            PolicyDescription = model.PolicyDescription,
            PolicyPermissions = model.PolicyPermissions,
            IsDefault = model.IsDefault,
            IsActive = model.IsActive
        };
    }
}