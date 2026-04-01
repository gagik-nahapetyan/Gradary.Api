using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Api.Dtos.Review;

/// <summary>
/// Represents the <see cref="ReviewDto"/> class.
/// </summary>
public class ReviewDto
{
    /// <summary>
    /// The id of the review.
    /// </summary>
    public int Id { get; set; }
    
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
    /// The review of the book.
    /// </summary>
    public string? Comment { get; set; }
}

