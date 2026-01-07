using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Settings;

namespace MinimalApi.Identity.AuthManager.HostedServices;

public class AuthenticationStartupTask(IServiceProvider serviceProvider, IConfiguration configuration) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var settings = new AppSettings();

        configuration.Bind(nameof(AppSettings), settings);

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var administratorUser = new ApplicationUser
        {
            UserName = settings.AssignAdminUsername,
            Email = settings.AssignAdminEmail,
            EmailConfirmed = true,
            LockoutEnabled = false,
            TwoFactorEnabled = false,
            RefreshToken = "",

            // TODO: Spostare in background job
            UserProfile = new UserProfile
            {
                FirstName = "Application",
                LastName = "Admin",
                IsEnabled = true,
                LastDateChangePassword = ConstantsConfiguration.DateOnlyToday
            }
            //EmailConfirmed = true,
            //LockoutEnabled = false,
            //TwoFactorEnabled = false,
            //RefreshToken = ""
        };

        await CheckCreateUserAsync(userManager, administratorUser, settings.AssignAdminPassword, nameof(DefaultRoles.Admin));
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    internal static async Task CheckCreateUserAsync(UserManager<ApplicationUser> userManager, ApplicationUser user, string password, params string[] roles)
    {
        if (user.Email is null)
        {
            throw new InvalidOperationException("User email cannot be null");
        }

        var dbUser = await userManager.FindByEmailAsync(user.Email);

        if (dbUser == null)
        {
            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRolesAsync(user, roles);
            }
        }
    }
}