using Microsoft.EntityFrameworkCore;

namespace TaskManagement.Tasks.Infrastructure.Database;

public class TasksContext(DbContextOptions<TasksContext> options) : DbContext(options)
{
    public DbSet<Task> Tasks => Set<Task>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Set the table name to lowercase with the first letter in lowercase
            entity.SetTableName(ToLowerCaseFirstLetter(entity.GetTableName()));

            // Loop through all properties of the entity and set the column name to lowercase
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.Name.ToLowerInvariant());
            }
        }
        
        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            // Other configurations
            entity.Property(t => t.UserId)
                .IsRequired(false);
            entity.Property(e => e.Id).HasColumnName("id");
        });
        base.OnModelCreating(modelBuilder);
    }
    
    private static string ToLowerCaseFirstLetter(string name)
    {
        if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
            return name;

        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }
    
}