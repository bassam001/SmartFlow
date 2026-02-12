using SmartFlow.Application.DTOs;
using SmartFlow.Domain.Entities;

namespace SmartFlow.Application.Interfaces;

public interface ITaskRepository
{
    Task AddAsync(TaskItem task, CancellationToken ct);
    Task<TaskItem?> GetByIdAsync(Guid id, string ownerId, CancellationToken ct);
    Task<IReadOnlyList<TaskItem>> GetAllAsync(string ownerId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task<List<TaskDto>> GetMyTasksAsync(CancellationToken ct);
}
