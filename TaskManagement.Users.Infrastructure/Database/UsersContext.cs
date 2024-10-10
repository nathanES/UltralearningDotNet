using Microsoft.EntityFrameworkCore;
using TaskManagement.Common.Models;
using TaskManagement.Users.Models;

namespace TaskManagement.Users.Infrastructure.Database;

public class UsersContext(DbContextOptions<UsersContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
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
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            // Other configurations
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