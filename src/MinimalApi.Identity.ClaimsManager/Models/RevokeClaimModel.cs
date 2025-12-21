namespace MinimalApi.Identity.ClaimsManager.Models;

public record class RevokeClaimModel(int UserId, string Type, string Value);
