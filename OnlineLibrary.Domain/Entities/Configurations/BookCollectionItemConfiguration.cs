using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineLibrary.Domain.Entities.Configurations;

/// <summary>
/// Represents a configuration class for the <see cref="BookCollectionItem"/> entity.
/// </summary>
public class BookCollectionItemConfiguration : AuditEntityConfiguration<BookCollectionItem>
{
    public override void Configure(EntityTypeBuilder<BookCollectionItem> modelBuilder)
    {
        base.Configure(modelBuilder);

        #region Foreign Keys

        modelBuilder
            .HasOne(i => i.BookCollection)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.BookCollectionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .HasOne(i => i.Book)
            .WithMany()
            .HasForeignKey(i => i.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        #endregion

        #region Indices

        modelBuilder
            .HasIndex(i => new { i.BookCollectionId, i.BookId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder
            .HasIndex(i => new { i.BookCollectionId, i.Position });

        #endregion
    }
}
