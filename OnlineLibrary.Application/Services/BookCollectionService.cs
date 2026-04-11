using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Application.Validators;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents a <see cref="BookCollectionService"/>.
/// </summary>
public class BookCollectionService(
    IBookCollectionRepository bookCollectionRepository,
    IBookCollectionItemRepository bookCollectionItemRepository,
    BookCollectionValidator validator) : IBookCollectionService
{
    public async Task<BookCollectionModel> CreateAsync(BookCollectionModel model, CancellationToken cancellationToken = default)
    {
        await validator.ValidateCreateAsync(model, cancellationToken);

        var entity = model.ToEntity();
        entity = await bookCollectionRepository.InsertAsync(entity, cancellationToken);
        await bookCollectionRepository.SaveChangesAsync(cancellationToken);

        return entity.ToModel();
    }

    public async Task<BookCollectionModel> UpdateAsync(BookCollectionModel model, int callerId, CancellationToken cancellationToken = default)
    {
        var collection = await bookCollectionRepository.GetByIdWithItemsAsync(model.Id, cancellationToken);
        await validator.ValidateUpdateAsync(collection, model, callerId, cancellationToken);

        collection!.Name = model.Name;
        collection.Description = model.Description;
        collection.Status = model.Status;

        bookCollectionRepository.Update(collection);
        await bookCollectionRepository.SaveChangesAsync(cancellationToken);

        return collection.ToModel();
    }

    public async Task<BookCollectionModel> GetByIdAsync(int id, int callerId, CancellationToken cancellationToken = default)
    {
        var collection = await bookCollectionRepository.GetByIdWithItemsAsync(id, cancellationToken);
        BookCollectionValidator.ValidateGetById(collection, callerId);

        return collection!.ToModel();
    }

    public async Task<List<BookCollectionModel>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var collections = await bookCollectionRepository.GetByUserIdAsync(userId, cancellationToken);
        return [.. collections.Select(c => c.ToModel())];
    }

    public async Task DeleteAsync(int id, int callerId, CancellationToken cancellationToken = default)
    {
        var collection = await bookCollectionRepository.GetByIdWithItemsAsync(id, cancellationToken);
        BookCollectionValidator.ValidateGetById(collection, callerId);

        bookCollectionRepository.Delete(collection!);
        await bookCollectionRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<BookCollectionItemModel> AddBookAsync(int collectionId, BookCollectionItemModel model, int callerId, CancellationToken cancellationToken = default)
    {
        var collection = await bookCollectionRepository.GetByIdWithItemsAsync(collectionId, cancellationToken);
        await validator.ValidateAddBookAsync(collection, model, callerId, cancellationToken);

        model.BookCollectionId = collectionId;
        var entity = model.ToEntity();
        entity = await bookCollectionItemRepository.InsertAsync(entity, cancellationToken);
        await bookCollectionItemRepository.SaveChangesAsync(cancellationToken);

        return entity.ToModel();
    }

    public async Task<BookCollectionItemModel> UpdateBookAsync(int collectionId, BookCollectionItemModel model, int callerId, CancellationToken cancellationToken = default)
    {
        var collection = await bookCollectionRepository.GetByIdWithItemsAsync(collectionId, cancellationToken);
        var item = await bookCollectionItemRepository.GetByCollectionAndBookAsync(collectionId, model.BookId, cancellationToken);
        validator.ValidateUpdateBook(collection, item, model, callerId);

        item!.Status = model.Status;
        item.Position = model.Position;

        bookCollectionItemRepository.Update(item);
        await bookCollectionItemRepository.SaveChangesAsync(cancellationToken);

        return item.ToModel();
    }

    public async Task RemoveBookAsync(int collectionId, int bookId, int callerId, CancellationToken cancellationToken = default)
    {
        var collection = await bookCollectionRepository.GetByIdWithItemsAsync(collectionId, cancellationToken);
        var item = await bookCollectionItemRepository.GetByCollectionAndBookAsync(collectionId, bookId, cancellationToken);
        BookCollectionValidator.ValidateRemoveBook(collection, item, bookId, callerId);

        bookCollectionItemRepository.Delete(item!);
        await bookCollectionItemRepository.SaveChangesAsync(cancellationToken);
    }
}
