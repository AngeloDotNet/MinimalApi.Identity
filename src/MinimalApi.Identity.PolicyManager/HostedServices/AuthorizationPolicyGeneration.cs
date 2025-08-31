namespace MinimalApi.Identity.PolicyManager.HostedServices;

//public class AuthorizationPolicyGeneration(IServiceProvider serviceProvider, ILogger<AuthorizationPolicyGeneration> logger) : IHostedService
//{
//    public async Task StartAsync(CancellationToken cancellationToken)
//    {
//        using var scope = serviceProvider.CreateAsyncScope();

//        var authPolicyService = scope.ServiceProvider.GetRequiredService<IAuthPolicyService>();
//        var result = await authPolicyService.GenerateAuthPoliciesAsync();

//        if (result)
//        {
//            logger.LogInformation("Authorization policies generated successfully.");
//        }
//        else
//        {
//            logger.LogWarning("An error occurred while generating authorization policies.");
//        }
//    }

//    public Task StopAsync(CancellationToken cancellationToken)
//    {
//        return Task.CompletedTask;
//    }
//}