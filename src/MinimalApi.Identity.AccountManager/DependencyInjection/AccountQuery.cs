using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using MinimalApi.Identity.AccountManager.Models;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Models;
using MinimalApi.Identity.Core.Utility.Generators;
using MinimalApi.Identity.Core.Utility.Messages;

namespace MinimalApi.Identity.AccountManager.DependencyInjection;

public static class AccountQuery
{
    public static async Task<string> ConfirmEmailAsync(UserManager<ApplicationUser> userManager, ConfirmEmailModel request)
    {
        if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.Token))
        {
            throw new BadRequestException(MessagesApi.UserIdTokenRequired);
        }

        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new BadRequestException(MessagesApi.UserNotFound);

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
        var result = await userManager.ConfirmEmailAsync(user, code);

        return result.Succeeded ? MessagesApi.ConfirmingEmail : throw new BadRequestException(MessagesApi.ErrorConfirmEmail);
    }

    public static async Task<string> ChangeEmailAsync(UserManager<ApplicationUser> userManager, ChangeEmailModel request, IHttpContextAccessor httpContextAccessor)
    {
        {
            if (request.NewEmail == null)
            {
                throw new BadRequestException(MessagesApi.RequiredNewEmail);
            }

            var user = await userManager.FindByEmailAsync(request.Email)
                ?? throw new BadRequestException(MessagesApi.UserNotFound);

            var userId = await userManager.GetUserIdAsync(user);
            var token = await userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var callbackUrl = await CallBackGenerator.GenerateCallBackUrlAsync(new GenerateCallBackUrlModel(userId, token, request.NewEmail), httpContextAccessor);
            var messageText = $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>." +
                "It is recommended to copy and paste for simplicity.";

            //TODO: Integrate email sender service to send emails
            //await emailSender.SendEmailAsync(user.Email!, "Confirm your email", messageText, EmailSendingType.ChangeEmail); //Use this
            //await emailSender.SendEmailTypeAsync(request.NewEmail, callbackUrl, EmailSendingType.ChangeEmail); // Don't use this

            return MessagesApi.SendEmailForChangeEmail;
        }
    }

    public static async Task<string> ConfirmEmailChangeAsync(UserManager<ApplicationUser> userManager, ConfirmEmailChangeModel request)
    {
        if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Token))
        {
            throw new BadRequestException(MessagesApi.UserIdEmailTokenRequired);
        }

        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new BadRequestException(MessagesApi.UserNotFound);

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
        var result = await userManager.ChangeEmailAsync(user, request.Email, code);

        //... Omitted code for username update as in our case username and email are two separate fields.
        //Reference: Lines 57 - 66 in ConfirmEmailChange.cshtml.cs file

        return result.Succeeded ? MessagesApi.ConfirmingEmailChanged : throw new BadRequestException(MessagesApi.ErrorConfirmEmailChange);
    }

    //private static Task<string> GenerateCallBackUrlAsync(string userId, string token, string newEmail, IHttpContextAccessor httpContextAccessor)
    //{
    //    var request = httpContextAccessor.HttpContext!.Request;
    //    var endpoint = AccountExtensions.EndpointsConfirmEmailChange
    //        .Replace("{userId}", userId)
    //        .Replace("{email}", newEmail)
    //        .Replace("{token}", token);

    //    var callbackUrl = string.Concat(request.Scheme, "://", request.Host, AccountExtensions.EndpointsAccountGroup, endpoint);

    //    return Task.FromResult(callbackUrl);
    //}
}