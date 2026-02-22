using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents a <see cref="BookService"/>.
/// </summary>
public class BookService(IBookRepository bookRepository) : IBookService
{
    public async Task<BookModel> CreateAsync(BookModel model)
    {
        var entity = model.ToEntity();
        entity = await bookRepository.InsertAsync(entity);

        await bookRepository.SaveChangesAsync();

        return entity.ToModel();
    }

    public async Task UpdateAsync(BookModel model)
    {
        var entity = model.ToEntity();
        bookRepository.Update(entity);

        await bookRepository.SaveChangesAsync();
    }

    public async Task<List<BookModel>> GetAsync()
    {
        var entities = await bookRepository.GetAllAsync();
        var models = entities.Select(e => e.ToModel()).ToList();
        
        return models;
    }

    public async Task<BookModel> GetByIdAsync(int id)
    {
        var entity = await bookRepository.GetByIdAsync(id);
        if (entity is null)
            throw new KeyNotFoundException($"Book with id {id} not found");

        var model = entity.ToModel();

        return model;
    }
}
