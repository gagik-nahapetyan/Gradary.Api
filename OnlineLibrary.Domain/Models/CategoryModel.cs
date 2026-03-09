namespace OnlineLibrary.Domain.Models;

/// <summary>
/// Represents the <see cref="CategoryModel"/> class.
/// </summary>
public class CategoryModel : AuditModel
{
    /// <summary>
    /// The name of the category.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The description of the category.
    /// </summary>
    public string? Description { get; set; }
}

