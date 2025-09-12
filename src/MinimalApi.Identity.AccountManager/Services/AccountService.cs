using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MinimalApi.Identity.AccountManager.DependencyInjection;
using MinimalApi.Identity.AccountManager.Models;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.EmailManager.Services;

namespace MinimalApi.Identity.AccountManager.Services;

public class AccountService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor,
    IEmailManagerService emailManager) : IAccountService
{
    public async Task<string> ConfirmEmailAsync(ConfirmEmailModel request)
        => await AccountQuery.ConfirmEmailAsync(userManager, request);

    public async Task<string> ChangeEmailAsync(ChangeEmailModel request)
        => await AccountQuery.ChangeEmailAsync(userManager, request, httpContextAccessor, emailManager);

    public async Task<string> ConfirmEmailChangeAsync(ConfirmEmailChangeModel request)
        => await AccountQuery.ConfirmEmailChangeAsync(userManager, request);
}