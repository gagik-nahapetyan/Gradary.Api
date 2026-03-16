namespace OnlineLibrary.Api.Dtos.Book;

/// <summary>
/// Represents the <see cref="BookDto"/> class.
/// </summary>
public class BookDto
{
    /// <summary>
    /// The id of the book.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The title of the book.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// The author of the book.
    /// </summary>
    public required int AuthorId { get; set; }

    /// <summary>
    /// The category id of the book.
    /// </summary>
    public required int CategoryId { get; set; }

    /// <summary>
    /// The description of the book.
    /// </summary>
    public string? Description { get; set; }
}

