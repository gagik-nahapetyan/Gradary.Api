namespace OnlineLibrary.Domain.Enums;

/// <summary>
/// Represents the reading status of a book inside a <see cref="Entities.BookCollection"/>.
/// </summary>
public enum BookCollectionItemStatus
{
    WantToRead = 1,
    Reading = 2,
    Finished = 3,
    Dropped = 4
}
