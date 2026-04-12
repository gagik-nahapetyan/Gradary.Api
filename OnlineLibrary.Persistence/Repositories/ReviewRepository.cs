using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Persistence.Repositories;

/// <summary>
/// Represents the <see cref="ReviewRepository"/> class.
/// </summary>
public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(OnlineLibraryDbContext dbContext) : base(dbContext)
    {
    }

    public Task<PagedList<Review>> GetByBookIdPagedAsync(int bookId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return FindPagedAsync(r => r.BookId == bookId, page, pageSize, cancellationToken);
    } 
}

