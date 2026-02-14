using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace SmartFlow.Wep.Controllers;

[ApiController]
[Route("ui-auth")]
public sealed class UiAuthController : ControllerBase
{
    private readonly CircuitAuthStateProvider _authState;

    public UiAuthController(CircuitAuthStateProvider authState)
    {
        _authState = authState;
    }

    [HttpGet("signin")]
    public async Task<IActionResult> SignIn(
        [FromQuery] string userId,
        [FromQuery] string email,
        [FromQuery] string token,
        [FromQuery] string returnUrl = "/dashboard")
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email),
            new("access_token", token)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        _authState.SetUser(principal);
        return LocalRedirect(returnUrl);
    }

    [HttpGet("signout")]
    public async Task<IActionResult> SignOutGet([FromQuery] string returnUrl = "/")
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _authState.ClearUser();
        return LocalRedirect(returnUrl);
    }

}
