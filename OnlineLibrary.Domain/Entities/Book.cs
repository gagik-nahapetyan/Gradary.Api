namespace OnlineLibrary.Domain.Entities;

/// <summary>
/// Represents the Book entity.
/// </summary>
public class Book : AuditEntity
{
    /// <summary>
    /// The main title of the book.
    /// </summary>
    public required string ShortTitle { get; set; }

    /// <summary>
    /// The main title with the subtitle of the book.
    /// </summary>
    public string? FullTitle { get; set; }

    /// <summary>
    /// The description of the book.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The author of the book.
    /// </summary>
    public required int AuthorId { get; set; }

    /// <summary>
    /// The category id of the book.
    /// </summary>
    public required int CategoryId { get; set; }


    /// <summary>
    /// The related Author instance.
    /// </summary>
    public Author? Author { get; set; }

    /// <summary>
    /// The related Category instance.
    /// </summary>
    public Category? Category { get; set; }


    /// <summary>
    /// The list of the related Reviews.
    /// </summary>
    public virtual ICollection<Review> Reviews { get; set; } = [];
}
