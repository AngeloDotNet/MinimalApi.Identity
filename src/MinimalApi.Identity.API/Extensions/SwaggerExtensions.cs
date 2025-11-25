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

    //public static void AddSwaggerDocumentFilters(this SwaggerGenOptions options, FeatureFlagsOptions featureFlagsOptions)
    //{
    //    options.DocumentFilter<SwaggerFeatureDocumentFilter>(featureFlagsOptions, (FeatureFlagsOptions opts)
    //        => opts.EnabledFeatureLicense, EndpointGenerator.EndpointsLicenseGroup);

    //    options.DocumentFilter<SwaggerFeatureDocumentFilter>(featureFlagsOptions, (FeatureFlagsOptions opts)
    //        => opts.EnabledFeatureModule, EndpointGenerator.EndpointsModulesGroup);
    //}
}