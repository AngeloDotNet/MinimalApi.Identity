namespace MinimalApi.Identity.Core.Exceptions;

public class UserTokenIsInvalidException : Exception
{
    public UserTokenIsInvalidException()
    { }

    public UserTokenIsInvalidException(string? message) : base(message)
    { }

    public UserTokenIsInvalidException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
