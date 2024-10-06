using Microsoft.EntityFrameworkCore;
using TaskManagement.Users.Interfaces;

namespace TaskManagement.Users.Infrastructure.Database;

public class DbInitializer : IDbInitializer
{
    
    private readonly UsersContext _usersContext;

    public DbInitializer(UsersContext usersContext)
    {
        _usersContext = usersContext;
    }

    public async Task InitializeAsync()
    {
        //Create the database if it is not
        await _usersContext.Database.MigrateAsync();
        //Add Seed data if needed
    }
}