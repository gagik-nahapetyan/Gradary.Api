using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Domain.Models;
using OnlineLibrary.Persistence.Extensions;

namespace OnlineLibrary.Persistence.Repositories;

/// <summary>
/// Represents a <see cref="Repository{TEntity}"/> class.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : EntityBase
{
    private readonly OnlineLibraryDbContext _dbContext;
    protected DbSet<TEntity> DbSet;

    public Repository(OnlineLibraryDbContext dbContext)
    {
        _dbContext = dbContext;
        DbSet = _dbContext.Set<TEntity>();
    }

    public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var entry = await DbSet.AddAsync(entity, cancellationToken);
        entity = entry.Entity;

        return entity;
    }

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default, bool includeDeleted = false)
    {
        var query = includeDeleted ? DbSet.IgnoreQueryFilters() : DbSet.AsQueryable();

        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default, bool includeDeleted = false)
    {
        var query = includeDeleted ? DbSet.IgnoreQueryFilters() : DbSet.AsQueryable();

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default, bool includeDeleted = false)
    {
        var query = includeDeleted ? DbSet.IgnoreQueryFilters() : DbSet.AsQueryable();

        return await query.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default, bool includeDeleted = false)
    {
        var query = includeDeleted ? DbSet.IgnoreQueryFilters() : DbSet.AsQueryable();

        return await query.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default, bool includeDeleted = false)
    {
        var query = includeDeleted ? DbSet.IgnoreQueryFilters() : DbSet.AsQueryable();

        return await query.CountAsync(predicate, cancellationToken);
    }

    public Task<PagedList<TEntity>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default, bool includeDeleted = false)
    {
        var query = includeDeleted ? DbSet.IgnoreQueryFilters() : DbSet.AsQueryable();
        return query.ToPagedListAsync(page, pageSize, cancellationToken);
    }

    public Task<PagedList<TEntity>> FindPagedAsync(Expression<Func<TEntity, bool>> predicate, int page, int pageSize, CancellationToken cancellationToken = default, bool includeDeleted = false)
    {
        var query = includeDeleted ? DbSet.IgnoreQueryFilters() : DbSet.AsQueryable();
        return query.Where(predicate).ToPagedListAsync(page, pageSize, cancellationToken);
    }

    public void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
