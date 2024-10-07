using Microsoft.EntityFrameworkCore;
using TaskManagement.Users.Models;

namespace TaskManagement.Users.Infrastructure.Database;

public class UsersContext(DbContextOptions<UsersContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            // Other configurations
        });
        base.OnModelCreating(modelBuilder);
    }

}