using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineLibrary.Domain.Entities.Configurations;

/// <summary>
/// Represents a configuration class for <see cref="Category"/> entity.
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> modelBuilder)
    {
        modelBuilder
            .Property(c => c.Name)
            .HasColumnType("nvarchar(100)");

        modelBuilder
            .Property(c => c.Description)
            .HasColumnType("nvarchar(500)");

        modelBuilder
            .HasIndex(c => c.Name)
            .IsUnique();
    }
}
