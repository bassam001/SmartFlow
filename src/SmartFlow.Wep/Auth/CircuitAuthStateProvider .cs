using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace SmartFlow.Wep.Auth;

public sealed class CircuitAuthStateProvider : AuthenticationStateProvider
{
    private readonly IHttpContextAccessor _http;

    public CircuitAuthStateProvider(IHttpContextAccessor http)
    {
        _http = http;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = _http.HttpContext?.User
                   ?? new ClaimsPrincipal(new ClaimsIdentity());

        return Task.FromResult(new AuthenticationState(user));
    }

    public void Notify()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
