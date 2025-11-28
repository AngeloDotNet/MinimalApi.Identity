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
    //public static void AddOpenApiInfo(this SwaggerGenOptions options)
    //{
    //    var openApiInfo = new OpenApiInfo
    //    {
    //        Title = "Minimal API Identity",
    //        Version = "v1",
    //        Contact = new OpenApiContact
    //        {
    //            Name = "Angelo Pirola",
    //            Email = "angelo@aepserver.it",
    //            Url = new Uri(ConstantsConfiguration.WebSiteDev)
    //        },
    //        License = new OpenApiLicense
    //        {
    //            Name = "License MIT",
    //            Url = new Uri(ConstantsConfiguration.LicenseMIT)
    //        },
    //    };
    //    options.SwaggerDoc("v1", openApiInfo);
    //}

    //public static void AddOpenApiSecuritySchemeRequirement(this SwaggerGenOptions options)
    //{
    //    var securityScheme = new OpenApiSecurityScheme
    //    {
    //        In = ParameterLocation.Header,
    //        Name = HeaderNames.Authorization,
    //        Description = "Insert the Bearer Token",
    //        Type = SecuritySchemeType.ApiKey,
    //        Reference = new OpenApiReference
    //        {
    //            Type = ReferenceType.SecurityScheme,
    //            Id = JwtBearerDefaults.AuthenticationScheme
    //        },
    //        Scheme = JwtBearerDefaults.AuthenticationScheme
    //    };
    //    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

    //    var securityRequirement = new OpenApiSecurityRequirement
    //    {
    //        { securityScheme, Array.Empty<string>() }
    //    };
    //}

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
        //opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "JWT Authorization header using the Bearer scheme.",
            Type = SecuritySchemeType.ApiKey,
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = JwtBearerDefaults.AuthenticationScheme
            },
            Scheme = JwtBearerDefaults.AuthenticationScheme

            //BearerFormat = "JWT",
            //Description = "JWT Authorization header using the Bearer scheme.",
            //Name = "Authorization",
            //In = ParameterLocation.Header,
            //Type = SecuritySchemeType.ApiKey,
            //Scheme = "Bearer"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        //Id = "Bearer",
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
        //options.DocumentFilter<SwaggerFeatureDocumentFilter>(featureFlagsOptions, (FeatureFlagsOptions opts)
        //    => opts.EnabledFeatureLicense, EndpointGenerator.EndpointsLicenseGroup);

        //options.DocumentFilter<SwaggerFeatureDocumentFilter>(featureFlagsOptions, (FeatureFlagsOptions opts)
        //    => opts.EnabledFeatureModule, EndpointGenerator.EndpointsModulesGroup);

        options.DocumentFilter<LicensesManagementSwaggerFilter>(featureFlagsOptions.EnabledFeatureLicense);
        options.DocumentFilter<ModulesManagementSwaggerFilter>(featureFlagsOptions.EnabledFeatureModule);
    }
}