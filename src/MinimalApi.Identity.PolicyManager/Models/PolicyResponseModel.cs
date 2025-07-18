namespace MinimalApi.Identity.PolicyManager.Models;

public record class PolicyResponseModel(int Id, string PolicyName, string PolicyDescription, string[] PolicyPermissions);