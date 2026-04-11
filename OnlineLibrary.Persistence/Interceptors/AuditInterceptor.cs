using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        var entries = eventData.Context.ChangeTracker.Entries<EntityBase>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        if (entries.Count == 0)
            return;

        var now = DateTime.UtcNow;
        var userId = currentUserProvider.GetUserId();

        foreach (var entry in entries)
        {
            ApplySoftDelete(entry);

            if (entry.Entity is AuditEntity auditEntity)
                ApplyAuditFields(entry, auditEntity, now, userId);
        }
    }

    private static void ApplySoftDelete(EntityEntry<EntityBase> entry)
    {
        if (entry.State is not EntityState.Deleted)
            return;

        entry.Entity.IsDeleted = true;
        entry.State = EntityState.Modified;
    }

    private static void ApplyAuditFields(EntityEntry<EntityBase> entry, AuditEntity auditEntity, DateTime now, int? userId)
    {
        if (entry.State is EntityState.Added)
        {
            auditEntity.CreatedAt = now;
            auditEntity.CreatedBy = userId;
            auditEntity.UpdatedAt = now;
            auditEntity.UpdatedBy = userId;
        }
        else
        {
            auditEntity.UpdatedAt = now;
            auditEntity.UpdatedBy = userId;

            entry.Property(nameof(AuditEntity.CreatedAt)).IsModified = false;
            entry.Property(nameof(AuditEntity.CreatedBy)).IsModified = false;
        }
    }
}
