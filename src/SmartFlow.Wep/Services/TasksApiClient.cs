using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using SmartFlow.Domain.Enums;

namespace SmartFlow.Wep.Services;

public sealed class TasksApiClient
{
    private readonly HttpClient _http;
    private readonly AuthenticationStateProvider _auth;

    public TasksApiClient(HttpClient http, AuthenticationStateProvider auth)
    {
        _http = http;
        _auth = auth;
    }

    private async Task AttachBearerAsync()
    {
        var state = await _auth.GetAuthenticationStateAsync();
        var token = state.User.FindFirst("access_token")?.Value;

        if (string.IsNullOrWhiteSpace(token))
        {
            _http.DefaultRequestHeaders.Authorization = null;
            return;
        }

        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<TaskDto>?> GetTasksAsync()
    {
        await AttachBearerAsync();
        return await _http.GetFromJsonAsync<List<TaskDto>>("api/tasks");
    }

    public async Task<List<TaskDto>?> GetFocusAsync(int take = 3)
    {
        await AttachBearerAsync();
        return await _http.GetFromJsonAsync<List<TaskDto>>($"api/tasks/focus?take={take}");
    }

    public async Task CreateTaskAsync(string title, string description, DateTime dueDate, TaskPriority priority, StatusOfTask status)
    {
        await AttachBearerAsync();

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
