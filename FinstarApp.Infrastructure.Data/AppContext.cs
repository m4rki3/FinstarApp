using FinstarApp.Domain.Core;
using Microsoft.EntityFrameworkCore;

namespace FinstarApp.Infrastructure.Data;

public class AppContext : DbContext
{
    private readonly IEntityTypeConfiguration<TaskEntity> _taskConfiguration;
    
    public DbSet<TaskEntity> Tasks { get; set; }

    public AppContext(DbContextOptions<AppContext> options, IEntityTypeConfiguration<TaskEntity> taskConfiguration)
        : base(options)
    {
        _taskConfiguration = taskConfiguration;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(_taskConfiguration);
        base.OnModelCreating(modelBuilder);
    }
}