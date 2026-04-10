using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineLibrary.Domain.Entities.Configurations;

/// <summary>
/// Represents a configuration class for the <see cref="BookCollectionItem"/> entity.
/// </summary>
public class BookCollectionItemConfiguration : IEntityTypeConfiguration<BookCollectionItem>
{
    public void Configure(EntityTypeBuilder<BookCollectionItem> modelBuilder)
    {
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
            .IsUnique();

        modelBuilder
            .HasIndex(i => new { i.BookCollectionId, i.Order });

        #endregion
    }
}
