using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Application.Helpers;
using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Domain.Enums;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents an <see cref="AuthorService"/>.
/// </summary>
public class AuthorService(
    IAuthorRepository authorRepository,
    IFileStorageService fileStorage) : IAuthorService
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
        var existingAuthor = await authorRepository.GetByIdAsync(authorModel.Id, cancellationToken, tracked: true);
        if (existingAuthor is null)
            throw new KeyNotFoundException($"Author with id {authorModel.Id} not found");

        existingAuthor.FullName = authorModel.FullName;
        existingAuthor.Biography = authorModel.Biography;

        await authorRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedList<AuthorModel>> GetAsync(int page, int pageSize, string? orderBy = null, OrderType orderType = OrderType.Asc, CancellationToken cancellationToken = default)
    {
        var paged = await authorRepository.GetPagedAsync(page, pageSize, BuildOrderBy(orderBy, orderType), cancellationToken);

        var items = new List<AuthorModel>(paged.Items.Count);
        foreach (var author in paged.Items)
            items.Add(await ToModelWithImageAsync(author, cancellationToken));

        return new PagedList<AuthorModel>
        {
            Items = items,
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

        return await ToModelWithImageAsync(author, cancellationToken);
    }

    public async Task UploadImageAsync(int id, string contentType, Func<Stream> openStream, CancellationToken cancellationToken = default)
    {
        if (!ImageContentTypes.Supported.Contains(contentType.ToLowerInvariant()))
            throw new ArgumentException($"Unsupported image content type: {contentType}");

        var authorExists = await authorRepository.ExistAsync(a => a.Id == id, cancellationToken);
        if (!authorExists)
            throw new KeyNotFoundException($"Author with id {id} not found");

        await fileStorage.DeleteByPrefixAsync($"author-images/{id}", cancellationToken);

        var ext = ImageContentTypes.GetExtension(contentType);
        using var stream = openStream();
        await fileStorage.UploadAsync($"author-images/{id}{ext}", stream, contentType, cancellationToken);
    }

    public async Task<(Stream stream, string contentType)> GetImageAsync(int id, CancellationToken cancellationToken = default)
    {
        var authorExists = await authorRepository.ExistAsync(a => a.Id == id, cancellationToken);
        if (!authorExists)
            throw new KeyNotFoundException($"Author with id {id} not found");

        var key = await fileStorage.FindKeyByPrefixAsync($"author-images/{id}", cancellationToken);
        if (key is null)
            throw new KeyNotFoundException($"No image found for author {id}");

        var result = await fileStorage.DownloadAsync(key, cancellationToken);
        return result!.Value;
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return authorRepository.ExistAsync(a => a.Id == id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var author = await authorRepository.GetByIdAsync(id, cancellationToken);
        if (author is null)
            throw new KeyNotFoundException($"Author with id {id} not found");

        authorRepository.Delete(author);
        await authorRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<AuthorModel> ToModelWithImageAsync(Author author, CancellationToken ct)
    {
        var model = author.ToModel();
        var key = await fileStorage.FindKeyByPrefixAsync($"author-images/{model.Id}", ct);
        model.ImageUrl = key is not null
            ? fileStorage.GetPublicUrl(key) ?? $"/api/authors/{model.Id}/image"
            : null;
        return model;
    }

    private static Func<IQueryable<Author>, IOrderedQueryable<Author>> BuildOrderBy(string? orderBy, OrderType orderType) =>
        orderBy?.ToLower() switch
        {
            "created" => orderType == OrderType.Desc
                ? q => q.OrderByDescending(a => a.CreatedAt)
                : q => q.OrderBy(a => a.CreatedAt),
            _ => orderType == OrderType.Desc
                ? q => q.OrderByDescending(a => a.FullName)
                : q => q.OrderBy(a => a.FullName)
        };
}
