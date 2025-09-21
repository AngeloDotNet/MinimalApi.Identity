using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace MinimalApi.Identity.Core.Extensions;

public static class GenericExtensions
{
    public static bool HasValue([NotNullWhen(true)] this string? input)
        => input.HasValue(allowEmptyString: false, whiteSpaceAsEmpty: true);

    public static bool HasValue([NotNullWhen(true)] this string? input, bool allowEmptyString)
        => input.HasValue(allowEmptyString, whiteSpaceAsEmpty: true);

    public static bool HasValue([NotNullWhen(true)] this string? input, bool allowEmptyString, bool whiteSpaceAsEmpty)
        => allowEmptyString ? input is not null : whiteSpaceAsEmpty ? !string.IsNullOrWhiteSpace(input) : !string.IsNullOrEmpty(input);

    internal static string FormatErrorMessage(IEnumerable<IdentityError> errors)
    {
        var sb = new StringBuilder();

        foreach (var error in errors)
        {
            sb.AppendLine($"{error.Code}: {error.Description}");
        }

        return sb.ToString();
    }
}