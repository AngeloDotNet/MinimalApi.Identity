using Microsoft.Extensions.Configuration;
using MinimalApi.Identity.API.Options;

namespace MinimalApi.Identity.API.Extensions;

public static class ManageOptionsExtensions
{
    private static FeatureFlagsOptions featureFlagsOptions;

    //TODO: Uncomment and implement JwtOptions if needed
    //private static JwtOptions jwtOptions;

    // TODO: Uncomment and implement ValidationOptions if needed
    //private static ValidationOptions validationOptions;

    public static FeatureFlagsOptions GetFeatureFlagsOptions(IConfiguration configuration)
    {
        var configOptions = configuration.GetSection("FeatureFlagsOptions").Get<FeatureFlagsOptions>();

        if (featureFlagsOptions == null)
        {
            featureFlagsOptions = configOptions ?? new FeatureFlagsOptions
            {
                EnabledFeatureLicense = false,
                EnabledFeatureModule = false
            };
        }
        else if (configOptions != null)
        {
            featureFlagsOptions.EnabledFeatureLicense = configOptions.EnabledFeatureLicense;
            featureFlagsOptions.EnabledFeatureModule = configOptions.EnabledFeatureModule;
        }

        return featureFlagsOptions;
    }
}