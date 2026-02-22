namespace OnlineLibrary.Domain.Entities;

/// <summary>
/// Represents the <see cref="Category"/> entity.
/// </summary>
public class Category : EntityBase
{
    /// <summary>
    /// The name of the category.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The description of the category.
    /// </summary>
    public string? Description { get; set; }


    /// <summary>
    /// The list of the related books.
    /// </summary>
    public virtual ICollection<Book> Books { get; set; } = [];
}
