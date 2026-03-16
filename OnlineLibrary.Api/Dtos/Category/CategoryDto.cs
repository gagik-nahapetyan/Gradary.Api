namespace OnlineLibrary.Api.Dtos.Category;

/// <summary>
/// Represents the <see cref="CategoryDto"/> class.
/// </summary>
public class CategoryDto
{
    /// <summary>
    /// The id of the category.
    /// </summary>
    public int Id { get; set; }

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
}

