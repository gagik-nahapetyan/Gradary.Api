using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineLibrary.Domain.Entities.Configurations;

/// <summary>
/// Represents a configuration class for <see cref="Author"/> entity.
/// </summary>
public class AuthorConfiguration : AuditEntityConfiguration<Author>
{
    public override void Configure(EntityTypeBuilder<Author> modelBuilder)
    {
        base.Configure(modelBuilder);

        modelBuilder
            .Property(a => a.FullName)
            .HasColumnType("nvarchar(200)");

        modelBuilder
            .Property(a => a.Biography)
            .HasColumnType("nvarchar(max)");

        #region Indices

        modelBuilder
            .HasIndex(u => u.FullName);

        #endregion
    }
}
