using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Domain.Entities.Configurations;

namespace OnlineLibrary.Persistence;

public class OnlineLibraryDbContext(DbContextOptions<OnlineLibraryDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookConfiguration).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(EntityBase).IsAssignableFrom(entityType.ClrType))
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var property = Expression.Property(parameter, nameof(EntityBase.IsDeleted));
            var filter = Expression.Lambda(Expression.Not(property), parameter);
            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
        }
    }
}
