using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace MinimalApi.Identity.Shared.Interceptor;

public class AuditSaveChangesInterceptor(IHttpContextAccessor httpContextAccessor, ILogger<AuditSaveChangesInterceptor> logger) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var userName = httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "anonymous";

        Console.WriteLine($"[AUDIT] User '{userName}' is calling SaveChanges at {DateTime.UtcNow:O}");
        logger.LogInformation("[AUDIT] User '{UserName}' is calling SaveChanges at {Time}", userName, DateTime.UtcNow.ToString("O"));

        return base.SavingChanges(eventData, result);
    }
}