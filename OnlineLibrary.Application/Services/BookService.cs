using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents a <see cref="BookService"/>.
/// </summary>
public class BookService(IBookRepository bookRepository) : IBookService
{
    public async Task<BookModel> CreateAsync(BookModel bookModel, CancellationToken cancellationToken = default)
    {
        var bookWithSameTitleExists = await bookRepository.ExistAsync(b => b.Title.ToLower()  == bookModel.Title.ToLower());
        if (bookWithSameTitleExists)
            throw new ArgumentException($"Book with title {bookModel.Title} already exists");

        var book = bookModel.ToEntity();
        book = await bookRepository.InsertAsync(book);

        await bookRepository.SaveChangesAsync();

        bookModel.Id = book.Id;

        if (bookModel.Stream is not null)
            await UploadFileAsync(bookModel, bookModel.Stream, cancellationToken);

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

        if (bookModel.Stream is not null)
            await UploadFileAsync(bookModel, bookModel.Stream, cancellationToken);
    }

    public async Task<BookModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var book = await bookRepository.GetByIdAsync(id, cancellationToken);
        if (book is null)
            throw new KeyNotFoundException($"Book with id {id} not found");

        var bookModel = book.ToModel();

        return bookModel;
    }

    public async Task<List<BookModel>> GetAsync(CancellationToken cancellationToken = default)
    {
        var books = await bookRepository.GetAllAsync(cancellationToken);
        var bookModels = books.Select(b => b.ToModel()).ToList();

        return bookModels;
    }

    private async Task UploadFileAsync(BookModel bookModel, Stream inputStream, CancellationToken cancellationToken = default)
    {
        var directory = $"{Directory.GetParent(Environment.CurrentDirectory)!.FullName}\\BookFiles";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var fullPath = $"{directory}\\{bookModel.Id}_{bookModel.Title}";

        using var fileStream = new FileStream(fullPath, FileMode.Create);

        await inputStream.CopyToAsync(fileStream, cancellationToken);
    }
}
