using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace MinimalApi.Identity.Core.Extensions;

public static class GenericExtensions
{
    public static bool HasValue([NotNullWhen(true)] this string? input)
        => input is not null && !string.IsNullOrWhiteSpace(input);

    public static bool HasValue([NotNullWhen(true)] this string? input, bool allowEmptyString)
        => input is not null && (allowEmptyString || !string.IsNullOrWhiteSpace(input));

    public static bool HasValue([NotNullWhen(true)] this string? input, bool allowEmptyString, bool whiteSpaceAsEmpty)
    {
        if (input is null)
        {
            return false;
        }

        if (allowEmptyString)
        {
            return true;
        }

        return whiteSpaceAsEmpty ? !string.IsNullOrWhiteSpace(input) : !string.IsNullOrEmpty(input);
    }

    internal static string FormatErrorMessage(IEnumerable<IdentityError> errors)
    {
        var sb = new StringBuilder();

        foreach (var error in errors)
        {
            sb.Append(error.Code);
            sb.Append(": ");
            sb.AppendLine(error.Description);
        }

        return sb.ToString();
    }
}