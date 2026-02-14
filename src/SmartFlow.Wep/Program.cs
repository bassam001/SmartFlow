using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using SmartFlow.Wep.Auth;
using SmartFlow.Wep.Services;
using SmartFlow.Wep.Components;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// =======================
// Razor + Blazor Server
// =======================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();

// =======================
// 🔐 Data Protection (IMPORTANT for Azure)
// =======================
var home = Environment.GetEnvironmentVariable("HOME") ?? @"C:\home";
var keysPath = Path.Combine(home, "DataProtectionKeys");

builder.Services
    .AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("SmartFlow");

// =======================
// 🍪 Cookie Authentication
// =======================
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";

        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

builder.Services.AddAuthorization();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<AuthenticationStateProvider, HttpContextAuthStateProvider>();

// =======================
// HTTP Clients
// =======================

// Client للـ UI نفسه
builder.Services.AddHttpClient("Ui", (sp, client) =>
{
    var http = sp.GetRequiredService<IHttpContextAccessor>().HttpContext!;
    client.BaseAddress = new Uri($"{http.Request.Scheme}://{http.Request.Host}/");
});

builder.Services.AddScoped<AuthHeaderHandler>();

// Client للـ API
var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    throw new InvalidOperationException("ApiBaseUrl is missing. Set it in Azure Configuration.");
}

builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api")
);

builder.Services.AddScoped<AuthApiClient>();
builder.Services.AddScoped<TasksApiClient>();

builder.Services.AddControllers();

var app = builder.Build();

// =======================
// Forwarded Headers (IMPORTANT for Azure)
// =======================
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                       ForwardedHeaders.XForwardedProto
});

// =======================
// Production settings
// =======================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
