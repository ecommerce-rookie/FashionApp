using Infrastructure.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Persistence.Extensions;
using Persistence.SeedWorks.Implements;
using System.Security.Claims;
using static Domain.Enums.UserEnums;

namespace ASM.Application.Infrastructure.Persistence.Interceptors;

public sealed class TrackableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _accessor;

    public TrackableEntityInterceptor(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        // Check if the context is null
        if (context is null) return;

        // Check if the context is a DbContext
        var userId = _accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return;

        // Get role of the user
        var userRole = _accessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
        if (userRole?.GetEnum<UserRole>() == null || userRole.GetEnum<UserRole>() != UserRole.Staff) return;

        foreach (var entry in context.ChangeTracker.Entries<BaseTrackableEntity<dynamic>>())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified) && !entry.HasChangedOwnedEntities())
                continue;

            if (entry.State == EntityState.Added) entry.Entity.CreatedBy = userId;

            entry.Entity.UpdatedBy = userId;
        }
    }
}
