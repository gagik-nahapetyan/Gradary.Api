using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Domain.Models;
using OnlineLibrary.Persistence.Extensions;

namespace OnlineLibrary.Persistence.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(OnlineLibraryDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default, bool includeDeleted = false, bool tracked = false)
    {
        var query = BuildQuery(includeDeleted, tracked);

        return await query
            .Include(b => b.Author)
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public override Task<PagedList<Book>> GetPagedAsync(
        int page,
        int pageSize,
        Func<IQueryable<Book>, IOrderedQueryable<Book>>? orderBy = null,
        CancellationToken cancellationToken = default,
        bool includeDeleted = false,
        bool tracked = false)
    {
        var query = BuildQuery(includeDeleted, tracked);

        return query
            .Include(b => b.Author)
            .Include(b => b.Category)
            .ToPagedListAsync(page, pageSize, orderBy, cancellationToken);
    }

    public override Task<PagedList<Book>> FindPagedAsync(
        Expression<Func<Book, bool>> predicate,
        int page,
        int pageSize,
        Func<IQueryable<Book>, IOrderedQueryable<Book>>? orderBy = null,
        CancellationToken cancellationToken = default,
        bool includeDeleted = false,
        bool tracked = false)
    {
        var query = BuildQuery(includeDeleted, tracked);

        return query
            .Include(b => b.Author)
            .Include(b => b.Category)
            .Where(predicate)
            .ToPagedListAsync(page, pageSize, orderBy, cancellationToken);
    }
}
