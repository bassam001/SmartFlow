using MediatR;
using SmartFlow.Application.DTOs;

namespace SmartFlow.Application.Features.Tasks.GetFocusTasks;

public sealed record GetFocusTasksQuery(int Take) : IRequest<List<TaskDto>>;
