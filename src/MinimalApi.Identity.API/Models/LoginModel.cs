namespace MinimalApi.Identity.API.Models;

public record class LoginModel(string Username, string Password, bool RememberMe);