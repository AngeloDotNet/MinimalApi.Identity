namespace MinimalApi.Identity.Core.Exceptions;

public class UserUnknownException : Exception
{
    public UserUnknownException()
    { }

    public UserUnknownException(string? message) : base(message)
    { }

    public UserUnknownException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
