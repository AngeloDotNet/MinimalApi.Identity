namespace MinimalApi.Identity.Core.Settings;

public class SwaggerSettings
{
    public bool IsEnabled { get; set; }
    public bool IsRequiredAuth { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
}