using SmartFlow.Domain.Common;

namespace SmartFlow.Domain.Entities;

public class Note : AuditableEntity
{
    public Guid? TaskId { get; set; }   // optional link to task
    public string Content { get; set; } = null!;

    public string OwnerId { get; set; } = null!;
}
