using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MinimalApi.Identity.Shared.Interceptor;

public class PerformanceInterceptor : DbCommandInterceptor
{
    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        var elapsed = eventData?.Duration.TotalMilliseconds ?? 0;
        Console.WriteLine($"[PERF] Query took {elapsed} ms. SQL: {Truncate(command.CommandText)}");

        return base.ReaderExecuted(command, eventData, result);
    }

    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        var elapsed = eventData?.Duration.TotalMilliseconds ?? 0;
        Console.WriteLine($"[PERF] NonQuery took {elapsed} ms. SQL: {Truncate(command.CommandText)}");

        return base.NonQueryExecuted(command, eventData, result);
    }

    private string Truncate(string s, int len = 200)
        => string.IsNullOrEmpty(s) ? s : (s.Length <= len ? s : s.Substring(0, len) + "...");
}