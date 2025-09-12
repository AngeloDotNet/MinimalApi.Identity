using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Options;

namespace MinimalApi.Identity.AuthManager.HostedServices;

public class AuthenticationStartupTask(IServiceProvider serviceProvider, IConfiguration configuration) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var usersOptions = new UsersOptions();

        configuration.Bind(nameof(UsersOptions), usersOptions);

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var administratorUser = new ApplicationUser
        {
            UserName = usersOptions.AssignAdminUsername,
            Email = usersOptions.AssignAdminEmail,
            UserProfile = new UserProfile
            {
                FirstName = "Application",
                LastName = "Admin"
            },
            EmailConfirmed = true,
            LockoutEnabled = false,
            TwoFactorEnabled = false,
            RefreshToken = null!
        };

        await CheckCreateUserAsync(userManager, administratorUser, usersOptions.AssignAdminPassword, nameof(DefaultRoles.Admin));
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