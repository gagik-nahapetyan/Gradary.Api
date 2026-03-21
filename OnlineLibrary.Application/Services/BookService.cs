using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents a <see cref="BookService"/>.
/// </summary>
public class BookService(IBookRepository bookRepository) : IBookService
{
    public async Task<BookModel> CreateAsync(BookModel bookModel)
    {
        var book = bookModel.ToEntity();
        book = await bookRepository.InsertAsync(book);

        await bookRepository.SaveChangesAsync();

        return book.ToModel();
    }

    public async Task UpdateAsync(BookModel bookModel)
    {
        var existingBook = await bookRepository.GetByIdAsync(bookModel.Id);
        if (existingBook is null)
            throw new KeyNotFoundException($"Book with id {bookModel.Id} not found");

        var book = bookModel.ToEntity();
        bookRepository.Update(book);

        await bookRepository.SaveChangesAsync();
    }

    public async Task<List<BookModel>> GetAsync()
    {
        var books = await bookRepository.GetAllAsync();
        var bookModels = books.Select(b => b.ToModel()).ToList();

        return bookModels;
    }

    public async Task<BookModel> GetByIdAsync(int id)
    {
        var book = await bookRepository.GetByIdAsync(id);
        if (book is null)
            throw new KeyNotFoundException($"Book with id {id} not found");

        var bookModel = book.ToModel();

        return bookModel;
    }
}
