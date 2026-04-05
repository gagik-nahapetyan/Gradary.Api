namespace OnlineLibrary.Domain.Entities;

/// <summary>
/// Represents the audit entity.
/// </summary>
public abstract class AuditEntity : EntityBase
{
    /// <summary>
    /// The created date of the item.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// The id of the user the item of created by.
    /// </summary>
    public int? CreatedBy { get; set; }

    /// <summary>
    /// The updated date of the item.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// The id of the user the item of updated by.
    /// </summary>
    public int? UpdatedBy { get; set; }
}
