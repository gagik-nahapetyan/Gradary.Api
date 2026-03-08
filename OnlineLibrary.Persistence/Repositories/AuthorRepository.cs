using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Domain.Entities;

namespace OnlineLibrary.Persistence.Repositories;

public class AuthorRepository : Repository<Author>, IAuthorRepository
{
    public AuthorRepository(OnlineLibraryDbContext dbContext) : base(dbContext)
    {
    }
}

