using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineLibrary.Domain.Entities.Configurations;

/// <summary>
/// Represents a configuration class for the <see cref="BookCollection"/> entity.
/// </summary>
public class BookCollectionConfiguration : AuditEntityConfiguration<BookCollection>
{
    public override void Configure(EntityTypeBuilder<BookCollection> modelBuilder)
    {
        base.Configure(modelBuilder);

        modelBuilder
            .Property(p => p.Name)
            .HasColumnType("nvarchar(127)");

        modelBuilder
            .Property(p => p.Description)
            .HasColumnType("nvarchar(max)");

        #region Foreign Keys

        modelBuilder
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #region Indices

        modelBuilder
            .HasIndex(c => new { c.UserId, c.Name })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        #endregion
    }
}
