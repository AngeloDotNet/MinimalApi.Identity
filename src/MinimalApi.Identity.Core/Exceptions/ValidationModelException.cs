namespace MinimalApi.Identity.Core.Exceptions;

public class ValidationModelException(string message, IDictionary<string, string[]> errors) : Exception(message)
{
    public IDictionary<string, string[]> Errors { get; } = errors;
}