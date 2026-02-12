using System.Net.Http.Json;
using SmartFlow.Domain.Enums;

namespace SmartFlow.Wep.Services;

public sealed class TasksApiClient
{
    private readonly HttpClient _http;

    public TasksApiClient(HttpClient http) => _http = http;

    public Task<List<TaskDto>?> GetTasksAsync() =>
        _http.GetFromJsonAsync<List<TaskDto>>("api/tasks");

    public Task<List<TaskDto>?> GetFocusAsync(int take = 3) =>
        _http.GetFromJsonAsync<List<TaskDto>>($"api/tasks/focus?take={take}");

    public async Task CreateTaskAsync(string title, string description, DateTime dueDate, TaskPriority priority, StatusOfTask status)
    {
        var resp = await _http.PostAsJsonAsync("api/tasks", new
        {
            title,
            description,
            dueDate,
            priority,
            status
        });

        resp.EnsureSuccessStatusCode();
    }

    public sealed class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public StatusOfTask Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
