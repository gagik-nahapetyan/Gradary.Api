using Microsoft.Extensions.Options;
using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Domain.Enums;
using OnlineLibrary.Domain.Models;
using OnlineLibrary.Domain.Settings;

namespace OnlineLibrary.Application.Validators;

/// <summary>
/// Represents a validator for book collection business rules.
/// </summary>
public class BookCollectionValidator(
    IBookCollectionRepository bookCollectionRepository,
    IBookCollectionItemRepository bookCollectionItemRepository,
    IBookRepository bookRepository,
    IOptions<BookCollectionSettings> settings)
{
    private readonly BookCollectionSettings _settings = settings.Value;

    public async Task ValidateCreateAsync(BookCollectionModel model, CancellationToken cancellationToken)
    {
        await ValidateActiveCollectionLimitAsync(model.UserId, cancellationToken);
        await ValidateCollectionNameAsync(model.UserId, model.Name, cancellationToken);
    }

    public async Task ValidateUpdateAsync(BookCollection? collection, BookCollectionModel model, int callerId, CancellationToken cancellationToken)
    {
        ValidateExists(collection);
        ValidateOwnership(collection!, callerId);

        if (IsActiveCollection(model.Status) && !IsActiveCollection(collection!.Status))
            await ValidateActiveCollectionLimitAsync(callerId, cancellationToken);
    }

    public static void ValidateGetById(BookCollection? collection, int callerId)
    {
        ValidateExists(collection);
        ValidateOwnership(collection!, callerId);
    }

    public async Task ValidateAddBookAsync(BookCollection? collection, BookCollectionItemModel model, int callerId, CancellationToken cancellationToken)
    {
        ValidateExists(collection);
        ValidateOwnership(collection!, callerId);

        await ValidateBookExistsAsync(model.BookId, cancellationToken);
        await ValidateBookNotDuplicateAsync(collection!.Id, model.BookId, cancellationToken);
        ValidateActiveItemLimit(collection.Items, model.Status);
        ValidateItemPositionNotTaken(collection.Items, model.Position);
    }

    public void ValidateUpdateBook(BookCollection? collection, BookCollectionItem? item, BookCollectionItemModel model, int callerId)
    {
        ValidateExists(collection);
        ValidateOwnership(collection!, callerId);
        ValidateItemExists(item, model.BookId);

        if (IsActiveItem(model.Status) && !IsActiveItem(item!.Status))
            ValidateActiveItemLimit(collection!.Items, model.Status, excludeBookId: model.BookId);

        if (item!.Position != model.Position)
            ValidateItemPositionNotTaken(collection!.Items, model.Position, excludeBookId: model.BookId);
    }

    public static void ValidateRemoveBook(BookCollection? collection, BookCollectionItem? item, int bookId, int callerId)
    {
        ValidateExists(collection);
        ValidateOwnership(collection!, callerId);
        ValidateItemExists(item, bookId);
    }

    private static void ValidateExists(BookCollection? collection)
    {
        if (collection is null)
            throw new KeyNotFoundException("Collection not found.");
    }

    private static void ValidateOwnership(BookCollection collection, int callerId)
    {
        if (collection.UserId != callerId)
            throw new UnauthorizedAccessException("You do not own this collection.");
    }

    private static void ValidateItemExists(BookCollectionItem? item, int bookId)
    {
        if (item is null)
            throw new KeyNotFoundException($"Book with id {bookId} not found in this collection.");
    }

    private static void ValidateItemPositionNotTaken(ICollection<BookCollectionItem> items, int position, int excludeBookId = 0)
    {
        if (items.Any(i => i.BookId != excludeBookId && i.Position == position))
            throw new InvalidOperationException($"Position {position} is already occupied in this collection.");
    }

    private void ValidateActiveItemLimit(ICollection<BookCollectionItem> items, BookCollectionItemStatus newStatus, int excludeBookId = 0)
    {
        if (!IsActiveItem(newStatus))
            return;

        var activeCount = items.Count(i => i.BookId != excludeBookId && IsActiveItem(i.Status));
        if (activeCount >= _settings.MaxActiveBooksPerCollection)
            throw new InvalidOperationException($"Cannot have more than {_settings.MaxActiveBooksPerCollection} active books in a collection.");
    }

    private async Task ValidateCollectionNameAsync(int userId, string name, CancellationToken cancellationToken)
    {
        var nameExists = await bookCollectionRepository.ExistAsync(
            c => c.UserId == userId && c.Name == name, cancellationToken);

        if (nameExists)
            throw new ArgumentException($"A collection named '{name}' already exists.");
    }

    private async Task ValidateActiveCollectionLimitAsync(int userId, CancellationToken cancellationToken)
    {
        var activeCount = await bookCollectionRepository.CountAsync(
            c => c.UserId == userId &&
                 (c.Status == BookCollectionStatus.NotStarted || c.Status == BookCollectionStatus.InProgress),
            cancellationToken);

        if (activeCount >= _settings.MaxActiveCollections)
            throw new InvalidOperationException($"Cannot have more than {_settings.MaxActiveCollections} active collections.");
    }

    private async Task ValidateBookExistsAsync(int bookId, CancellationToken cancellationToken)
    {
        if (!await bookRepository.ExistAsync(b => b.Id == bookId, cancellationToken))
            throw new KeyNotFoundException($"Book with id {bookId} not found.");
    }

    private async Task ValidateBookNotDuplicateAsync(int collectionId, int bookId, CancellationToken cancellationToken)
    {
        var exists = await bookCollectionItemRepository.ExistAsync(
            i => i.BookCollectionId == collectionId && i.BookId == bookId, cancellationToken);

        if (exists)
            throw new ArgumentException($"Book with id {bookId} is already in this collection.");
    }

    private static bool IsActiveCollection(BookCollectionStatus status)
    {
        return status is BookCollectionStatus.NotStarted or BookCollectionStatus.InProgress;
    }

    private static bool IsActiveItem(BookCollectionItemStatus status)
    {
        return status is BookCollectionItemStatus.WantToRead or BookCollectionItemStatus.Reading;
    }
}
