using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineLibrary.Domain.Entities.Configurations;

/// <summary>
/// Represents a configuration class for <see cref="Book"/> entity.
/// </summary>
public class BookConfiguration : AuditEntityConfiguration<Book>
{
    public override void Configure(EntityTypeBuilder<Book> modelBuilder)
    {
        base.Configure(modelBuilder);

        modelBuilder
            .Property(p => p.ShortTitle)
            .HasColumnType("nvarchar(127)");

        modelBuilder
            .Property(p => p.FullTitle)
            .HasColumnType("nvarchar(255)");

        modelBuilder
            .Property(p => p.Description)
            .HasColumnType("nvarchar(max)");

        #region Foreign Keys

        modelBuilder
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        #endregion

        #region Indices

        modelBuilder
            .HasIndex(u => u.ShortTitle);

        #endregion

    }
}
