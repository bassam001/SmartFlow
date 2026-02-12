using MediatR;
using SmartFlow.Application.DTOs;

namespace SmartFlow.Application.Features.Tasks.GetTasks;

public sealed record GetTasksQuery() : IRequest<IReadOnlyList<TaskDto>>;
