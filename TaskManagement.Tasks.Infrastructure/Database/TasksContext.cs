using Microsoft.EntityFrameworkCore;

namespace TaskManagement.Tasks.Infrastructure.Database;

public class TasksContext(DbContextOptions<TasksContext> options) : DbContext(options)
{
    public DbSet<Task> Tasks => Set<Task>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            // Other configurations
            entity.Property(t => t.UserId)
                .IsRequired(false);
        });
        base.OnModelCreating(modelBuilder);
    }
}