using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Models;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.Core.Utility.Generators;
using MinimalApi.Identity.Core.Utility.Messages;
using MinimalApi.Identity.EmailManager.Services;
using MinimalApi.Identity.ProfileManager.Models;
using MinimalApi.Identity.ProfileManager.Services;

namespace MinimalApi.Identity.API.DependencyInjection;

public static class AuthExtensions
{
    public static async Task CheckUserProfileAndPasswordAsync(ApplicationUser user, IOptions<UsersOptions> usersOptions, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var profileService = scope.ServiceProvider.GetRequiredService<IProfileService>();

        var profileUser = await profileService.GetProfileAsync(user.Id, CancellationToken.None)
            .ConfigureAwait(false) ?? throw new NotFoundException(MessagesApi.ProfileNotFound);

        if (!profileUser.IsEnabled)
        {
            throw new BadRequestException(MessagesApi.UserNotEnableLogin);
        }

        var lastDateChangePassword = profileUser.LastDateChangePassword;
        var checkLastDateChangePassword = CheckLastDateChangePassword(lastDateChangePassword, usersOptions.Value);

        if (lastDateChangePassword is null || checkLastDateChangePassword)
        {
            throw new BadRequestException(MessagesApi.UserForcedChangePassword);
        }
    }

    public static async Task<List<Claim>> GetCustomClaimsUserAsync(ApplicationUser user, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var profileService = scope.ServiceProvider.GetRequiredService<IProfileService>();
        var moduleService = scope.ServiceProvider.GetRequiredService<IModuleService>();

        var userProfileTask = await profileService.GetClaimUserProfileAsync(user, CancellationToken.None);
        var userClaimModulesTask = await moduleService.GetClaimsModuleUserAsync(user);

        // TODO: Integrate licenseService to get user license claims
        // var userLicensesTask = licenseService.GetClaimsLicenseUserAsync(user, CancellationToken.None);

        var customClaims = new List<Claim>();

        if (userProfileTask is { Count: > 0 })
        {
            customClaims.AddRange(userProfileTask);
        }

        if (userClaimModulesTask is { Count: > 0 })
        {
            customClaims.AddRange(userClaimModulesTask);
        }

        // if (userLicensesTask.Result is { Count: > 0 })
        // {
        //     customClaims.AddRange(userLicensesTask.Result);
        // }

        return customClaims;
    }

    public static async Task CreateProfileAsync(RegisterModel model, ApplicationUser user, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var profileService = scope.ServiceProvider.GetRequiredService<IProfileService>();

        await profileService.CreateProfileAsync(new CreateUserProfileModel(user.Id, model.Firstname, model.Lastname), CancellationToken.None).ConfigureAwait(false);
    }

    public static bool CheckLastDateChangePassword(DateOnly? lastDate, UsersOptions userOptions)
        => lastDate is not null && lastDate.Value.AddDays(userOptions.PasswordExpirationDays) <= DateOnly.FromDateTime(DateTime.UtcNow);

    public static async Task SendEmailRegisterUserAsync(UserManager<ApplicationUser> userManager, ApplicationUser user, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var emailManager = scope.ServiceProvider.GetRequiredService<IEmailManagerService>();

        var userId = await userManager.GetUserIdAsync(user).ConfigureAwait(false);
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);

        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var callbackUrl = await CallBackGenerator.GenerateCallBackUrlAsync(new GenerateCallBackUrlModel(userId, token, null),
            httpContextAccessor).ConfigureAwait(false);

        var emailModel = new EmailSending
        {
            EmailTo = user.Email!,
            Subject = "Confirm your email",
            Body = $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>. It is recommended to copy and paste for simplicity.",
            TypeEmailSendingId = (int)EmailSendingType.RegisterUser,
            TypeEmailStatusId = (int)EmailStatusType.Pending,
            DateSent = DateTime.UtcNow,
            RetrySender = 0
        };

        await emailManager.GenerateAutomaticEmailAsync(emailModel, CancellationToken.None).ConfigureAwait(false);
    }

    public static async Task SendEmailForgotPasswordAsync(UserManager<ApplicationUser> userManager, ApplicationUser user, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var emailManager = scope.ServiceProvider.GetRequiredService<IEmailManagerService>();

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));

        var emailModel = new EmailSending
        {
            EmailTo = user.Email!,
            Subject = "Reset Password",
            Body = $"To reset your password, you will need to indicate this code: {encodedToken}. It is recommended to copy and paste for simplicity.",
            TypeEmailSendingId = (int)EmailSendingType.ForgotPassword,
            TypeEmailStatusId = (int)EmailStatusType.Pending,
            DateSent = DateTime.UtcNow,
            RetrySender = 0
        };

        await emailManager.GenerateAutomaticEmailAsync(emailModel, CancellationToken.None).ConfigureAwait(false);
    }
}