using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Domain.Entities;

namespace OnlineLibrary.Persistence.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(OnlineLibraryDbContext dbContext) : base(dbContext)
    {
    }
}

