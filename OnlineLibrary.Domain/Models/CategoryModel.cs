namespace OnlineLibrary.Domain.Models;

/// <summary>
/// Represents the <see cref="CategoryModel"/> class.
/// </summary>
public class CategoryModel : AuditModel
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
}

