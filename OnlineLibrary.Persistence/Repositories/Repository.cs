using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Domain.Entities;
using System.Linq.Expressions;

namespace OnlineLibrary.Persistence.Repositories;

/// <summary>
/// Represents a <see cref="Repository{TEntity}"/> class.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class Repository<TEntity> : IRepository<TEntity> 
    where TEntity : class
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

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await DbSet.Where(predicate).ToListAsync(cancellationToken);

        return entities;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await DbSet.ToListAsync(cancellationToken);

        return entities;
    }

    public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.FindAsync(id, cancellationToken);

        return entity;
    }

    public void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(predicate, cancellationToken);
    }
}
