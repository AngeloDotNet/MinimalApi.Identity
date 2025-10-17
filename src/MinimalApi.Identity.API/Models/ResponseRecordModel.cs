namespace MinimalApi.Identity.API.Models;

public record class AuthResponseModel(string AccessToken, string RefreshToken, DateTime ExpiredToken);
public record class ModuleResponseModel(int Id, string Name, string Description);

//TODO: Verify if needed
//public record class PermissionResponseModel(int Id, string Name, bool Default);
public record class ClaimResponseModel(int Id, string Type, string Value, bool Default);