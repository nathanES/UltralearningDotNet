using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Common.Models;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;
using TaskManagement.Users.Infrastructure.Database;
using TaskManagement.Users.Interfaces;
using TaskManagement.Users.Models;

namespace TaskManagement.Users.Infrastructure.Repositories;

public class PostgresUserRepository(ILogger<PostgresUserRepository> logger, UsersContext context) : IUserRepository
{
    public async Task<Result<User>> CreateAsync(User user, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(CreateAsync)} - Start");
        return await ResultExtensions.TryExecuteAsync(async () =>
        {
            await context.Users.AddAsync(user, token);
            await context.SaveChangesAsync(token);
            return Result<User>.Success(user);
        }, ex =>
        {
            logger.LogError(ex, "An error occurred while creating task");
            return new List<Error>
            {
                new DatabaseError("Failed to create user", "User",
                    JsonSerializer.Serialize(user, new JsonSerializerOptions { WriteIndented = true }),
                    details: ex.Message)
            };
        });
    }

    public async Task<Result<User>> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(GetByIdAsync)} - Start");
        return await ResultExtensions.TryExecuteAsync(async () =>
        {
            var user = await context.Users.FindAsync(new object[] { id }, token);
            if (user is null)
            {
                logger.LogWarning("User not found while trying to retrieving it");
                return Result<User>.Failure(new NotFoundError("user", id.ToString()));
            }

            return Result<User>.Success(user);
        }, ex =>
        {
            logger.LogError(ex, "An error occurred while retrieving user");

            return new List<Error>
            {
                new DatabaseError("Failed to retrieve user", "User", id.ToString(), ex.Message)
            }; 
        });
    }

    public async Task<Result<IEnumerable<User>>> GetAllAsync(GetAllUsersOptions options, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(GetAllAsync)} - Start");
        return await ResultExtensions.TryExecuteAsync(async () =>
        {
            var query = context.Users.AsNoTracking().AsQueryable();
            if (string.IsNullOrWhiteSpace(options.Email))
            {
                query = query.Where(t => t.Email!.Contains(options.Email));
            }

            if (!string.IsNullOrWhiteSpace(options.Username))
            {
                query = query.Where(t => t.Username == options.Username);
            }

            var users = await query
                .Skip((options.Page - 1) * options.PageSize)
                .Take(options.PageSize)
                .ToListAsync(token);
            return Result<IEnumerable<User>>.Success(users);
        }, ex =>
        {
            logger.LogError(ex, "An error occurred while retrieving all users");

            return new List<Error>
            {
                new DatabaseError("Failed to retrieve all users", "User",
                    JsonSerializer.Serialize(options, new JsonSerializerOptions { WriteIndented = true }),
                    details: ex.Message)
            }; 
        });
    }

    public async Task<Result<User>> UpdateAsync(User user, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(UpdateAsync)} - Start");
        return await ResultExtensions.TryExecuteAsync(async () =>
        {
            var userToUpdate = await context.Users.FindAsync(new object[] { user.Id }, token);
            if (userToUpdate is null)
            {
                logger.LogError("User not found while trying to update it");
                return Result<User>.Failure(new NotFoundError("user", user.Id.ToString()));
            }

            userToUpdate.UpdateUsername(user.Username);
            userToUpdate.UpdateEmail(user.Email);

            await context.SaveChangesAsync(token);
            return Result<User>.Success(userToUpdate);
        }, ex =>
        {
            logger.LogError(ex, "An error occurred while updating user");

            return new List<Error>
            {
                new DatabaseError("Failed to update user", "User", user.Id.ToString(), ex.Message)
            };
        });
    }

    public async Task<Result<None>> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(DeleteByIdAsync)} - Start");
        return await ResultExtensions.TryExecuteAsync(async () =>
        {
            var userToDelete = await context.Users.FindAsync(new object[]{id}, token);
            if (userToDelete is null)
            {
                logger.LogError("User not found while trying to delete it");
                return Result<None>.Failure(new NotFoundError("user", id.ToString()));
            }

            context.Users.Remove(userToDelete);
            await context.SaveChangesAsync(token);
            return Result<None>.Success(None.Value);
        }, ex =>
        {
            logger.LogError(ex, "An error occurred while deleting user");

            return new List<Error>
            {
                new DatabaseError("Failed to delete user", "User", id.ToString(), ex.Message)
            };
        });
    }

    public async Task<Result<bool>> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(ExistsByIdAsync)} - Start");
        return await ResultExtensions.TryExecuteAsync(async () =>
        {
            var doesExist = await context.Users.AnyAsync(x => x.Id == id, token);
            logger.LogDebug($"{nameof(ExistsByIdAsync)} - User existence: {doesExist}");
            return Result<bool>.Success(doesExist);
        }, ex =>
        {
            logger.LogError(ex, "An error occurred while checking if the user exists.");
            return new List<Error>()
            {
                new DatabaseError("Failed to check if user exists.", "User", id.ToString(), ex.Message)
            };
        });
    }
}