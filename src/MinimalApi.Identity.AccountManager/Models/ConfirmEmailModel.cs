namespace MinimalApi.Identity.AccountManager.Models;

public record class ConfirmEmailModel(string UserId, string Token);
