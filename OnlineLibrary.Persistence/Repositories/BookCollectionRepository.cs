using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Domain.Entities;

namespace OnlineLibrary.Persistence.Repositories;

/// <summary>
/// Represents the <see cref="BookCollectionRepository"/> class.
/// </summary>
public class BookCollectionRepository : Repository<BookCollection>, IBookCollectionRepository
{
    public BookCollectionRepository(OnlineLibraryDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<BookCollection>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.UserId == userId)
            .Include(c => c.Items)
            .ToListAsync(cancellationToken);
    }

    public async Task<BookCollection?> GetByIdWithItemsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
