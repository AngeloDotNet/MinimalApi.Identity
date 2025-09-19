using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Extensions;
using MinimalApi.Identity.Core.Models;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.Core.Utility.Generators;
using MinimalApi.Identity.Core.Utility.Messages;
using MinimalApi.Identity.EmailManager.Services;
using MinimalApi.Identity.ProfileManager.Models;
using MinimalApi.Identity.ProfileManager.Services;

namespace MinimalApi.Identity.API.Services;

public class AuthService(IOptions<JwtOptions> jwtOptions, IOptions<UsersOptions> usersOptions, UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, IModuleService moduleService,
    IProfileService profileService, IEmailManagerService emailManager, ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResponseModel> LoginAsync(LoginModel model)
    {
        var signInResult = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe,
            jwtOptions.Value.AllowedForNewUsers).ConfigureAwait(false);

        if (!signInResult.Succeeded)
        {
            return signInResult switch
            {
                { IsNotAllowed: true } => throw new BadRequestException(MessagesApi.UserNotAllowedLogin),
                { IsLockedOut: true } => throw new UserIsLockedException(MessagesApi.UserLockedOut),
                { RequiresTwoFactor: true } => throw new BadRequestException(MessagesApi.RequiredTwoFactor),
                _ => throw new BadRequestException(MessagesApi.InvalidCredentials)
            };
        }

        var user = await userManager.FindByNameAsync(model.Username).ConfigureAwait(false)
            ?? throw new NotFoundException(MessagesApi.UserNotFound);

        if (!user.EmailConfirmed)
        {
            throw new BadRequestException(MessagesApi.UserNotEmailConfirmed);
        }

        var profileUser = await profileService.GetProfileAsync(user.Id, CancellationToken.None).ConfigureAwait(false)
            ?? throw new NotFoundException(MessagesApi.ProfileNotFound);

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

        await userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);

        var userRolesTask = await userManager.GetRolesAsync(user);
        var userClaimsTask = await userManager.GetClaimsAsync(user);
        var customClaimsTask = await GetCustomClaimsUserAsync(user);

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

        var loginResponse = CreateToken(claims, jwtOptions.Value);

        user.RefreshToken = loginResponse.RefreshToken;
        user.RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(jwtOptions.Value.RefreshTokenExpirationMinutes);

        await userManager.UpdateAsync(user).ConfigureAwait(false);

        return loginResponse;
    }

    public async Task<string> RegisterAsync(RegisterModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email
        };

        var result = await userManager.CreateAsync(user, model.Password).ConfigureAwait(false);

        if (!result.Succeeded)
        {
            throw new BadRequestException(result.Errors);
        }

        await profileService.CreateProfileAsync(
            new CreateUserProfileModel(user.Id, model.Firstname, model.Lastname), CancellationToken.None
        ).ConfigureAwait(false);

        var role = await CheckUserIsAdminDesignedAsync(user.Email, usersOptions.Value).ConfigureAwait(false)
            ? DefaultRoles.Admin
            : DefaultRoles.User;

        var roleAssignResult = await userManager.AddToRoleAsync(user, role.ToString()).ConfigureAwait(false);

        if (!roleAssignResult.Succeeded)
        {
            throw new BadRequestException(MessagesApi.RoleNotAssigned);
        }

        var claimsAssignResult = await AddClaimsToUserAsync(user, role).ConfigureAwait(false);

        if (!claimsAssignResult.Succeeded)
        {
            throw new BadRequestException(MessagesApi.ClaimsNotAssigned);
        }

        var userId = await userManager.GetUserIdAsync(user).ConfigureAwait(false);
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);

        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var callbackUrl = await CallBackGenerator.GenerateCallBackUrlAsync(
            new GenerateCallBackUrlModel(userId, token, null), httpContextAccessor
        ).ConfigureAwait(false);

        var messageText = $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>." +
            "It is recommended to copy and paste for simplicity.";

        var emailModel = new EmailSending
        {
            EmailTo = user.Email!,
            Subject = "Confirm your email",
            Body = messageText,
            TypeEmailSendingId = (int)EmailSendingType.RegisterUser,
            TypeEmailStatusId = (int)EmailStatusType.Pending,
            DateSent = DateTime.UtcNow,
            RetrySender = 0
        };

        await emailManager.GenerateAutomaticEmailAsync(emailModel, CancellationToken.None).ConfigureAwait(false);

        return MessagesApi.UserCreated;
    }

    public async Task<AuthResponseModel> RefreshTokenAsync(RefreshTokenModel model)
    {
        var user = ValidateAccessToken(model.AccessToken)
            ?? throw new BadRequestException(MessagesExceptions.InvalidAccessToken);

        var userId = user.GetUserId();
        var dbUser = await userManager.FindByIdAsync(userId).ConfigureAwait(false);

        if (dbUser?.RefreshToken is null ||
            dbUser.RefreshTokenExpirationDate <= DateTime.UtcNow ||
            dbUser.RefreshToken != model.RefreshToken)
        {
            throw new BadRequestException(MessagesExceptions.InvalidRefreshToken);
        }

        var loginResponse = CreateToken(user.Claims.ToList(), jwtOptions.Value);

        dbUser.RefreshToken = loginResponse.RefreshToken;
        dbUser.RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(jwtOptions.Value.RefreshTokenExpirationMinutes);

        await userManager.UpdateAsync(dbUser).ConfigureAwait(false);

        return loginResponse;
    }

    public async Task<string> LogoutAsync()
    {
        await signInManager.SignOutAsync().ConfigureAwait(false);

        return MessagesApi.UserLogOut;
    }

    public async Task<AuthResponseModel> ImpersonateAsync(ImpersonateUserModel inputModel)
    {
        var user = await userManager.FindByIdAsync(inputModel.UserId.ToString()).ConfigureAwait(false)
            ?? throw new UserUnknownException("User not found");

        if (user.LockoutEnd.GetValueOrDefault() > DateTimeOffset.UtcNow)
        {
            throw new UserIsLockedException(MessagesApi.UserLockedOut);
        }

        await userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);

        var userRolesTask = userManager.GetRolesAsync(user);
        var userClaimsTask = userManager.GetClaimsAsync(user);
        var customClaimsTask = GetCustomClaimsUserAsync(user);

        await Task.WhenAll(userRolesTask, userClaimsTask, customClaimsTask).ConfigureAwait(false);

        var identity = UsersExtensions.GetIdentity(httpContextAccessor);

        UpdateClaim(ClaimTypes.NameIdentifier, user.Id.ToString());
        UpdateClaim(ClaimTypes.Name, user.UserName ?? string.Empty);
        UpdateClaim(ClaimTypes.Email, user.Email ?? string.Empty);
        UpdateClaim(ClaimTypes.SerialNumber, user.SecurityStamp!);

        var updateIdentity = identity.Claims
            .Union(userClaimsTask.Result)
            .Union(customClaimsTask.Result)
            .Union(userRolesTask.Result.Select(role => new Claim(ClaimTypes.Role, role)))
            .ToList();

        var loginResponse = CreateToken(updateIdentity, jwtOptions.Value);

        user.RefreshToken = loginResponse.RefreshToken;
        user.RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(jwtOptions.Value.RefreshTokenExpirationMinutes);

        await userManager.UpdateAsync(user).ConfigureAwait(false);

        return loginResponse;

        void UpdateClaim(string type, string value)
        {
            var existingClaim = identity.FindFirst(type);

            if (existingClaim is not null)
            {
                identity.RemoveClaim(existingClaim);
            }

            identity.AddClaim(new Claim(type, value));
        }
    }

    public async Task<string> ForgotPasswordAsync(ForgotPasswordModel inputModel)
    {
        var user = await userManager.FindByEmailAsync(inputModel.Email).ConfigureAwait(false)
            ?? throw new NotFoundException(MessagesApi.UserNotFound);

        if (!await userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false))
        {
            throw new BadRequestException(MessagesApi.ErrorEmailNotConfirmed);
        }

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));
        var messageText = $"To reset your password, you will need to indicate this code: {encodedToken}. " +
            "It is recommended to copy and paste for simplicity.";

        var emailModel = new EmailSending
        {
            EmailTo = user.Email!,
            Subject = "Reset Password",
            Body = messageText,
            TypeEmailSendingId = (int)EmailSendingType.ForgotPassword,
            TypeEmailStatusId = (int)EmailStatusType.Pending,
            DateSent = DateTime.UtcNow,
            RetrySender = 0
        };

        await emailManager.GenerateAutomaticEmailAsync(emailModel, CancellationToken.None).ConfigureAwait(false);

        return MessagesApi.SendEmailResetPassword;
    }

    public async Task<string> ResetPasswordAsync(ResetPasswordModel inputModel, string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new BadRequestException(MessagesApi.ErrorCodeResetPassword);
        }

        var user = await userManager.FindByEmailAsync(inputModel.Email).ConfigureAwait(false)
            ?? throw new NotFoundException(MessagesApi.UserNotFound);

        var result = await userManager.ResetPasswordAsync(user, code, inputModel.Password).ConfigureAwait(false);

        if (result.Succeeded)
        {
            return MessagesApi.ResetPassword;
        }

        throw new BadRequestException(result.Errors);
    }

    private static AuthResponseModel CreateToken(List<Claim> claims, JwtOptions jwtOptions)
    {
        var audienceClaim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Aud);
        if (audienceClaim is not null)
        {
            claims.Remove(audienceClaim);
        }

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            jwtOptions.Issuer,
            jwtOptions.Audience,
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(jwtOptions.AccessTokenExpirationMinutes),
            signingCredentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        var italyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        var expiredLocalNow = TimeZoneInfo.ConvertTimeFromUtc(jwtSecurityToken.ValidTo, italyTimeZone);

        return new AuthResponseModel(accessToken, GenerateRefreshToken(), expiredLocalNow);

        static string GenerateRefreshToken()
        {
            var randomNumber = new byte[256];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }

    private async Task<bool> CheckUserIsAdminDesignedAsync(string email, UsersOptions userOptions)
    {
        var user = await userManager.FindByEmailAsync(email).ConfigureAwait(false);

        return user is not null &&
               user.Email is not null &&
               user.Email.Equals(userOptions.AssignAdminEmail, StringComparison.InvariantCultureIgnoreCase);
    }

    private async Task<IdentityResult> AddClaimsToUserAsync(ApplicationUser user, DefaultRoles role)
        => role switch
        {
            DefaultRoles.Admin => await AddClaimsToAdminUserAsync(user).ConfigureAwait(false),
            DefaultRoles.User => await AddClaimsToDefaultUserAsync(user).ConfigureAwait(false),
            _ => IdentityResult.Failed()
        };

    private async Task<IdentityResult> AddClaimsToAdminUserAsync(ApplicationUser user)
    {
        var claims = Enum.GetValues<Permissions>()
            .Select(claim => new Claim(ServiceCoreExtensions.Permission, claim.ToString()))
            .ToList();

        return await userManager.AddClaimsAsync(user, claims).ConfigureAwait(false);
    }

    private async Task<IdentityResult> AddClaimsToDefaultUserAsync(ApplicationUser user)
    {
        var claims = Enum.GetValues<Permissions>()
            .Where(claim => claim.ToString().Contains("profilo", StringComparison.InvariantCultureIgnoreCase))
            .Select(claim => new Claim(ServiceCoreExtensions.Permission, claim.ToString()))
            .ToList();

        return await userManager.AddClaimsAsync(user, claims).ConfigureAwait(false);
    }

    private async Task<List<Claim>> GetCustomClaimsUserAsync(ApplicationUser user)
    {
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

    private ClaimsPrincipal ValidateAccessToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Value.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Value.Audience,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.SecurityKey)),
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var user = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);

            if (securityToken is JwtSecurityToken { Header.Alg: SecurityAlgorithms.HmacSha256 })
            {
                return user;
            }
        }
        catch
        {
            logger.LogWarning("Token validation failed");
        }

        logger.LogWarning("Token is invalid or expired");
        return null!;
    }

    private static bool CheckLastDateChangePassword(DateOnly? lastDate, UsersOptions userOptions)
        => lastDate is not null && lastDate.Value.AddDays(userOptions.PasswordExpirationDays) <= DateOnly.FromDateTime(DateTime.UtcNow);
}