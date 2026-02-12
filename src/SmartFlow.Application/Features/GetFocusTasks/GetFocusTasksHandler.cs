using MediatR;
using SmartFlow.Application.DTOs;
using SmartFlow.Application.Interfaces;
using SmartFlow.Domain.Enums;

namespace SmartFlow.Application.Features.Tasks.GetFocusTasks;

public sealed class GetFocusTasksHandler : IRequestHandler<GetFocusTasksQuery, List<TaskDto>>
{
    private readonly ITaskRepository _repo;

    public GetFocusTasksHandler(ITaskRepository repo) => _repo = repo;

    public async Task<List<TaskDto>> Handle(GetFocusTasksQuery request, CancellationToken ct)
    {
        var tasks = await _repo.GetMyTasksAsync(ct);

        var now = DateTime.UtcNow.Date;

        var focused = tasks
            .Where(t => t.Status != StatusOfTask.Done)
            .Select(t => new
            {
                Task = t,
                Score = Score(t, now)
            })
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Task.DueDate ?? DateTime.MaxValue)
            .Take(request.Take)
            .Select(x => x.Task)
            .ToList();

        return focused;
    }

    private static int Score(TaskDto t, DateTime todayUtc)
    {
        var score = 0;

        // Priority
        score += t.Priority switch
        {
            TaskPriority.High => 20,
            TaskPriority.Medium => 10, 
            _ => 0   
        };

        // Status
        score += t.Status switch
        {
            StatusOfTask.InProgress => 8,  // InProgress
            StatusOfTask.Todo => 5,  // Todo
            _ => 0
        };

        // DueDate urgency
        if (t.DueDate is null)
            return score;

        var due = t.DueDate.Value.Date;
        var days = (due - todayUtc).Days;

        if (days < 0) score += 100;        
        else if (days == 0) score += 80;  
        else if (days == 1) score += 65;   
        else if (days <= 3) score += 45;
        else if (days <= 7) score += 25;

        return score;
    }
}
