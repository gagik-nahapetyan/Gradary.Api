using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Domain.Entities;

/// <summary>
/// Represents the <see cref="BookCollection"/> entity.
/// </summary>
public class BookCollection : AuditEntity
{
    /// <summary>
    /// The id of the owner.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The name of the collection.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// An optional description of the collection.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The lifecycle status of the collection.
    /// </summary>
    public BookCollectionStatus Status { get; set; }

    /// <summary>
    /// The owner of the collection.
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// The books in this collection.
    /// </summary>
    public ICollection<BookCollectionItem> Items { get; set; } = [];
}
