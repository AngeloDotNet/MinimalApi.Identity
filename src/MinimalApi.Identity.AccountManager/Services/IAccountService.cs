using MinimalApi.Identity.AccountManager.Models;

namespace MinimalApi.Identity.AccountManager.Services;

public interface IAccountService
{
    Task<string> ConfirmEmailAsync(ConfirmEmailModel request);
    Task<string> ChangeEmailAsync(ChangeEmailModel request);
    Task<string> ConfirmEmailChangeAsync(ConfirmEmailChangeModel request);
}
