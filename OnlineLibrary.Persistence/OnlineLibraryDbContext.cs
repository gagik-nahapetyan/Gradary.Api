using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Domain.Entities.Configurations;

namespace OnlineLibrary.Persistence;

public class OnlineLibraryDbContext(DbContextOptions<OnlineLibraryDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookConfiguration).Assembly);
    }
}
