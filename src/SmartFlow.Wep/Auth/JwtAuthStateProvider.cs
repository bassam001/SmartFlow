using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace SmartFlow.Wep.Auth;

public sealed class JwtAuthStateProvider : AuthenticationStateProvider
{
    private readonly TokenStore _store;

    public JwtAuthStateProvider(TokenStore store) => _store = store;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (string.IsNullOrWhiteSpace(_store.Token))
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(_store.Token);

        var identity = new ClaimsIdentity(jwt.Claims, authenticationType: "jwt");
        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }

    public void NotifyUserAuthentication(string token)
    {
        _store.Set(token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void NotifyUserLogout()
    {
        _store.Clear();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
