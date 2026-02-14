using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SmartFlow.Api.Services;
using SmartFlow.Application.Interfaces;
using SmartFlow.Infrastructure.Persistence;
using SmartFlow.Infrastructure.Repositories;
using System.Text;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((ctx, lc) =>
            lc.ReadFrom.Configuration(ctx.Configuration));

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        builder.Services.AddDbContext<SmartFlowDbContext>(options =>
        {
            var conn = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException("Missing connection string: DefaultConnection");

            options.UseSqlServer(conn);
        });


        builder.Services.AddScoped<ITaskRepository, TaskRepository>();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddHttpContextAccessor();


        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(SmartFlow.Application.DTOs.TaskDto).Assembly));


        builder.Services.AddValidatorsFromAssembly(typeof(SmartFlow.Application.DTOs.TaskDto).Assembly);

        builder.Services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<SmartFlowDbContext>()
            .AddDefaultTokenProviders();

        // JWT (fail fast if missing)
        var jwtSection = builder.Configuration.GetSection("Jwt");
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var key = jwtSection["Key"];

        if (string.IsNullOrWhiteSpace(issuer) ||
            string.IsNullOrWhiteSpace(audience) ||
            string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException(
                "Missing JWT settings. Required: Jwt__Issuer, Jwt__Audience, Jwt__Key");
        }

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

                    ClockSkew = TimeSpan.FromMinutes(2)
                };
            });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        app.UseSerilogRequestLogging();

        if (!app.Environment.IsProduction())
        {
            app.UseDeveloperExceptionPage();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

 
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
