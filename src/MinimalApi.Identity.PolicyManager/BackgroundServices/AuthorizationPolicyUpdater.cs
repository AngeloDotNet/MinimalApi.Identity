using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.PolicyManager.Services;

namespace MinimalApi.Identity.PolicyManager.BackgroundServices;

public class AuthorizationPolicyUpdater(IServiceScopeFactory serviceScopeFactory, ILogger<AuthorizationPolicyUpdater> logger,
    IOptions<HostedServiceOptions> hostedOptions) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new System.Timers.Timer
        {
            Interval = TimeSpan.FromMinutes(hostedOptions.Value.IntervalAuthPolicyUpdaterMinutes).TotalMilliseconds
        };

        timer.Elapsed += Timer_ElapsedAsync;
        timer.Start();

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async void Timer_ElapsedAsync(object? sender, ElapsedEventArgs e)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var authPolicyService = scope.ServiceProvider.GetRequiredService<IAuthPolicyService>();

        try
        {
            var result = await authPolicyService.UpdateAuthPoliciesAsync();

            if (result)
            {
                logger.LogInformation("Authorization policies updated successfully.");
            }
            else
            {
                logger.LogWarning("An error occurred while generating authorization policies.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred while updating authorization policies.");
        }
    }
}

//public class AuthorizationPolicyUpdater(IServiceProvider serviceProvider, ILogger<AuthorizationPolicyUpdater> logger,
//    IOptions<HostedServiceOptions> hostedOptions) : IHostedService
//{
//    private Timer? timer;
//    private readonly HostedServiceOptions options = hostedOptions.Value;

//    public Task StartAsync(CancellationToken cancellationToken)
//    {
//        timer = new Timer(UpdateAuthorizationPolicyAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(options.IntervalAuthPolicyUpdaterMinutes));
//        return Task.CompletedTask;
//    }

//    private async void UpdateAuthorizationPolicyAsync(object? state)
//    {
//        try
//        {
//            using var scope = serviceProvider.CreateScope();
//            var authPolicyService = scope.ServiceProvider.GetRequiredService<IAuthPolicyService>();
//            var result = await authPolicyService.UpdateAuthPoliciesAsync();

//            if (result)
//            {
//                logger.LogInformation("Authorization policies updated successfully.");
//            }
//            else
//            {
//                logger.LogWarning("An error occurred while generating authorization policies.");
//            }
//        }
//        catch (Exception ex)
//        {
//            logger.LogError(ex, "An exception occurred while updating authorization policies.");
//        }
//    }

//    public Task StopAsync(CancellationToken cancellationToken)
//    {
//        timer?.Change(Timeout.Infinite, 0);
//        return Task.CompletedTask;
//    }
//}