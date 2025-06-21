using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MinimalApi.Identity.API.Extensions;

public static class ServicesExtensions
{
    public static TOptions AddOptionValidate<TOptions>(this IServiceCollection services, string sectionName) where TOptions : class
    {
        services.AddOptions<TOptions>().BindConfiguration(sectionName).ValidateDataAnnotations().ValidateOnStart();

        using var serviceProvider = services.BuildServiceProvider();

        return serviceProvider.GetRequiredService<IOptions<TOptions>>().Value;
    }

    public static string GetDatabaseConnString(this IConfiguration configuration, string sectionName)
    {
        return configuration.GetConnectionString(sectionName)
            ?? throw new ArgumentNullException(nameof(sectionName), "Connection string not found");
    }
}