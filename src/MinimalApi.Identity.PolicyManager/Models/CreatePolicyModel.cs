namespace MinimalApi.Identity.PolicyManager.Models;

public record class CreatePolicyModel(string PolicyName, string PolicyDescription, string[] PolicyPermissions);