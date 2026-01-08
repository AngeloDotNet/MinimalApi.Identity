using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace MinimalApi.Identity.Shared.Interceptor;

public class PerformanceInterceptor(ILogger<PerformanceInterceptor> logger) : DbCommandInterceptor
{
    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        var elapsed = eventData?.Duration.TotalMilliseconds ?? 0;
        Console.WriteLine($"[PERF] Query took {elapsed} ms. SQL: {Truncate(command.CommandText)}");
        logger.LogInformation("[PERF] Query took {Elapsed} ms. SQL: {Sql}", elapsed, Truncate(command.CommandText));

        return base.ReaderExecuted(command, eventData, result);
    }

    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        var elapsed = eventData?.Duration.TotalMilliseconds ?? 0;
        Console.WriteLine($"[PERF] NonQuery took {elapsed} ms. SQL: {Truncate(command.CommandText)}");
        logger.LogInformation("[PERF] NonQuery took {Elapsed} ms. SQL: {Sql}", elapsed, Truncate(command.CommandText));

        return base.NonQueryExecuted(command, eventData, result);
    }

    private static string Truncate(string s, int len = 200)
    {
        ArgumentNullException.ThrowIfNull(s);

        if (s.Length <= len)
        {
            return s;
        }

        return s[..len] + "...";
    }
}