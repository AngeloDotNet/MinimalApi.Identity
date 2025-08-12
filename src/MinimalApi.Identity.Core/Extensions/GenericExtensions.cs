using System.Text;
using Microsoft.AspNetCore.Identity;

namespace MinimalApi.Identity.Core.Extensions;

public static class GenericExtensions
{
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