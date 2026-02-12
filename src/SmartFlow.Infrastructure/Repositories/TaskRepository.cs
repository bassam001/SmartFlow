using Microsoft.EntityFrameworkCore;
using SmartFlow.Application.DTOs;
using SmartFlow.Application.Interfaces;
using SmartFlow.Domain.Entities;
using SmartFlow.Infrastructure.Persistence;

namespace SmartFlow.Infrastructure.Repositories;

public sealed class TaskRepository : ITaskRepository
{
    private readonly SmartFlowDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public TaskRepository(SmartFlowDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<List<TaskDto>> GetMyTasksAsync(CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrWhiteSpace(userId))
            return new List<TaskDto>();

        var items = await _db.Tasks
            .AsNoTracking()
            .Where(t => t.OwnerId == userId)
            .OrderByDescending(t => t.UpdatedAt)
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                DueDate = t.DueDate
            })
            .ToListAsync(ct);

        return items;
    }

    public Task AddAsync(TaskItem task, CancellationToken ct) =>
        _db.Tasks.AddAsync(task, ct).AsTask();

    public Task<TaskItem?> GetByIdAsync(Guid id, string ownerId, CancellationToken ct) =>
        _db.Tasks.FirstOrDefaultAsync(x => x.Id == id && x.OwnerId == ownerId, ct);

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(string ownerId, CancellationToken ct) =>
        await _db.Tasks
            .Where(x => x.OwnerId == ownerId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct) =>
        _db.SaveChangesAsync(ct);
}
