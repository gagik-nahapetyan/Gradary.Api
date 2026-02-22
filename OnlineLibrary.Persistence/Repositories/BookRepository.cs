using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Application.Abstractions.Repositories;

namespace OnlineLibrary.Persistence.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(OnlineLibraryDbContext dbContext) : base(dbContext)
    {
    }
}
