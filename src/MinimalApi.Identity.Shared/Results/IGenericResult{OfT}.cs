using System.Diagnostics.CodeAnalysis;

namespace MinimalApi.Identity.Shared.Results;

public interface IGenericResult<T> : IGenericResult
{
    public T? Content { get; }
    public bool TryGetContent([NotNullWhen(returnValue: true)] out T? content);
}