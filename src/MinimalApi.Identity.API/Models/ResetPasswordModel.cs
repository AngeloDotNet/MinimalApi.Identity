namespace MinimalApi.Identity.API.Models;

public record class ResetPasswordModel(string Email, string Password, string ConfirmPassword);