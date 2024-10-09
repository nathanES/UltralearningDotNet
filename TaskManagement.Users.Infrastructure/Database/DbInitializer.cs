using Microsoft.EntityFrameworkCore;
using TaskManagement.Common.Interfaces;

namespace TaskManagement.Users.Infrastructure.Database;

public class DbInitializer(UsersContext usersContext) : IDbInitializer
{
    public async Task InitializeAsync(CancellationToken token = default)
    {
        //Create the database if it is not
        await usersContext.Database.MigrateAsync(token);
        //Add Seed data if needed
    }
    
    public async Task<bool> HealthCheckAsync(CancellationToken token = default)
    {
        try
        {
            // Check if the database connection can be opened
            return await usersContext.Database.CanConnectAsync(token);
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}