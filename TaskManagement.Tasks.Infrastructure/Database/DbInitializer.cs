using Microsoft.EntityFrameworkCore;
using TaskManagement.Common.Interfaces;

namespace TaskManagement.Tasks.Infrastructure.Database;

public class DbInitializer(TasksContext tasksContext) : IDbInitializer
{
    public async System.Threading.Tasks.Task InitializeAsync(CancellationToken token = default)
    {
        //Create the database if it is not
        await tasksContext.Database.MigrateAsync(token);
        //Add Seed data if needed
    }

    public async Task<bool> HealthCheckAsync(CancellationToken token = default)
    {
        try
        {
            // Check if the database connection can be opened
            return await tasksContext.Database.CanConnectAsync(token);
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}