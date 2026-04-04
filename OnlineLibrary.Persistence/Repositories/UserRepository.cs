using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Domain.Entities;

namespace OnlineLibrary.Persistence.Repositories;

public class UserRepository(OnlineLibraryDbContext dbContext) : Repository<User>(dbContext), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var results = await FindAsync(u => u.Email == email, cancellationToken);
        return results.FirstOrDefault();
    }
}
