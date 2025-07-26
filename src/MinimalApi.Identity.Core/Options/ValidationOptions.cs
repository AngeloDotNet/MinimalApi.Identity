using MinimalApi.Identity.Core.Enums;

namespace MinimalApi.Identity.Core.Options;

public class ValidationOptions
{
    public int MinLength { get; init; }
    public int MaxLength { get; init; }
    public int MinLengthDescription { get; init; }
    public int MaxLengthDescription { get; init; }
    public ErrorResponseFormat ErrorResponseFormat { get; set; }
}