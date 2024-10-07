using Microsoft.EntityFrameworkCore;
using TaskManagement.Common.Interfaces;

namespace TaskManagement.Tasks.Infrastructure.Database;

public class DbInitializer(TasksContext tasksContext) : IDbInitializer
{
    public async System.Threading.Tasks.Task InitializeAsync()
    {
        //Create the database if it is not
        await tasksContext.Database.MigrateAsync();
        //Add Seed data if needed
    }
}