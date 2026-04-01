using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Domain.Entities;

/// <summary>
/// Represents the <see cref="Review"/> entity.
/// </summary>
public class Review : AuditEntity
{
    /// <summary>
    /// The id of the reviewer.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The id of the book.
    /// </summary>
    public int BookId { get; set; }

    /// <summary>
    /// The rating of the book.
    /// </summary>
    public BookRating Rating { get; set; }

    /// <summary>
    /// The review of the book
    /// </summary>
    public string? Comment { get; set; }


    /// <summary>
    /// The related user instance.
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// The related book instance.
    /// </summary>
    public Book? Book { get; set; }
}
