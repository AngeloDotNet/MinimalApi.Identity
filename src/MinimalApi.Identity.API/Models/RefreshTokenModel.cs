namespace MinimalApi.Identity.API.Models;

public record class RefreshTokenModel(string AccessToken, string RefreshToken);