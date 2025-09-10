namespace MinimalApi.Identity.API.Options;

/// <summary>
/// Represents feature flag configuration options for enabling or disabling specific application features.
/// </summary>
/// <remarks>
/// Use this class to control the availability of features at runtime.
/// </remarks>
public class FeatureFlagsOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the license feature is enabled.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the license feature is enabled; otherwise, <see langword="false"/>.
    /// </value>
    public bool EnabledFeatureLicense { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the module feature is enabled.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the module feature is enabled; otherwise, <see langword="false"/>.
    /// </value>
    public bool EnabledFeatureModule { get; set; } = false;
}