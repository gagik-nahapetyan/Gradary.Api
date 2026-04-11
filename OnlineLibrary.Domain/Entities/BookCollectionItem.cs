using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Domain.Entities;

/// <summary>
/// Represents a book entry inside a <see cref="BookCollection"/>.
/// </summary>
public class BookCollectionItem : AuditEntity
{
    /// <summary>
    /// The id of the parent collection.
    /// </summary>
    public int BookCollectionId { get; set; }

    /// <summary>
    /// The id of the book.
    /// </summary>
    public int BookId { get; set; }

    /// <summary>
    /// The reading status of this book in the collection.
    /// </summary>
    public BookCollectionItemStatus Status { get; set; }

    /// <summary>
    /// The display position of this book within the collection.
    /// </summary>
    public int Position { get; set; }


    /// <summary>
    /// The parent collection.
    /// </summary>
    public BookCollection? BookCollection { get; set; }

    /// <summary>
    /// The related book.
    /// </summary>
    public Book? Book { get; set; }
}
