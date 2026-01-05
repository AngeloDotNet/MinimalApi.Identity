namespace MinimalApi.Identity.Core.Models;

public record class GenerateCallBackUrlModel(string UserId, string Token, string? NewEmail);