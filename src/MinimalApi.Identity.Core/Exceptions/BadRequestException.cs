using Microsoft.AspNetCore.Identity;
using MinimalApi.Identity.Core.Extensions;

namespace MinimalApi.Identity.Core.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException()
    { }

    public BadRequestException(string? message) : base(message)
    { }

    public BadRequestException(string? message, Exception? innerException) : base(message, innerException)
    { }

    public BadRequestException(IEnumerable<IdentityError> errors) : base(GenericExtensions.FormatErrorMessage(errors))
    { }

    //private static string FormatErrorMessage(IEnumerable<IdentityError> errors)
    //{
    //    var sb = new StringBuilder();

    //    foreach (var error in errors)
    //    {
    //        sb.AppendLine($"{error.Code}: {error.Description}");
    //    }

    //    return sb.ToString();
    //}
}