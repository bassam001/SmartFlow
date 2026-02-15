using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;

namespace SmartFlow.Wep.Auth;

public sealed class AuthHeaderHandler : DelegatingHandler
{
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthHeaderHandler(AuthenticationStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var state = await _authStateProvider.GetAuthenticationStateAsync();
        var token = state.User.FindFirst("access_token")?.Value;

        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
