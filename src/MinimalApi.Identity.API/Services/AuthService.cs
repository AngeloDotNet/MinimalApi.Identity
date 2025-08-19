using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Extensions;
using MinimalApi.Identity.Core.Models;
using MinimalApi.Identity.Core.Utility.Generators;
using MinimalApi.Identity.Core.Utility.Messages;

namespace MinimalApi.Identity.API.Services;

//public class AuthService(IOptions<JwtOptions> jwtOptions, IOptions<UsersOptions> usersOptions, UserManager<ApplicationUser> userManager,
//    SignInManager<ApplicationUser> signInManager, IEmailSenderService emailSender, IHttpContextAccessor httpContextAccessor,
//    IModuleService moduleService, IProfileService profileService) : IAuthService
//public class AuthService(IOptions<JwtOptions> jwtOptions, IOptions<UsersOptions> usersOptions, UserManager<ApplicationUser> userManager,
//    SignInManager<ApplicationUser> signInManager, IEmailSenderService emailSender, IHttpContextAccessor httpContextAccessor, IModuleService moduleService) : IAuthService
public class AuthService(IOptions<JwtOptions> jwtOptions, IOptions<UsersOptions> usersOptions, UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, IModuleService moduleService) : IAuthService
{
    public async Task<AuthResponseModel> LoginAsync(LoginModel model)
    {
        var signInResult = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, jwtOptions.Value.AllowedForNewUsers);

        if (!signInResult.Succeeded)
        {
            return signInResult switch
            {
                { IsNotAllowed: true } => throw new BadRequestException(MessagesAPI.UserNotAllowedLogin),
                { IsLockedOut: true } => throw new UserIsLockedException(MessagesAPI.UserLockedOut),
                { RequiresTwoFactor: true } => throw new BadRequestException(MessagesAPI.RequiredTwoFactor),
                _ => throw new BadRequestException(MessagesAPI.InvalidCredentials)
            };
        }

        var user = await userManager.FindByNameAsync(model.Username)
            ?? throw new NotFoundException(MessagesAPI.UserNotFound);

        if (!user.EmailConfirmed)
        {
            throw new BadRequestException(MessagesAPI.UserNotEmailConfirmed);
        }

        //TODO: Integrate profile service to get user profile
        //var profileUser = await profileService.GetProfileAsync(user.Id) ?? throw new NotFoundException(MessagesAPI.ProfileNotFound);

        //if (!profileUser.IsEnabled)
        //{
        //    throw new BadRequestException(MessagesAPI.UserNotEnableLogin);
        //}

        //var lastDateChangePassword = profileUser.LastDateChangePassword;
        //var checkLastDateChangePassword = CheckLastDateChangePassword(lastDateChangePassword, usersOptions.Value);

        //if (lastDateChangePassword == null || checkLastDateChangePassword)
        //{
        //    throw new BadRequestException(MessagesAPI.UserForcedChangePassword);
        //}

        await userManager.UpdateSecurityStampAsync(user);

        var userRoles = await userManager.GetRolesAsync(user);
        var userClaims = await userManager.GetClaimsAsync(user);

