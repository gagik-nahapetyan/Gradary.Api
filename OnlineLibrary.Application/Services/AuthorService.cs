using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents an <see cref="AuthorService"/>.
/// </summary>
public class AuthorService(IAuthorRepository authorRepository) : IAuthorService
{
    public async Task<AuthorModel> CreateAsync(AuthorModel model)
    {
        var entity = model.ToEntity();
        entity = await authorRepository.InsertAsync(entity);

        await authorRepository.SaveChangesAsync();

        return entity.ToModel();
    }

    public async Task UpdateAsync(AuthorModel model)
    {
        var entity = model.ToEntity();
        authorRepository.Update(entity);

        await authorRepository.SaveChangesAsync();
    }

    public async Task<List<AuthorModel>> GetAsync()
    {
        var entities = await authorRepository.GetAllAsync();
        var models = entities.Select(e => e.ToModel()).ToList();

        return models;
    }

    public async Task<AuthorModel> GetByIdAsync(int id)
    {
        var entity = await authorRepository.GetByIdAsync(id);
        if (entity is null)
            throw new KeyNotFoundException($"Author with id {id} not found");

        var model = entity.ToModel();

        return model;
    }
}

