using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using SmartFlow.Wep.Auth;
using SmartFlow.Wep.Services;
using SmartFlow.Wep.Components;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
    });

builder.Services.AddAuthorization();


builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<AuthenticationStateProvider, HttpContextAuthStateProvider>();


builder.Services.AddHttpClient("Ui", client =>
{
    client.BaseAddress = new Uri("http://localhost:5207/");
});


builder.Services.AddScoped<AuthHeaderHandler>();

builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri("http://localhost:5194/");
})
.AddHttpMessageHandler<AuthHeaderHandler>();


builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api")
);

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<AuthApiClient>();
builder.Services.AddScoped<TasksApiClient>();


builder.Services.AddControllers();


var app = builder.Build();


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
