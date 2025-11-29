namespace MinimalApi.Identity.Core.Options;

public class CorsOptions
{
    public bool AllowAnyOrigin { get; set; }
    public bool AllowAnyMethod { get; set; }
    public bool AllowAnyHeader { get; set; }
    public string[] AllowedOrigins { get; set; } = [];
    public string[] AllowedMethods { get; set; } = [];
    public string[] AllowedHeaders { get; set; } = [];
}