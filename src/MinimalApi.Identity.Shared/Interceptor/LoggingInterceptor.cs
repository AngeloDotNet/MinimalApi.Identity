using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace MinimalApi.Identity.Shared.Interceptor;

public class LoggingInterceptor(ILogger<LoggingInterceptor> logger) : DbCommandInterceptor
{
    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        Console.WriteLine($"[SQL LOG] ReaderExecuting: {command.CommandText}");
        logger.LogInformation("[SQL LOG] ReaderExecuting: {Sql}", command.CommandText);

        return base.ReaderExecuting(command, eventData, result);
    }

    public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
    {
        Console.WriteLine($"[SQL LOG] NonQueryExecuting: {command.CommandText}");
        logger.LogInformation("[SQL LOG] NonQueryExecuting: {Sql}", command.CommandText);

        return base.NonQueryExecuting(command, eventData, result);
    }
}