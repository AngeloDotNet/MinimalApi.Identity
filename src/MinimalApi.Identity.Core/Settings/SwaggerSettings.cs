namespace MinimalApi.Identity.Core.Settings;

public class SwaggerSettings
{
    public bool IsEnabled { get; set; }
    public AuthSettings AuthSettings { get; set; } = new AuthSettings();
}