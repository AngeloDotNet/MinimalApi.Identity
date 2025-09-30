using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MinimalApi.Identity.Shared.Interceptor;

public class AuditSaveChangesInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var userName = httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "anonymous";
        Console.WriteLine($"[AUDIT] User '{userName}' is calling SaveChanges at {DateTime.UtcNow:O}");

        return base.SavingChanges(eventData, result);
    }
}