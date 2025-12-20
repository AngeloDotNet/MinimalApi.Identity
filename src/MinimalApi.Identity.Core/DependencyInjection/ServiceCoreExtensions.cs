using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.Enums;
using MinimalApi.Identity.Core.Filters;
using MinimalApi.Identity.Core.Settings;

namespace MinimalApi.Identity.Core.DependencyInjection;

public static class ServiceCoreExtensions
{
    public const string Permission = nameof(ClaimsType.Permission);

    public static RouteHandlerBuilder ProducesDefaultProblem(this RouteHandlerBuilder builder, params int[] statusCodes)
    {
        foreach (var statusCode in statusCodes)
        {
            builder.ProducesProblem(statusCode);
        }

        return builder;
    }

    public static OpenApiResponse Response(this OpenApiOperation operation, int statusCode)
        => operation.Responses!.GetByStatusCode(statusCode);

    public static OpenApiResponse GetByStatusCode(this OpenApiResponses responses, int statusCode)
        => responses.Single(r => r.Key == statusCode.ToString()).Value;

    public static RouteHandlerBuilder WithValidation<TModel>(this RouteHandlerBuilder builder) where TModel : class
        => builder.AddEndpointFilter<ValidatorFilter<TModel>>().ProducesValidationProblem();

    public static IServiceCollection ConfigureFluentValidation<TValidator>(this IServiceCollection services) where TValidator : IValidator
        => services.AddValidatorsFromAssembly(typeof(TValidator).Assembly);

    public static IServiceCollection ConfigureValidation(this IServiceCollection services, Action<AppSettings> configureOptions)
        => services.Configure(configureOptions);

    public static IServiceCollection AddRegisterServices(this IServiceCollection services, Action<ServiceRegistrationConfiguration> configure)
    {
        var configuration = new ServiceRegistrationConfiguration(services);
        configure.Invoke(configuration);

        foreach (var interfaceType in configuration.Interfaces)
        {
            var assembly = FindImplementationInterfaces(interfaceType);

            services.Scan(scan =>
            {
                var classSelector = scan.FromAssemblies(assembly).AddClasses(classes
                    => classes.Where(type => type.Name.EndsWith(configuration.StringEndsWith))).AsImplementedInterfaces();

                if (configuration.Lifetime is ServiceLifetime.Singleton)
                {
                    classSelector.WithSingletonLifetime();
                }
                else if (configuration.Lifetime is ServiceLifetime.Scoped)
                {
                    classSelector.WithScopedLifetime();
                }
                else
                {
                    classSelector.WithTransientLifetime();
                }
            });
        }

        return services;
    }

    internal static Assembly FindImplementationInterfaces(Type interfaceType)
    {
        ArgumentNullException.ThrowIfNull(interfaceType);

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && interfaceType.IsAssignableFrom(type))
                {
                    return type.Assembly;
                }
            }
        }

        throw new InvalidOperationException($"No implementation found for interface {interfaceType.FullName}");
    }
}