using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Domain.Entities;

namespace OnlineLibrary.Persistence.Repositories;

/// <summary>
/// Represents the <see cref="ReviewRepository"/> class.
/// </summary>
public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(OnlineLibraryDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Review>> GetByBookIdAsync(int bookId, CancellationToken cancellationToken = default)
    {
        var reviews = await FindAsync(r => r.BookId == bookId, cancellationToken);

        return reviews;
    }
}

