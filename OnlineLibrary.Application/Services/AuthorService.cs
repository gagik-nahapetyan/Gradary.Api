using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents an <see cref="AuthorService"/>.
/// </summary>
public class AuthorService(IAuthorRepository authorRepository) : IAuthorService
{
    public async Task<AuthorModel> CreateAsync(AuthorModel authorModel)
    {
        var author = authorModel.ToEntity();
        author = await authorRepository.InsertAsync(author);

        await authorRepository.SaveChangesAsync();

        return author.ToModel();
    }

    public async Task UpdateAsync(AuthorModel authorModel)
    {
        var existingAuthor = await authorRepository.GetByIdAsync(authorModel.Id);
        if (existingAuthor is null)
            throw new KeyNotFoundException($"Author with id {authorModel.Id} not found");

        var author = authorModel.ToEntity();
        authorRepository.Update(author);

        await authorRepository.SaveChangesAsync();
    }

    public async Task<List<AuthorModel>> GetAsync()
    {
        var authors = await authorRepository.GetAllAsync();
        var authorModels = authors.Select(a => a.ToModel()).ToList();

        return authorModels;
    }

    public async Task<AuthorModel> GetByIdAsync(int id)
    {
        var author = await authorRepository.GetByIdAsync(id);
        if (author is null)
            throw new KeyNotFoundException($"Author with id {id} not found");

        var authorModel = author.ToModel();

        return authorModel;
    }
}
