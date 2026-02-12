using MediatR;
using SmartFlow.Application.DTOs;
using SmartFlow.Application.Interfaces;

namespace SmartFlow.Application.Features.Tasks.GetTasks;

public sealed class GetTasksHandler : IRequestHandler<GetTasksQuery, IReadOnlyList<TaskDto>>
{
    private readonly ITaskRepository _repo;
    private readonly ICurrentUserService _currentUser;

    public GetTasksHandler(ITaskRepository repo, ICurrentUserService currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<TaskDto>> Handle(GetTasksQuery request, CancellationToken ct)
    {
        var items = await _repo.GetAllAsync(_currentUser.UserId, ct);

        return items.Select(x => new TaskDto
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            Status = x.Status,
            Priority = x.Priority,
            DueDate = x.DueDate,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }).ToList();
    }
}
