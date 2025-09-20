using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Identity.API.DependencyInjection;
using MinimalApi.Identity.API.Models;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.Core.DependencyInjection;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Exceptions;
using MinimalApi.Identity.Core.Extensions;
using MinimalApi.Identity.Core.Options;
using MinimalApi.Identity.Core.Utility.Messages;

namespace MinimalApi.Identity.API.Services;

public class AuthService(IOptions<JwtOptions> jwtOptions, IOptions<UsersOptions> usersOptions, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
    IHttpContextAccessor httpContextAccessor, ILogger<AuthService> logger, IServiceProvider serviceProvider) : IAuthService
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

        await AuthExtensions.CheckUserProfileAndPasswordAsync(user, usersOptions, serviceProvider).ConfigureAwait(false);

        await userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);

        var userRolesTask = await userManager.GetRolesAsync(user);
        var userClaimsTask = await userManager.GetClaimsAsync(user);
        var customClaimsTask = await AuthExtensions.GetCustomClaimsUserAsync(user, serviceProvider);

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

        await AuthExtensions.CreateProfileAsync(model, user, serviceProvider).ConfigureAwait(false);

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

        await AuthExtensions.SendEmailRegisterUserAsync(userManager, user, httpContextAccessor, serviceProvider).ConfigureAwait(false);

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
        var customClaimsTask = AuthExtensions.GetCustomClaimsUserAsync(user, serviceProvider);

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
        var user = await userManager.FindByEmailAsync(inputModel.Email)
            .ConfigureAwait(false) ?? throw new NotFoundException(MessagesApi.UserNotFound);

        if (!await userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false))
        {
            throw new BadRequestException(MessagesApi.ErrorEmailNotConfirmed);
        }

        await AuthExtensions.SendEmailForgotPasswordAsync(userManager, user, serviceProvider).ConfigureAwait(false);

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

        return user is not null && user.Email is not null && user.Email.Equals(userOptions.AssignAdminEmail, StringComparison.InvariantCultureIgnoreCase);
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
}