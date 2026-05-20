using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Application.Helpers;
using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Domain.Enums;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents a <see cref="BookService"/>.
/// </summary>
public class BookService(
    IBookRepository bookRepository,
    ICategoryRepository categoryRepository,
    IAuthorRepository authorRepository,
    IFileStorageService fileStorage) : IBookService
{
    public async Task<BookModel> CreateAsync(BookModel bookModel, CancellationToken cancellationToken = default)
    {
        var bookWithTitleExists = await bookRepository.ExistAsync(b => b.Title.ToLower() == bookModel.Title.ToLower(), cancellationToken);
        if (bookWithTitleExists)
            throw new ArgumentException($"Book with title {bookModel.Title} already exists");

        var book = bookModel.ToEntity();
        book = await bookRepository.InsertAsync(book, cancellationToken);

        await bookRepository.SaveChangesAsync(cancellationToken);

        return book.ToModel();
    }

    public async Task UpdateAsync(BookModel bookModel, CancellationToken cancellationToken = default)
    {
        var bookExists = await bookRepository.ExistAsync(b => b.Id == bookModel.Id, cancellationToken);
        if (!bookExists)
            throw new KeyNotFoundException($"Book with id {bookModel.Id} not found");

        var bookWithSameTitleExists = await bookRepository.ExistAsync(b => b.Id != bookModel.Id && b.Title.ToLower() == bookModel.Title.ToLower(), cancellationToken);
        if (bookWithSameTitleExists)
            throw new ArgumentException($"Book with title {bookModel.Title} already exists");

        var book = bookModel.ToEntity();
        bookRepository.Update(book);

        await bookRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task UploadFileAsync(int id, Func<Stream> openStream, CancellationToken cancellationToken = default)
    {
        var bookExists = await bookRepository.ExistAsync(b => b.Id == id, cancellationToken);
        if (!bookExists)
            throw new KeyNotFoundException($"Book with id {id} not found");

        await fileStorage.DeleteByPrefixAsync($"book-files/{id}", cancellationToken);

        using var stream = openStream();
        await fileStorage.UploadAsync($"book-files/{id}", stream, "application/octet-stream", cancellationToken);
    }

    public async Task UploadImageAsync(int id, string contentType, Func<Stream> openStream, CancellationToken cancellationToken = default)
    {
        if (!ImageContentTypes.Supported.Contains(contentType.ToLowerInvariant()))
            throw new ArgumentException($"Unsupported image content type: {contentType}");

        var bookExists = await bookRepository.ExistAsync(b => b.Id == id, cancellationToken);
        if (!bookExists)
            throw new KeyNotFoundException($"Book with id {id} not found");

        await fileStorage.DeleteByPrefixAsync($"book-covers/{id}", cancellationToken);

        var ext = ImageContentTypes.GetExtension(contentType);
        using var stream = openStream();
        await fileStorage.UploadAsync($"book-covers/{id}{ext}", stream, contentType, cancellationToken);
    }

    public async Task<(Stream stream, string contentType)> GetImageAsync(int id, CancellationToken cancellationToken = default)
    {
        var bookExists = await bookRepository.ExistAsync(b => b.Id == id, cancellationToken);
        if (!bookExists)
            throw new KeyNotFoundException($"Book with id {id} not found");

        var key = await fileStorage.FindKeyByPrefixAsync($"book-covers/{id}", cancellationToken);
        if (key is null)
            throw new KeyNotFoundException($"No image found for book {id}");

        var result = await fileStorage.DownloadAsync(key, cancellationToken);
        return result!.Value;
    }

    public async Task<BookModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var book = await bookRepository.GetByIdAsync(id, cancellationToken);
        if (book is null)
            throw new KeyNotFoundException($"Book with id {id} not found");

        return await ToModelWithImageAsync(book, cancellationToken);
    }

    public async Task<PagedList<BookModel>> GetAsync(int page, int pageSize, string? orderBy = null, OrderType orderType = OrderType.Asc, CancellationToken cancellationToken = default)
    {
        var paged = await bookRepository.GetPagedAsync(page, pageSize, BuildOrderBy(orderBy, orderType), cancellationToken);

        var items = new List<BookModel>(paged.Items.Count);
        foreach (var book in paged.Items)
            items.Add(await ToModelWithImageAsync(book, cancellationToken));

        return new PagedList<BookModel>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            CurrentPage = paged.CurrentPage,
            PageSize = paged.PageSize
        };
    }

    public async Task<PagedList<BookModel>> GetByCategoryIdAsync(int categoryId, int page, int pageSize, string? orderBy = null, OrderType orderType = OrderType.Asc, CancellationToken cancellationToken = default)
    {
        var categoryExists = await categoryRepository.ExistAsync(c => c.Id == categoryId, cancellationToken);
        if (!categoryExists)
            throw new KeyNotFoundException($"Category with id {categoryId} not found");

        var paged = await bookRepository.FindPagedAsync(b => b.CategoryId == categoryId, page, pageSize, BuildOrderBy(orderBy, orderType), cancellationToken);

        var items = new List<BookModel>(paged.Items.Count);
        foreach (var book in paged.Items)
            items.Add(await ToModelWithImageAsync(book, cancellationToken));

        return new PagedList<BookModel>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            CurrentPage = paged.CurrentPage,
            PageSize = paged.PageSize
        };
    }

    public async Task<PagedList<BookModel>> GetByAuthorIdAsync(int authorId, int page, int pageSize, string? orderBy = null, OrderType orderType = OrderType.Asc, CancellationToken cancellationToken = default)
    {
        var authorExists = await authorRepository.ExistAsync(a => a.Id == authorId, cancellationToken);
        if (!authorExists)
            throw new KeyNotFoundException($"Author with id {authorId} not found");

        var paged = await bookRepository.FindPagedAsync(b => b.AuthorId == authorId, page, pageSize, BuildOrderBy(orderBy, orderType), cancellationToken);

        var items = new List<BookModel>(paged.Items.Count);
        foreach (var book in paged.Items)
            items.Add(await ToModelWithImageAsync(book, cancellationToken));

        return new PagedList<BookModel>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            CurrentPage = paged.CurrentPage,
            PageSize = paged.PageSize
        };
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var book = await bookRepository.GetByIdAsync(id, cancellationToken);
        if (book is null)
            throw new KeyNotFoundException($"Book with id {id} not found");

        bookRepository.Delete(book);
        await bookRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<BookModel> ToModelWithImageAsync(Book book, CancellationToken ct)
    {
        var model = book.ToModel();
        var key = await fileStorage.FindKeyByPrefixAsync($"book-covers/{model.Id}", ct);
        model.ImageUrl = key is not null
            ? fileStorage.GetPublicUrl(key) ?? $"/api/books/{model.Id}/image"
            : null;
        return model;
    }

    private static Func<IQueryable<Book>, IOrderedQueryable<Book>> BuildOrderBy(string? orderBy, OrderType orderType) =>
        orderBy?.ToLower() switch
        {
            "created" => orderType == OrderType.Desc
                ? q => q.OrderByDescending(b => b.CreatedAt)
                : q => q.OrderBy(b => b.CreatedAt),
            _ => orderType == OrderType.Desc
                ? q => q.OrderByDescending(b => b.Title)
                : q => q.OrderBy(b => b.Title)
        };

}
