using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using SmartFlow.Wep.Auth;
using SmartFlow.Wep.Components;
using SmartFlow.Wep.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

var home = Environment.GetEnvironmentVariable("HOME");
var keysPath = Path.Combine(home ?? builder.Environment.ContentRootPath, "DataProtectionKeys");
Directory.CreateDirectory(keysPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("SmartFlow");

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";

        options.Cookie.Name = "__Host-SmartFlow.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });


builder.Services.AddHttpClient("Ui", (sp, client) =>
{
    var http = sp.GetRequiredService<IHttpContextAccessor>().HttpContext!;
    client.BaseAddress = new Uri($"{http.Request.Scheme}://{http.Request.Host}/");
});

builder.Services.AddScoped<AuthHeaderHandler>();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
if (string.IsNullOrWhiteSpace(apiBaseUrl))

    apiBaseUrl = "https://localhost:5194";

builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

builder.Services.AddScoped<AuthApiClient>();
builder.Services.AddScoped<TasksApiClient>();

builder.Services.AddScoped<AuthenticationStateProvider, Microsoft.AspNetCore.Components.Server.ServerAuthenticationStateProvider>();


builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<TokenStore>();
builder.Services.AddScoped<JwtAuthStateProvider>();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();