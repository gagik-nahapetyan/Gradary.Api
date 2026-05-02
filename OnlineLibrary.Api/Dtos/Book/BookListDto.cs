namespace OnlineLibrary.Api.Dtos.Book;

/// <summary>
/// Represents a book summary returned in paginated list responses.
/// </summary>
public class BookListDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? AuthorName { get; set; }
    public string? CategoryName { get; set; }
    public string? ImageUrl { get; set; }
}
