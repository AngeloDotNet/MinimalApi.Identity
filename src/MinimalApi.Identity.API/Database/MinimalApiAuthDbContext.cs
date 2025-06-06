using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Entities.Common;

namespace MinimalApi.Identity.API.Database;

public class MinimalApiAuthDbContext(DbContextOptions<MinimalApiAuthDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole,
    int, IdentityUserClaim<int>, ApplicationUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var entityTypes = Assembly.GetExecutingAssembly()
            .GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(IEntity).IsAssignableFrom(t));

        foreach (var type in entityTypes)
        {
            builder.Entity(type);
        }

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}