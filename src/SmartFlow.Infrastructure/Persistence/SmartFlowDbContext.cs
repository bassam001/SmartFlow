using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartFlow.Domain.Entities;

namespace SmartFlow.Infrastructure.Persistence;

public sealed class SmartFlowDbContext : IdentityDbContext<ApplicationUser>
{
    public SmartFlowDbContext(DbContextOptions<SmartFlowDbContext> options) : base(options) { }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Note> Notes => Set<Note>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmartFlowDbContext).Assembly);
    }
}
