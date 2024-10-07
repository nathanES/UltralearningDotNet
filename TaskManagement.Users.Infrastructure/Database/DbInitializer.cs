using Microsoft.EntityFrameworkCore;
using TaskManagement.Common.Interfaces;

namespace TaskManagement.Users.Infrastructure.Database;

public class DbInitializer(UsersContext usersContext) : IDbInitializer
{
    public async Task InitializeAsync()
    {
        //Create the database if it is not
        await usersContext.Database.MigrateAsync();
        //Add Seed data if needed
    }
}