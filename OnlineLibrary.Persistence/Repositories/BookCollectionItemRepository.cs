using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Domain.Entities;

namespace OnlineLibrary.Persistence.Repositories;

/// <summary>
/// Represents the <see cref="BookCollectionItemRepository"/> class.
/// </summary>
public class BookCollectionItemRepository : Repository<BookCollectionItem>, IBookCollectionItemRepository
{
    public BookCollectionItemRepository(OnlineLibraryDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<BookCollectionItem?> GetByCollectionAndBookAsync(int collectionId, int bookId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(i => i.BookCollectionId == collectionId && i.BookId == bookId, cancellationToken);
    }
}
