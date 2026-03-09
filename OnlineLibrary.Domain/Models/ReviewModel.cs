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
    /// The id of the book.
    /// </summary>
    public int BookId { get; set; }

    /// <summary>
    /// The rating of the book.
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// The review of the book
    /// </summary>
    public required string Comment { get; set; }
}
