using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MinimalApi.Identity.AccountManager.DependencyInjection;
using MinimalApi.Identity.AccountManager.Models;
using MinimalApi.Identity.Core.Entities;

namespace MinimalApi.Identity.AccountManager.Services;

public class AccountService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor) : IAccountService
{
    public async Task<string> ConfirmEmailAsync(ConfirmEmailModel request) => await AccountQuery.ConfirmEmailAsync(userManager, request);
    //{
    //    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
    //    {
    //        throw new BadRequestUserException(AccountExtensions.UserIdTokenRequired);
    //    }

    //    var user = await userManager.FindByIdAsync(userId)
    //        ?? throw new BadRequestUserException(AccountExtensions.UserNotFound);

    //    var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
    //    var result = await userManager.ConfirmEmailAsync(user, code);

    //    return result.Succeeded ? AccountExtensions.ConfirmingEmail : throw new BadRequestException(AccountExtensions.ErrorConfirmEmail);
    //}

    public async Task<string> ChangeEmailAsync(ChangeEmailModel request)
        => await AccountQuery.ChangeEmailAsync(userManager, request, httpContextAccessor);
    //{
    //    if (inputModel.NewEmail == null)
    //    {
    //        throw new BadRequestException(AccountExtensions.NewEmailIsRequired);
    //    }

    //    var user = await userManager.FindByEmailAsync(inputModel.Email)
    //        ?? throw new BadRequestUserException(AccountExtensions.UserNotFound);

    //    var userId = await userManager.GetUserIdAsync(user);
    //    var token = await userManager.GenerateChangeEmailTokenAsync(user, inputModel.NewEmail);
    //    token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

    //    var callbackUrl = await GenerateCallBackUrlAsync(userId, token, inputModel.NewEmail);
    //    await emailSender.SendEmailTypeAsync(inputModel.NewEmail, callbackUrl, EmailSendingType.ChangeEmail);

    //    return AccountExtensions.SendEmailForChangeEmail;
    //}

    public async Task<string> ConfirmEmailChangeAsync(ConfirmEmailChangeModel request)
        => await AccountQuery.ConfirmEmailChangeAsync(userManager, request);
    //{
    //    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
    //    {
    //        throw new BadRequestUserException(AccountExtensions.UserIdEmailTokenRequired);
    //    }

    //    var user = await userManager.FindByIdAsync(userId)
    //        ?? throw new BadRequestUserException(AccountExtensions.UserNotFound);

    //    var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
    //    var result = await userManager.ChangeEmailAsync(user, email, code);

    //    //... Omitted code for username update as in our case username and email are two separate fields.
    //    //Reference: Lines 57 - 66 in ConfirmEmailChange.cshtml.cs file

    //    return result.Succeeded ? AccountExtensions.ConfirmingEmailChanged : throw new BadRequestException(AccountExtensions.ErrorConfirmEmailChange);
    //}
}
