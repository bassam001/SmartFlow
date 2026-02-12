using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace SmartFlow.Wep.Auth;

public sealed class AuthHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _http;

    public AuthHeaderHandler(IHttpContextAccessor http) => _http = http;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _http.HttpContext?.User?.FindFirst("access_token")?.Value;

        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return base.SendAsync(request, cancellationToken);
    }
}
