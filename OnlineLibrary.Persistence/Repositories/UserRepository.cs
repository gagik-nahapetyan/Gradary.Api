using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Domain.Entities;

namespace OnlineLibrary.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(OnlineLibraryDbContext dbContext) : base(dbContext)
    {
    }
}

