using MediatR;
using SmartFlow.Application.DTOs;
using SmartFlow.Application.Interfaces;
using SmartFlow.Domain.Entities;

namespace SmartFlow.Application.Features.Tasks.CreateTask;

public sealed class CreateTaskHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly ITaskRepository _repo;
    private readonly ICurrentUserService _currentUser;

    public CreateTaskHandler(ITaskRepository repo, ICurrentUserService currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }

    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken ct)
    {
        var entity = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            Priority = request.Priority,
            Status = request.Status,
            OwnerId = _currentUser.UserId
        };

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return new TaskDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Status = entity.Status,
            Priority = entity.Priority,
            DueDate = entity.DueDate,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
