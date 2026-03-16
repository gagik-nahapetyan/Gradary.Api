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
    /// The parent id of the category.
    /// </summary>
    public int? ParentId { get; set; }


    /// <summary>
    /// The parent category in the hierarchy.
    /// </summary>
    public virtual Category? Parent { get; set; }


    /// <summary>
    /// The child categories in the hierarchy.
    /// </summary>
    public virtual ICollection<Category> Children { get; set; } = [];

    /// <summary>
    /// The list of the related books.
    /// </summary>
    public virtual ICollection<Book> Books { get; set; } = [];
}
