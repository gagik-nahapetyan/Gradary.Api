using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineLibrary.Domain.Entities.Configurations;

public abstract class AuditEntityConfiguration<TEntity> 
    : IEntityTypeConfiguration<TEntity> where TEntity : AuditEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> modelBuilder)
    {
        modelBuilder
            .Property(p => p.CreatedAt)
            .ValueGeneratedNever()
            .Metadata
            .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        modelBuilder
            .Property(p => p.UpdatedAt)
            .ValueGeneratedNever();
    }
}
