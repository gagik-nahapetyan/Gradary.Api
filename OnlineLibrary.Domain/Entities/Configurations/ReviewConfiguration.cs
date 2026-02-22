using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineLibrary.Domain.Entities.Configurations;

/// <summary>
/// Represents a configuration class for <see cref="Review"/> entity.
/// </summary>
public class ReviewConfiguration : AuditEntityConfiguration<Review>
{
    public override void Configure(EntityTypeBuilder<Review> modelBuilder)
    {
        base.Configure(modelBuilder);

        modelBuilder
            .Property(p => p.Comment)
            .HasColumnType("nvarchar(max)");


        #region Foreign Keys

        modelBuilder
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .HasOne(r => r.Book)
            .WithMany(b => b.Reviews)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .HasIndex(r => new { r.BookId, r.UserId })
            .IsUnique();

        #endregion
    }
}
