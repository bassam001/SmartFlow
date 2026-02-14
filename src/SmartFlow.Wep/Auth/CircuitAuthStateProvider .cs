using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

public sealed class CircuitAuthStateProvider : AuthenticationStateProvider
{
    private AuthenticationState _state =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
        => Task.FromResult(_state);

    public void SetUser(ClaimsPrincipal user)
    {
        _state = new AuthenticationState(user);
        NotifyAuthenticationStateChanged(Task.FromResult(_state));
    }

    public void ClearUser()
    {
        _state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        NotifyAuthenticationStateChanged(Task.FromResult(_state));
    }
}
