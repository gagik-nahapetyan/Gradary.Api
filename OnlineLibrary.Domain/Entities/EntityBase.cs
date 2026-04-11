namespace OnlineLibrary.Domain.Entities;

/// <summary>
/// Represents the entity base.
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// The unique identifier of the entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Indicates whether the entity has been soft-deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
