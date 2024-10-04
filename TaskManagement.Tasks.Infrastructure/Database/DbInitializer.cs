using Microsoft.EntityFrameworkCore;
using TaskManagement.Tasks.Database;
using TaskManagement.Tasks.Infrastructure.Data;

namespace TaskManagement.Tasks.Infrastructure.Database;

public class DbInitializer : IDbInitializer
{

    private readonly TasksContext _context;

    public DbInitializer(TasksContext context)
    {
        _context = context;
    }

    public async Task InitializeAsync()
    {
        //Create the database if it is not
        await _context.Database.MigrateAsync();
        //Add Seed data if needed
    }
}