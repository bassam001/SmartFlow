using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SmartFlow.Api.Services;
using SmartFlow.Application.Interfaces;
using SmartFlow.Infrastructure.Persistence;
using SmartFlow.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
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


        builder.Services.AddDbContext<SmartFlowDbContext>(opt =>
            opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


        // DI
        builder.Services.AddScoped<ITaskRepository, TaskRepository>();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

        // MediatR (scan Application assembly)
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(SmartFlow.Application.DTOs.TaskDto).Assembly));

        // FluentValidation auto
        builder.Services.AddValidatorsFromAssembly(typeof(SmartFlow.Application.DTOs.TaskDto).Assembly);


        builder.Services.AddHttpContextAccessor();

        builder.Services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<SmartFlowDbContext>();

        var jwtSection = builder.Configuration.GetSection("Jwt");
        var key = jwtSection["Key"]!;

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
                    ValidIssuer = jwtSection["Issuer"],
                    ValidAudience = jwtSection["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        app.UseSerilogRequestLogging();


        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<SmartFlowDbContext>();
            db.Database.Migrate();
        }


        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}