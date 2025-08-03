using Microsoft.OpenApi.Models;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.Core.Utility.Generators;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MinimalApi.Identity.API.Swagger;

public class SwaggerModulesDocumentFilter(FeatureFlagsOptions featureFlagsOptions) : IDocumentFilter
{
    //public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    //{
    //    if (!featureFlagsOptions.EnabledFeatureLicense)
    //    {
    //        var licensePaths = swaggerDoc.Paths
    //            .Where(p => p.Key.StartsWith(EndpointGenerator.EndpointsLicenseGroup, StringComparison.OrdinalIgnoreCase))
    //            .Select(p => p.Key)
    //            .ToList();

    //        foreach (var path in licensePaths)
    //        {
    //            swaggerDoc.Paths.Remove(path);
    //        }
    //    }

    //    if (!featureFlagsOptions.EnabledFeatureModule)
    //    {
    //        var modulePaths = swaggerDoc.Paths
    //            .Where(p => p.Key.StartsWith(EndpointGenerator.EndpointsModulesGroup, StringComparison.OrdinalIgnoreCase))
    //            .Select(p => p.Key)
    //            .ToList();
    //        foreach (var path in modulePaths)
    //        {
    //            swaggerDoc.Paths.Remove(path);
    //        }
    //    }
    //}

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (!featureFlagsOptions.EnabledFeatureLicense)
        {
            var toRemove = new List<string>();
            foreach (var path in swaggerDoc.Paths.Keys)
            {
                if (path.StartsWith(EndpointGenerator.EndpointsLicenseGroup, StringComparison.OrdinalIgnoreCase))
                {
                    toRemove.Add(path);
                }
            }
            foreach (var path in toRemove)
            {
                swaggerDoc.Paths.Remove(path);
            }
        }

        if (!featureFlagsOptions.EnabledFeatureModule)
        {
            var toRemove = new List<string>();
            foreach (var path in swaggerDoc.Paths.Keys)
            {
                if (path.StartsWith(EndpointGenerator.EndpointsModulesGroup, StringComparison.OrdinalIgnoreCase))
                {
                    toRemove.Add(path);
                }
            }
            foreach (var path in toRemove)
            {
                swaggerDoc.Paths.Remove(path);
            }
        }
    }
}