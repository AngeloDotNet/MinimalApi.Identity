using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Models;
using MinimalApi.Identity.Core.Settings;
using MinimalApi.Identity.Core.Utility.Generators;
using MinimalApi.Identity.Core.Utility.Messages;
using MinimalApi.Identity.EmailManager.Services;
using MinimalApi.Identity.LicenseManager.Services;
using MinimalApi.Identity.ModuleManager.Services;
using MinimalApi.Identity.ProfileManager.Models;
using MinimalApi.Identity.ProfileManager.Services;

namespace MinimalApi.Identity.API.DependencyInjection;

public static class AuthExtensions
{
    public static async Task CheckUserAsync(ApplicationUser user, IOptions<AppSettings> options, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        if (!user.EmailConfirmed)
        {
            throw new BadRequestException(MessagesApi.UserNotEmailConfirmed);
        }

        var profileService = scope.ServiceProvider.GetRequiredService<IProfileService>();
        var profileUser = await profileService.GetProfileAsync(user.Id, CancellationToken.None)
            .ConfigureAwait(false) ?? throw new NotFoundException(MessagesApi.ProfileNotFound);

        if (!profileUser.IsEnabled)
        {
            throw new BadRequestException(MessagesApi.UserNotEnableLogin);
        }

        var lastDateChangePassword = profileUser.LastDateChangePassword;
        var checkLastDateChangePassword = CheckLastDateChangePassword(lastDateChangePassword, options.Value);

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
        var licenseService = scope.ServiceProvider.GetRequiredService<ILicenseService>();

        var userProfileTask = await profileService.GetClaimUserProfileAsync(user, CancellationToken.None);
        var userClaimModulesTask = await moduleService.GetClaimsModuleUserAsync(user);

        var userLicensesTask = await licenseService.GetClaimLicenseUserAsync(user, CancellationToken.None);
        var customClaims = new List<Claim>();

        if (userProfileTask is { Count: > 0 })
        {
            customClaims.AddRange(userProfileTask);
        }

        if (userClaimModulesTask is { Count: > 0 })
        {
            customClaims.AddRange(userClaimModulesTask);
        }

        if (userLicensesTask is not null)
        {
            customClaims.Add(userLicensesTask);
        }

        return customClaims;
    }

    // TODO: Spostare in background job
    public static async Task CreateProfileAsync(RegisterModel model, ApplicationUser user, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var profileService = scope.ServiceProvider.GetRequiredService<IProfileService>();

        await profileService.CreateProfileAsync(new CreateUserProfileModel(user.Id, model.Firstname, model.Lastname), CancellationToken.None)
            .ConfigureAwait(false);
    }

    // TODO: Spostare in background job
    public static async Task UpdateDateLastChangePasswordAsync(int userId, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var profileService = scope.ServiceProvider.GetRequiredService<IProfileService>();

        await profileService.UpdateLastDateChangePasswordAsync(userId, CancellationToken.None).ConfigureAwait(false);
    }

    public static bool CheckLastDateChangePassword(DateOnly? lastDate, AppSettings options)
        => lastDate is not null && lastDate.Value.AddDays(options.PasswordExpirationDays) <= DateOnly.FromDateTime(DateTime.UtcNow);

    // TODO: Spostare in background job
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

    // TODO: Spostare in background job
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

    public static async Task<List<Claim>> GenerateUserClaimsAsync(UserManager<ApplicationUser> userManager, ApplicationUser user, IServiceProvider serviceProvider)
    {
        var userRolesTask = await userManager.GetRolesAsync(user);
        var userClaimsTask = await userManager.GetClaimsAsync(user);
        var customClaimsTask = await GetCustomClaimsUserAsync(user, serviceProvider);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.SerialNumber, user.SecurityStamp!)
        };
        claims.AddRange(userClaimsTask);
        claims.AddRange(userRolesTask.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(customClaimsTask);

        return claims;
    }

    public static async Task AssignRoleAndClaimsUserAsync(UserManager<ApplicationUser> userManager, ApplicationUser user, AppSettings options)
    {
        if (user.Email is null)
        {
            throw new BadRequestException(MessagesApi.EmailNotFound);
        }

        var role = await CheckUserIsAdminDesignedAsync(userManager, user.Email, options)
            .ConfigureAwait(false) ? DefaultRoles.Admin : DefaultRoles.User;

        var roleAssignResult = await userManager.AddToRoleAsync(user, role.ToString()).ConfigureAwait(false);

        if (!roleAssignResult.Succeeded)
        {
            throw new BadRequestException(MessagesApi.RoleNotAssigned);
        }

        var claimsAssignResult = await AddClaimsToUserAsync(userManager, user, role).ConfigureAwait(false);

        if (!claimsAssignResult.Succeeded)
        {
            throw new BadRequestException(MessagesApi.ClaimsNotAssigned);
        }
    }

    public static async Task<bool> CheckUserIsAdminDesignedAsync(UserManager<ApplicationUser> userManager, string email, AppSettings options)
    {
        var user = await userManager.FindByEmailAsync(email).ConfigureAwait(false);

        return user is not null && user.Email is not null && user.Email.Equals(options.AssignAdminEmail, StringComparison.InvariantCultureIgnoreCase);
    }

    public static async Task<IdentityResult> AddClaimsToUserAsync(UserManager<ApplicationUser> userManager, ApplicationUser user, DefaultRoles role)
        => role switch
        {
            DefaultRoles.Admin => await AddClaimsToAdminUserAsync(userManager, user).ConfigureAwait(false),
            DefaultRoles.User => await AddClaimsToDefaultUserAsync(userManager, user).ConfigureAwait(false),
            _ => IdentityResult.Failed()
        };

    private static async Task<IdentityResult> AddClaimsToAdminUserAsync(UserManager<ApplicationUser> userManager, ApplicationUser user)
    {
        var claims = Enum.GetValues<Permissions>()
            .Select(claim => new Claim(ServiceCoreExtensions.Permission, claim.ToString()))
            .ToList();

        return await userManager.AddClaimsAsync(user, claims).ConfigureAwait(false);
    }

    private static async Task<IdentityResult> AddClaimsToDefaultUserAsync(UserManager<ApplicationUser> userManager, ApplicationUser user)
    {
        var claims = Enum.GetValues<Permissions>()
            .Where(claim => claim.ToString().Contains("profilo", StringComparison.InvariantCultureIgnoreCase))
            .Select(claim => new Claim(ServiceCoreExtensions.Permission, claim.ToString()))
            .ToList();

        return await userManager.AddClaimsAsync(user, claims).ConfigureAwait(false);
    }
}