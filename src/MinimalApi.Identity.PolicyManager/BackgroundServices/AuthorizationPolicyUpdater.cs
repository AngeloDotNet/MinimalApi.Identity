namespace MinimalApi.Identity.PolicyManager.BackgroundServices;

//public class AuthorizationPolicyUpdater(IServiceScopeFactory serviceScopeFactory, ILogger<AuthorizationPolicyUpdater> logger,
//    IOptions<HostedServiceOptions> hostedOptions) : BackgroundService
//{
//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        ArgumentNullException.ThrowIfNull(serviceScopeFactory);
//        ArgumentNullException.ThrowIfNull(logger);
//        ArgumentNullException.ThrowIfNull(hostedOptions);

//        try
//        {
//            var timer = new System.Timers.Timer
//            {
//                Interval = TimeSpan.FromMinutes(hostedOptions.Value.IntervalAuthPolicyUpdaterMinutes).TotalMilliseconds
//            };

//            timer.Elapsed += Timer_ElapsedAsync;
//            timer.Start();

//            await Task.Delay(Timeout.Infinite, stoppingToken);
//        }
//        catch (Exception ex)
//        {
//            logger.LogError(ex, "An exception occurred in ExecuteAsync of {ServiceName}.", nameof(AuthorizationPolicyUpdater));
//        }
//    }

//    private async void Timer_ElapsedAsync(object? sender, ElapsedEventArgs e)
//    {
//        using var scope = serviceScopeFactory.CreateScope();

//        var authPolicyService = scope.ServiceProvider.GetRequiredService<IAuthPolicyService>();

//        try
//        {
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
//}