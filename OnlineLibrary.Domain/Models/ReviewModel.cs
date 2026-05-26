using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Domain.Models;

/// <summary>
/// Represents the <see cref="ReviewModel"/> class.
/// </summary>
public class ReviewModel : AuditModel
{
    /// <summary>
    /// The id of the reviewer.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The full name of the reviewer.
    /// </summary>
    public string? UserFullName { get; set; }

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
}
