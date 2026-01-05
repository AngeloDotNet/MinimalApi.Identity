namespace MinimalApi.Identity.AccountManager.Models;

public record class ConfirmEmailChangeModel(string UserId, string Email, string Token);

