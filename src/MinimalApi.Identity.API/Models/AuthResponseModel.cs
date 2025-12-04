namespace MinimalApi.Identity.API.Models;

public record class AuthResponseModel(string AccessToken, string RefreshToken, DateTime ExpiredToken);