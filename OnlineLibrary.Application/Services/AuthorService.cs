using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents an <see cref="AuthorService"/>.
/// </summary>
public class AuthorService(IAuthorRepository authorRepository) : IAuthorService
{
    public async Task<AuthorModel> CreateAsync(AuthorModel authorModel, CancellationToken cancellationToken = default)
    {
        var author = authorModel.ToEntity();
        author = await authorRepository.InsertAsync(author, cancellationToken);

        await authorRepository.SaveChangesAsync(cancellationToken);

        return author.ToModel();
    }

    public async Task UpdateAsync(AuthorModel authorModel, CancellationToken cancellationToken = default)
    {
        var existingAuthor = await authorRepository.GetByIdAsync(authorModel.Id, cancellationToken);
        if (existingAuthor is null)
            throw new KeyNotFoundException($"Author with id {authorModel.Id} not found");

        var author = authorModel.ToEntity();
        authorRepository.Update(author);

        await authorRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedList<AuthorModel>> GetAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var paged = await authorRepository.GetPagedAsync(page, pageSize, cancellationToken);

        return new PagedList<AuthorModel>
        {
            Items = paged.Items.Select(a => a.ToModel()).ToList(),
            TotalCount = paged.TotalCount,
            CurrentPage = paged.CurrentPage,
            PageSize = paged.PageSize
        };
    }

    public async Task<AuthorModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var author = await authorRepository.GetByIdAsync(id, cancellationToken);
        if (author is null)
            throw new KeyNotFoundException($"Author with id {id} not found");

        var authorModel = author.ToModel();

        return authorModel;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var author = await authorRepository.GetByIdAsync(id, cancellationToken);
        if (author is null)
            throw new KeyNotFoundException($"Author with id {id} not found");

        authorRepository.Delete(author);
        await authorRepository.SaveChangesAsync(cancellationToken);
    }
}
