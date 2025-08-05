using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Identity.API.Options;

public class FeatureFlagsOptions
{
    [Required]
    public bool EnabledFeatureLicense { get; set; } = false;

    [Required]
    public bool EnabledFeatureModule { get; set; } = false;
}