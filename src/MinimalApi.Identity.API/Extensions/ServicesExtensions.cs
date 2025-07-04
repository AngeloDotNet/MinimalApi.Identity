using Microsoft.Extensions.Configuration;

namespace MinimalApi.Identity.API.Extensions;

public static class ServicesExtensions
{
    public static string GetDatabaseConnString(this IConfiguration configuration, string sectionName)
    {
        return configuration.GetConnectionString(sectionName)
            ?? throw new ArgumentNullException(nameof(sectionName), "Connection string not found");
    }
}