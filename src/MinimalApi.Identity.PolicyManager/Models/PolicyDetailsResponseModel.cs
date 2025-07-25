namespace MinimalApi.Identity.PolicyManager.Models;

public record class PolicyDetailsResponseModel(int Id, string PolicyName, string PolicyDescription, string[] PolicyPermissions, bool IsDefault, bool IsActive);