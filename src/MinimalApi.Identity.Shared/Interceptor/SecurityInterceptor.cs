using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace MinimalApi.Identity.Shared.Interceptor;

public class SecurityInterceptor(ILogger<SecurityInterceptor> logger) : DbCommandInterceptor
{
    public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
    {
        var sql = (command.CommandText ?? string.Empty).TrimStart();

        // Case-insensitive check and ignore leading comments/whitespace
        if (sql.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase)
            && !sql.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase).Equals(-1) == false) // no WHERE present
        {
            // Block dangerous DELETE without WHERE
            //Console.WriteLine("[SECURITY] Blocked dangerous DELETE without WHERE."); //TODO: cleanup
            logger.LogWarning("[SECURITY] Blocked dangerous DELETE without WHERE.");
            throw new InvalidOperationException("Blocked dangerous DELETE without WHERE clause.");
        }

        return base.NonQueryExecuting(command, eventData, result);
    }
}