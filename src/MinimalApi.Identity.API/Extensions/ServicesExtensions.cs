using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MinimalApi.Identity.API.Authorization.Handlers;
using MinimalApi.Identity.API.HostedServices;
using MinimalApi.Identity.Core.Entities;

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

    internal static IServiceCollection AddServicesToDependencyInjection(this IServiceCollection services)
    {
        return services.AddSingleton<IHostedService, AuthorizationPolicyGeneration>()
            .AddScoped<SignInManager<ApplicationUser>>()
            .AddScoped<IAuthorizationHandler, PermissionHandler>()
            .AddHostedService<AuthorizationPolicyUpdater>();
    }

    //public static IServiceCollection AddRegisterService<TAssembly>(this IServiceCollection services, string stringEndsWith,
    //    ServiceLifetime lifetime) where TAssembly : class
    //{
    //    return services.Scan(scan => scan.FromAssemblyOf<TAssembly>().AddClasses(classes
    //        => classes.Where(type => type.Name.EndsWith(stringEndsWith))).AsImplementedInterfaces().WithLifetime(lifetime));
    //}

    //public static IServiceCollection AddRegisterTransientService<TAssembly>(this IServiceCollection services, string stringEndsWith)
    //    where TAssembly : class => services.AddRegisterService<TAssembly>(stringEndsWith, ServiceLifetime.Transient);

    //public static IServiceCollection AddRegisterScopedService<TAssembly>(this IServiceCollection services, string stringEndsWith)
    //    where TAssembly : class => services.AddRegisterService<TAssembly>(stringEndsWith, ServiceLifetime.Scoped);

    //public static IServiceCollection AddRegisterSingletonService<TAssembly>(this IServiceCollection services, string stringEndsWith)
    //    where TAssembly : class => services.AddRegisterService<TAssembly>(stringEndsWith, ServiceLifetime.Singleton);

    public static IServiceCollection AddRegisterTransientService(this IServiceCollection services, List<Type> interfaces, string stringEndsWith)
    {
        var assembly = FindImplementationInterfaces(services, interfaces.FirstOrDefault()
            ?? throw new ArgumentNullException(nameof(interfaces), "No interfaces provided"));

        return services.Scan(scan => scan.FromAssemblies(assembly)
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith(stringEndsWith)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());
    }

    public static IServiceCollection AddRegisterScopedService(this IServiceCollection services, List<Type> interfaces, string stringEndsWith)
    {
        var assembly = FindImplementationInterfaces(services, interfaces.FirstOrDefault()
            ?? throw new ArgumentNullException(nameof(interfaces), "No interfaces provided"));

        return services.Scan(scan => scan.FromAssemblies(assembly)
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith(stringEndsWith)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }

    public static IServiceCollection AddRegisterSingletonService(this IServiceCollection services, List<Type> interfaces, string stringEndsWith)
    {
        var assembly = FindImplementationInterfaces(services, interfaces.FirstOrDefault()
            ?? throw new ArgumentNullException(nameof(interfaces), "No interfaces provided"));

        return services.Scan(scan => scan.FromAssemblies(assembly)
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith(stringEndsWith)))
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }

    internal static Assembly FindImplementationInterfaces(this IServiceCollection services, Type interfaceType)
    {
        var implementationType = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => interfaceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
            ?? throw new InvalidOperationException($"No implementation found for interface {interfaceType.FullName}");

        return interfaceType.Assembly;
    }
}