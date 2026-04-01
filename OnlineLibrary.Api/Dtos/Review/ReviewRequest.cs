using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Api.Dtos.Review;

/// <summary>
/// Represents the <see cref="ReviewRequest"/> class.
/// </summary>
public class ReviewRequest
{
    /// <summary>
    /// The id of the reviewer.
    /// </summary>
    public required int UserId { get; set; }
    
    /// <summary>
    /// The id of the book.
    /// </summary>
    public required int BookId { get; set; }
    
    /// <summary>
    /// The rating of the book.
    /// </summary>
    public required BookRating Rating { get; set; }
    
    /// <summary>
    /// The review of the book.
    /// </summary>
    public string? Comment { get; set; }
}

