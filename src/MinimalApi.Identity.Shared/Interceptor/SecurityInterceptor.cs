using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace MinimalApi.Identity.Shared.Interceptor;

public class SecurityInterceptor(ILogger<SecurityInterceptor> logger) : DbCommandInterceptor
{
    public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(command);

        var text = command.CommandText;

        if (string.IsNullOrEmpty(text))
        {
            return base.NonQueryExecuting(command, eventData, result);
        }

        var index = SkipLeadingCommentsAndWhitespace(text);

        if (index >= text.Length)
        {
            return base.NonQueryExecuting(command, eventData, result);
        }

        if (IsDeleteStatementWithoutWhere(text, index))
        {
            logger.LogWarning("[SECURITY] Blocked dangerous DELETE without WHERE.");
            throw new InvalidOperationException("Blocked dangerous DELETE without WHERE clause.");
        }

        return base.NonQueryExecuting(command, eventData, result);
    }

    private static int SkipLeadingCommentsAndWhitespace(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var span = text.AsSpan();
        var blockEnd = "*/".AsSpan();

        var len = span.Length;
        var i = 0;

        while (i < len)
        {
            // skip whitespace
            while (i < len && char.IsWhiteSpace(span[i]))
            {
                i++;
            }

            if (i + 1 >= len)
            {
                break;
            }

            // single-line comment: -- ... (skip to end of line)
            if (span[i] == '-' && span[i + 1] == '-')
            {
                i += 2;
                while (i < len)
                {
                    var c = span[i];
                    if (c is '\n' or '\r')
                    {
                        break;
                    }

                    i++;
                }

                // continue to outer loop to skip further whitespace/comments
                continue;
            }

            // block comment: /* ... */
            if (span[i] == '/' && span[i + 1] == '*')
            {
                i += 2;
                var slice = span.Slice(i);
                var idx = slice.IndexOf(blockEnd); // ordinal search on span

                if (idx == -1)
                {
                    // unterminated block comment: treat as end of text
                    return len;
                }

                i += idx + 2;
                continue;
            }

            // no more leading comments/whitespace
            break;
        }

        return i;
    }

    private static bool IsDeleteStatementWithoutWhere(string text, int startIndex)
    {
        var span = text.AsSpan(startIndex);

        if (!span.StartsWith("DELETE".AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Use string.IndexOf with start index to avoid allocation of substring.
        return text.IndexOf("WHERE", startIndex, StringComparison.OrdinalIgnoreCase) == -1;
    }
}