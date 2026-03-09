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

    public async Task<IEnumerable<Review>> GetByBookIdAsync(int bookId)
    {
        var reviews = await FindAsync(r => r.BookId == bookId);

        return reviews;
    }
}

