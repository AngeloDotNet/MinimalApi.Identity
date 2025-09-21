using Microsoft.OpenApi.Models;
using MinimalApi.Identity.Core.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MinimalApi.Identity.Core.Filters;

public class SwaggerFeatureDocumentFilter(FeatureFlagsOptions featureFlagsOptions, Func<FeatureFlagsOptions, bool> isFeatureEnabled,
    string endpointGroup) : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (!isFeatureEnabled(featureFlagsOptions))
        {
            var toRemove = swaggerDoc.Paths.Keys
                .Where(path => path.StartsWith(endpointGroup, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var path in toRemove)
            {
                swaggerDoc.Paths.Remove(path);
            }
        }
    }
}