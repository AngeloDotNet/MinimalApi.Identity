using System.ComponentModel.DataAnnotations;
using MinimalApi.Identity.Core.Enums;

namespace MinimalApi.Identity.Core.Options;

public class ValidationOptions
{
    [Required]
    public int MinLength { get; init; }

    [Required]
    public int MaxLength { get; init; }

    [Required]
    public int MinLengthDescription { get; init; }

    [Required]
    public int MaxLengthDescription { get; init; }

    [Required]
    public ErrorResponseFormat ErrorResponseFormat { get; set; }
}