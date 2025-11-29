using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MinimalApi.Identity.Core.Configurations;
using MinimalApi.Identity.Core.Filters;
using MinimalApi.Identity.Core.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MinimalApi.Identity.API.Extensions;

public static class SwaggerExtensions
{
    public static void AddSwaggerGenOptions(this SwaggerGenOptions options, FeatureFlagsOptions featureFlagsOptions)
    {
        var openApiInfo = new OpenApiInfo
        {
            Title = "Minimal API Identity",
            Version = "v1",
            Contact = new OpenApiContact
            {
                Name = "Angelo Pirola",
                Email = "angelo@aepserver.it"
            },
            License = new OpenApiLicense
            {
                Name = "License MIT",
                Url = new Uri(ConstantsConfiguration.LicenseMIT)
            },
        };

        options.SwaggerDoc("v1", openApiInfo);
        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            //BearerFormat = "JWT",
            Description = "JWT Authorization header using the Bearer scheme.",
            Type = SecuritySchemeType.ApiKey,
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = JwtBearerDefaults.AuthenticationScheme
            },
            Scheme = JwtBearerDefaults.AuthenticationScheme
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                },
                Array.Empty<string>()
            }
        });
        options.AddSwaggerDocumentFilters(featureFlagsOptions);
    }

    public static void AddSwaggerDocumentFilters(this SwaggerGenOptions options, FeatureFlagsOptions featureFlagsOptions)
    {
        options.DocumentFilter<LicensesManagementSwaggerFilter>(featureFlagsOptions.EnabledFeatureLicense);
        options.DocumentFilter<ModulesManagementSwaggerFilter>(featureFlagsOptions.EnabledFeatureModule);
    }
}