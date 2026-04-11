using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Domain.Models;

/// <summary>
/// Represents the book collection model.
/// </summary>
public class BookCollectionModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public BookCollectionStatus Status { get; set; }
    public List<BookCollectionItemModel> Items { get; set; } = [];
}
