using Microsoft.AspNetCore.Http;

namespace OnlineLibrary.Api.Dtos.Book;

/// <summary>
/// Represents the <see cref="BookRequest"/> class.
/// </summary>
public class BookRequest
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

    /// <summary>
    /// The content file of the book.
    /// </summary>
    public IFormFile? File { get; set; }
}

