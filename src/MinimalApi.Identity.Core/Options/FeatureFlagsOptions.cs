namespace MinimalApi.Identity.Core.Options;

public class FeatureFlagsOptions
{
    public bool EnabledFeatureLicense { get; set; } = false;
    public bool EnabledFeatureModule { get; set; } = false;
}