        var customClaims = await GetCustomClaimsUserAsync(user);
        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.SerialNumber, user.SecurityStamp!.ToString()),
        }
        .Union(userClaims)
        .Union(customClaims)
        .Union(userRoles.Select(role => new Claim(ClaimTypes.Role, role))).ToList();

        var loginResponse = CreateToken(claims, jwtOptions.Value);

        user.RefreshToken = loginResponse.RefreshToken;
        user.RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(60);

        await userManager.UpdateAsync(user);

        return loginResponse;
    }

    public async Task<string> RegisterAsync(RegisterModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email
        };

        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            //TODO: Integrate profile service to create user profile
            //await profileService.CreateProfileAsync(new CreateUserProfileModel(user.Id, model.Firstname, model.Lastname));

            var role = await CheckUserIsAdminDesignedAsync(user.Email, usersOptions.Value) ? DefaultRoles.Admin : DefaultRoles.User;
            var roleAssignResult = await userManager.AddToRoleAsync(user, role.ToString());

            if (!roleAssignResult.Succeeded)
            {
                throw new BadRequestException(MessagesAPI.RoleNotAssigned);
            }

            var claimsAssignResult = await AddClaimsToUserAsync(user, role);

            if (!claimsAssignResult.Succeeded)
            {
                throw new BadRequestException(MessagesAPI.ClaimsNotAssigned);
            }

            var userId = await userManager.GetUserIdAsync(user);
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var callbackUrl = await CallBackGenerator.GenerateCallBackUrlAsync(new GenerateCallBackUrlModel(userId, token, null), httpContextAccessor);
            var messageText = $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>." +
                "It is recommended to copy and paste for simplicity.";

            //TODO: Integrate email sender service to send emails
            //await emailSender.SendEmailAsync(user.Email!, "Confirm your email", messageText, EmailSendingType.RegisterUser);

            return MessagesAPI.UserCreated;
        }

        throw new BadRequestException(result.Errors);
    }

    public async Task<AuthResponseModel> RefreshTokenAsync(RefreshTokenModel model)
    {
        var user = ValidateAccessToken(model.AccessToken)
            ?? throw new BadRequestException(MessagesExceptions.InvalidAccessToken);

        var userId = user.GetUserId();
        var dbUser = await userManager.FindByIdAsync(userId);

        if (dbUser?.RefreshToken == null || dbUser.RefreshTokenExpirationDate <= DateTime.UtcNow || dbUser.RefreshToken != model.RefreshToken)
        {
            throw new BadRequestException(MessagesExceptions.InvalidRefreshToken);
        }

        var loginResponse = CreateToken(user.Claims.ToList(), jwtOptions.Value);

        dbUser.RefreshToken = loginResponse.RefreshToken;
        dbUser.RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(jwtOptions.Value.RefreshTokenExpirationMinutes);

        await userManager.UpdateAsync(dbUser);

        return loginResponse;
    }

    public async Task<string> LogoutAsync()
    {
        await signInManager.SignOutAsync();

        return MessagesAPI.UserLogOut;
    }

    public async Task<AuthResponseModel> ImpersonateAsync(ImpersonateUserModel inputModel)
    {
        var user = await userManager.FindByIdAsync(inputModel.UserId.ToString()) ?? throw new UserUnknownException($"User not found");

        if (user.LockoutEnd.GetValueOrDefault() > DateTimeOffset.UtcNow)
        {
            throw new UserIsLockedException(MessagesAPI.UserLockedOut);
        }

        await userManager.UpdateSecurityStampAsync(user);

        var userRoles = await userManager.GetRolesAsync(user);
        var userClaims = await userManager.GetClaimsAsync(user);

        var customClaims = await GetCustomClaimsUserAsync(user);
        var identity = UsersExtensions.GetIdentity(httpContextAccessor);

        UpdateClaim(ClaimTypes.NameIdentifier, user.Id.ToString());
        UpdateClaim(ClaimTypes.Name, user.UserName ?? string.Empty);
        UpdateClaim(ClaimTypes.Email, user.Email ?? string.Empty);
        UpdateClaim(ClaimTypes.SerialNumber, user.SecurityStamp!.ToString());

        var updateIdentity = identity.Claims
            .Union(userClaims)
            .Union(customClaims)
            .Union(userRoles.Select(role => new Claim(ClaimTypes.Role, role))).ToList();

        var loginResponse = CreateToken(updateIdentity, jwtOptions.Value);

        user.RefreshToken = loginResponse.RefreshToken;
        user.RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(jwtOptions.Value.RefreshTokenExpirationMinutes);

        await userManager.UpdateAsync(user);

        return loginResponse;

        void UpdateClaim(string type, string value)
        {
            var existingClaim = identity.FindFirst(type);

            if (existingClaim != null)
            {
                identity.RemoveClaim(existingClaim);
            }

            identity.AddClaim(new Claim(type, value));
        }
    }

    public async Task<string> ForgotPasswordAsync(ForgotPasswordModel inputModel)
    {
        var user = await userManager.FindByEmailAsync(inputModel.Email)
            ?? throw new NotFoundException(MessagesAPI.UserNotFound);

        if (!await userManager.IsEmailConfirmedAsync(user))
        {
            throw new BadRequestException(MessagesAPI.ErrorEmailNotConfirmed);
        }

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));
        var messageText = $"To reset your password, you will need to indicate this code: {encodedToken}. " +
            "It is recommended to copy and paste for simplicity.";

        //TODO: Integrate email sender service to send emails
        //await emailSender.SendEmailAsync(user.Email!, "Reset Password", messageText, EmailSendingType.ForgotPassword);

        return MessagesAPI.SendEmailResetPassword;
    }

    public async Task<string> ResetPasswordAsync(ResetPasswordModel inputModel, string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new BadRequestException(MessagesAPI.ErrorCodeResetPassword);
        }

        var user = await userManager.FindByEmailAsync(inputModel.Email)
            ?? throw new NotFoundException(MessagesAPI.UserNotFound);

        var result = await userManager.ResetPasswordAsync(user, code, inputModel.Password);

        if (result.Succeeded)
        {
            return MessagesAPI.ResetPassword;
        }
        else
        {
            throw new BadRequestException(result.Errors);
        }
    }

    private static AuthResponseModel CreateToken(List<Claim> claims, JwtOptions jwtOptions)
    {
        var audienceClaim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Aud);
        claims.Remove(audienceClaim!);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(jwtOptions.Issuer, jwtOptions.Audience, claims,
            DateTime.UtcNow, DateTime.UtcNow.AddMinutes(jwtOptions.AccessTokenExpirationMinutes), signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        var italyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        var expiredLocalNow = TimeZoneInfo.ConvertTimeFromUtc(jwtSecurityToken.ValidTo, italyTimeZone);

        var response = new AuthResponseModel(accessToken, GenerateRefreshToken(), expiredLocalNow);

        return response;

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
        var user = await userManager.FindByEmailAsync(email);

        if (user is null || user.Email is null)
        {
            return false;
        }

        return user.Email.Equals(userOptions.AssignAdminEmail, StringComparison.InvariantCultureIgnoreCase);
    }

    private async Task<IdentityResult> AddClaimsToUserAsync(ApplicationUser user, DefaultRoles role)
    {
        return role switch
        {
            DefaultRoles.Admin => await AddClaimsToAdminUserAsync(user),
            DefaultRoles.User => await AddClaimsToDefaultUserAsync(user),
            _ => IdentityResult.Failed()
        };
    }

    private async Task<IdentityResult> AddClaimsToAdminUserAsync(ApplicationUser user)
    {
        var claims = Enum.GetValues<Permissions>()
            .Select(claim => new Claim(ServiceCoreExtensions.Permission, claim.ToString()))
            .ToList();

        return await userManager.AddClaimsAsync(user, claims);
    }

    private async Task<IdentityResult> AddClaimsToDefaultUserAsync(ApplicationUser user)
    {
        var claims = Enum.GetValues<Permissions>()
            .Where(claim => claim.ToString().Contains("profilo", StringComparison.InvariantCultureIgnoreCase))
            .Select(claim => new Claim(ServiceCoreExtensions.Permission, claim.ToString()))
            .ToList();

        return await userManager.AddClaimsAsync(user, claims);
    }

    private async Task<List<Claim>> GetCustomClaimsUserAsync(ApplicationUser user)
    {
        var customClaims = new List<Claim>();
        var userProfile = new List<Claim>();
        var userClaimModules = new List<Claim>();

        //TODO: Integrate licenseService to get user license claims

        //TODO: Integrate profileService to get user profile claims
        var task = new List<Task> {
            //Task.Run(async () => userProfile = await profileService.GetClaimUserProfileAsync(user)),
            Task.Run(async () => userClaimModules = await moduleService.GetClaimsModuleUserAsync(user))
        };

        await Task.WhenAll(task);

        if (userProfile != null)
        {
            customClaims.AddRange(userProfile);
        }

        if (userClaimModules?.Count > 0)
        {
            customClaims.AddRange(userClaimModules);
        }

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

            if (securityToken is JwtSecurityToken jwtSecurityToken && jwtSecurityToken.Header.Alg == SecurityAlgorithms.HmacSha256)
            {
                return user;
            }
        }
        catch
        {
            // Token validation failed
            // Log the exception if needed
        }

        // Token is invalid or expired
        // Log the invalid token if needed
        return null!;
    }

    private static bool CheckLastDateChangePassword(DateOnly? lastDate, UsersOptions userOptions)
    {
        if (lastDate!.Value.AddDays(userOptions.PasswordExpirationDays) <= DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return true;
        }

        return false;
    }
}