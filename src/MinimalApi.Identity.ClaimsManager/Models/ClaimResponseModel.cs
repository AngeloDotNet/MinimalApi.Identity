namespace MinimalApi.Identity.ClaimsManager.Models;

public record class ClaimResponseModel(int Id, string Type, string Value, bool Default);