namespace MinimalApi.Identity.ClaimsManager.Models;

public record class AssignClaimModel(int UserId, string Type, string Value);
