using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Common.Models;
using TaskManagement.Users.Infrastructure.Database;
using TaskManagement.Users.Models;
using TaskManagement.Users.Repositories;

namespace TaskManagement.Users.Infrastructure.Repositories;

public class PostgreUserRepository(ILogger<PostgreUserRepository> logger, UsersContext context) : IUserRepository
{
    public async Task<bool> CreateAsync(User user, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(CreateAsync)} - Start");

        if (await ExistsByIdAsync(user.Id, token))
            return false;
        await context.Users.AddAsync(user, token);
        await context.SaveChangesAsync(token);

        logger.LogDebug($"{nameof(CreateAsync)} - End");
        return true;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(GetByIdAsync)} - Start");
        var user = await context.Users.FindAsync(new object[] { id }, token);
        logger.LogDebug($"{nameof(GetByIdAsync)} - End");
        return user;
    }

    public async Task<IEnumerable<User>> GetAllAsync(GetAllUsersOptions options, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(GetAllAsync)} - Start");
        var query = context.Users.AsQueryable();
        if (string.IsNullOrWhiteSpace(options.Email))
        {
            query.Where(t => t.Email!.Contains(options.Email));
        }

        if (!string.IsNullOrWhiteSpace(options.Username))
        {
            query.Where(t => t.Username == options.Username);
        }

        var users = await query
            .Skip((options.Page - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync(token);
        logger.LogDebug($"{nameof(GetAllAsync)} - End");
        return users;
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(UpdateAsync)} - Start");
        var userToUpdate = await context.Users.FindAsync(new object[] { user.Id }, token);
        if (userToUpdate is null)
            return false;

        userToUpdate.UpdateUsername(user.Username);
        userToUpdate.UpdateEmail(user.Email);

        await context.SaveChangesAsync(token);
        logger.LogDebug($"{nameof(UpdateAsync)} - End");
        return true;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(DeleteByIdAsync)} - Start");
        var userToDelete = await context.Users.FindAsync(new object[]{id}, token);
        if (userToDelete is null)
        {
            return false;
        }

        context.Users.Remove(userToDelete);
        await context.SaveChangesAsync(token);
        logger.LogDebug($"{nameof(DeleteByIdAsync)} - End");
        return true;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(ExistsByIdAsync)} - Start");
        var doesExists = await context.Users.AnyAsync(x => x.Id == id, token);
        logger.LogDebug($"{nameof(ExistsByIdAsync)} - End");
        return doesExists;
    }
}