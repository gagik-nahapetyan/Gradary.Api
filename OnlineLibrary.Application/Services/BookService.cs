using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents a <see cref="BookService"/>.
/// </summary>
public class BookService(IBookRepository bookRepository, ICategoryRepository categoryRepository) : IBookService
{
    public async Task<BookModel> CreateAsync(BookModel bookModel, CancellationToken cancellationToken = default)
    {
        var bookWithTitleExists = await bookRepository.ExistAsync(b => string.Equals(b.Title, bookModel.Title, StringComparison.OrdinalIgnoreCase), cancellationToken);
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

        var bookWithSameTitleExists = await bookRepository.ExistAsync(b => b.Id != bookModel.Id && string.Equals(b.Title, bookModel.Title, StringComparison.OrdinalIgnoreCase), cancellationToken);
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

        var directory = $"{Directory.GetParent(Environment.CurrentDirectory)!.FullName}\\BookFiles";
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var fullPath = $"{directory}\\{id}";
        using var stream = openStream();
        using var fileStream = new FileStream(fullPath, FileMode.Create);
        await stream.CopyToAsync(fileStream, cancellationToken);
    }

    public async Task<BookModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var book = await bookRepository.GetByIdAsync(id, cancellationToken);
        if (book is null)
            throw new KeyNotFoundException($"Book with id {id} not found");

        return book.ToModel();
    }

    public async Task<PagedList<BookModel>> GetAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var paged = await bookRepository.GetPagedAsync(page, pageSize, cancellationToken);

        return new PagedList<BookModel>
        {
            Items = paged.Items.Select(b => b.ToModel()).ToList(),
            TotalCount = paged.TotalCount,
            CurrentPage = paged.CurrentPage,
            PageSize = paged.PageSize
        };
    }

    public async Task<PagedList<BookModel>> GetByCategoryIdAsync(int categoryId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var categoryExists = await categoryRepository.ExistAsync(c => c.Id == categoryId, cancellationToken);
        if (!categoryExists)
            throw new KeyNotFoundException($"Category with id {categoryId} not found");

        var paged = await bookRepository.FindPagedAsync(b => b.CategoryId == categoryId, page, pageSize, cancellationToken);

        return new PagedList<BookModel>
        {
            Items = paged.Items.Select(b => b.ToModel()).ToList(),
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
}
