using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MinimalApi.Identity.Shared.Interceptor;

public class LoggingInterceptor : DbCommandInterceptor
{
    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        Console.WriteLine($"[SQL LOG] ReaderExecuting: {command.CommandText}");
        return base.ReaderExecuting(command, eventData, result);
    }

    public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
    {
        Console.WriteLine($"[SQL LOG] NonQueryExecuting: {command.CommandText}");
        return base.NonQueryExecuting(command, eventData, result);
    }
}