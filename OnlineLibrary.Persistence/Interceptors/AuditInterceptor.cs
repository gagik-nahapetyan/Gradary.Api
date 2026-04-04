using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OnlineLibrary.Application.Abstractions;
using OnlineLibrary.Domain.Entities;

namespace OnlineLibrary.Persistence.Interceptors;

public class AuditInterceptor(ICurrentUserProvider currentUserProvider) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateAuditFields(eventData);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditFields(DbContextEventData eventData)
    {
        if (eventData.Context is null)
            return;

        var entries = eventData.Context.ChangeTracker.Entries<AuditEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        var now = DateTime.UtcNow;
        var userId = currentUserProvider.GetUserId();

        foreach (var entry in entries)
        {
            if (entry.State is EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = userId;
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = userId;
            }
            else
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = userId;

                entry.Property(p => p.CreatedAt).IsModified = false;
                entry.Property(p => p.CreatedBy).IsModified = false;
            }
        }
    }
}
