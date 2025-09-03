using MinimalApi.Identity.Core.Enums;

namespace MinimalApi.Identity.API.Options;

public class ProgramOptions
{
    public JwtOptions JwtOptions { get; set; } = new JwtOptions();
    public FeatureFlagsOptions FeatureFlagsOptions { get; set; } = new FeatureFlagsOptions();
    public string DatabaseType { get; set; } = "sqlserver";
    public string MigrationsAssembly { get; set; } = string.Empty;
    public ErrorResponseFormat FormatErrors { get; set; } = ErrorResponseFormat.Default;
}