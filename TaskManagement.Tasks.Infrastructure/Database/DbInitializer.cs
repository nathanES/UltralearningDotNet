using Microsoft.EntityFrameworkCore;
using TaskManagement.Tasks.Interfaces;

namespace TaskManagement.Tasks.Infrastructure.Database;

public class DbInitializer : IDbInitializer
{

    private readonly TasksContext _tasksContext;

    public DbInitializer(TasksContext tasksContext)
    {
        _tasksContext = tasksContext;
    }

    public async Task InitializeAsync()
    {
        //Create the database if it is not
        await _tasksContext.Database.MigrateAsync();
        //Add Seed data if needed
    }
}