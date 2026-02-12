using MediatR;
using SmartFlow.Application.DTOs;
using SmartFlow.Domain.Enums;

namespace SmartFlow.Application.Features.Tasks.CreateTask;

public sealed record CreateTaskCommand(
    string Title,
    string Description,
    DateTime DueDate,
    TaskPriority Priority,
    StatusOfTask Status 
) : IRequest<TaskDto>;
