using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineLibrary.Domain.Entities.Configurations;

/// <summary>
/// Represents a configuration class for <see cref="User"/> entity.
/// </summary>
public class UserConfiguration : AuditEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> modelBuilder)
    {
        base.Configure(modelBuilder);

        modelBuilder
            .Property(p => p.FullName)
            .HasColumnType("nvarchar(100)");

        modelBuilder
            .Property(p => p.Email)
            .HasColumnType("varchar(100)");

        modelBuilder
            .Property(p => p.PasswordHash)
            .HasColumnType("varchar(256)");

        #region Indices

        modelBuilder
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        #endregion
    }
}
