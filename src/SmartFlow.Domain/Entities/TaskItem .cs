using SmartFlow.Domain.Common;
using SmartFlow.Domain.Enums;

namespace SmartFlow.Domain.Entities;

public class TaskItem : AuditableEntity
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public StatusOfTask Status { get; set; } = StatusOfTask.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime? DueDate { get; set; }

    // multi-tenant / per-user
    public string OwnerId { get; set; } = null!;
}
