using Microsoft.EntityFrameworkCore;
using Task = TaskManagement.Tasks.Models.Task;

namespace TaskManagement.Tasks.Infrastructure.Data;

public class TasksContext(DbContextOptions<TasksContext> options) : DbContext(options)
{
    public DbSet<Task> Tasks => Set<Task>();
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     
    //     base.OnConfiguring(optionsBuilder);
    // }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            // Other configurations
        });
        base.OnModelCreating(modelBuilder);
    }
}