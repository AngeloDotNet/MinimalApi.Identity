using Microsoft.OpenApi.Models;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.Core.Utility.Generators;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MinimalApi.Identity.API.Swagger;

public class SwaggerModulesDocumentFilter(FeatureFlagsOptions featureFlagsOptions) : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (!featureFlagsOptions.EnabledFeatureModule)
        {
            var toRemove = swaggerDoc.Paths.Keys
                .Where(path => path.StartsWith(EndpointGenerator.EndpointsModulesGroup, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var path in toRemove)
            {
                swaggerDoc.Paths.Remove(path);
            }
        }
    }
}