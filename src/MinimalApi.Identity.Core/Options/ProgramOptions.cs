using MinimalApi.Identity.Core.Enums;

namespace MinimalApi.Identity.Core.Options;

/// <summary>
/// Represents configuration options for the program, including JWT, feature flags, database type, migrations assembly, and error formatting.
/// </summary>
public class ProgramOptions
{
    /// <summary>
    /// Gets or sets the options for JWT authentication.
    /// </summary>
    public JwtOptions JwtOptions { get; set; } = new JwtOptions();

    /// <summary>
    /// Gets or sets the feature flags options for enabling or disabling application features.
    /// </summary>
    public FeatureFlagsOptions FeatureFlagsOptions { get; set; } = new FeatureFlagsOptions();

    /// <summary>
    /// Gets or sets the type of database to use (e.g., "sqlserver").
    /// </summary>
    public string DatabaseType { get; set; } = "sqlserver";

    /// <summary>
    /// Gets or sets the name of the assembly containing database migrations.
    /// </summary>
    public string MigrationsAssembly { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the format for error responses.
    /// </summary>
    public ErrorResponseFormat FormatErrors { get; set; } = ErrorResponseFormat.Default;
}