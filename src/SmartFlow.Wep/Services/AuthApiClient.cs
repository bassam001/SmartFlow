using System.Net.Http.Json;

namespace SmartFlow.Wep.Services;

public sealed class AuthApiClient
{
    private readonly HttpClient _http;

    public AuthApiClient(HttpClient http) => _http = http;

    public Task<HttpResponseMessage> RegisterAsync(string email, string password) =>
        _http.PostAsJsonAsync("api/auth/register", new { email, password });

    public async Task<string?> LoginAsync(string email, string password)
    {
        var resp = await _http.PostAsJsonAsync("api/auth/login", new { email, password });

        if (!resp.IsSuccessStatusCode)
        {
            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new InvalidOperationException("Wrong email or password.");

            var body = await resp.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Login failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. {body}");
        }

        var json = await resp.Content.ReadFromJsonAsync<LoginResponse>();
        return json?.Token;
    }


    private sealed record LoginResponse(string Token);
}
