using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace SmartFlow.Wep.Controllers;

[ApiController]
[Route("ui-auth")]
public sealed class UiAuthController : ControllerBase
{
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
            new(ClaimTypes.Email, email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        HttpContext.Session.SetString("access_token", token);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return LocalRedirect(returnUrl);
    }

    [HttpGet("signout")]
    public async Task<IActionResult> SignOutGet([FromQuery] string returnUrl = "/")
    {
        HttpContext.Session.Remove("access_token");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return LocalRedirect(returnUrl);
    }
}
